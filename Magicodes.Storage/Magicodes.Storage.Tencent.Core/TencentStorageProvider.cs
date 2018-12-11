// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : TencentStorageProvider.cs
//           description :
//   
//           created by 雪雁 at  2018-08-02 9:59
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using Amazon.S3;
using Amazon.S3.Model;
using Magicodes.Storage.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    ///     腾讯云存储提供服务
    /// </summary>
    public class TencentStorageProvider : IStorageProvider
    {
        private readonly TencentCosConfig _tcConfig;
        private readonly AmazonS3Client _amazonS3Client;

        /// <summary>
        ///     腾讯云存储对象提供构造函数
        /// </summary>
        /// <param name="tcConfig">配置信息</param>
        public TencentStorageProvider(TencentCosConfig tcConfig)
        {
            _tcConfig = tcConfig;
            var config = new AmazonS3Config
            {
                //注意要将<region>替换为相对应的region，如ap-beijing，ap-guangzhou...
                //ServiceURL = $"http://{bucket.Name}.cos.{bucket.Region}.myqcloud.com",
                ServiceURL = $"http://cos.{tcConfig.Region}.myqcloud.com",
            };
            _amazonS3Client = new AmazonS3Client(
                tcConfig.SecretId,
                tcConfig.SecretKey,
                config
            );
        }

        /// <summary>
        /// 提供服务名称
        /// </summary>
        public string ProviderName => "TencentCOS";


        /// <summary>
        ///     删除对象
        /// </summary>
        /// <param name="containerName">容器(Bucket)的地址</param>
        /// <param name="blobName">文件名称</param>
        public async Task DeleteBlob(string containerName, string blobName)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _tcConfig.BucketName,
                Key = $"{containerName}/{blobName}"
            };
            var response = await _amazonS3Client.DeleteObjectAsync(request);
            response.HandlerError("删除对象出错!");
        }

        /// <summary>
        ///     删除容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task DeleteContainer(string containerName)
        {
            //删除目录等于删除该目录下的所有文件
            var objs = await ListBlobs(containerName);
            var count = objs.Count / 1000 + (objs.Count % 1000 > 0 ? 1 : 0);

            for (var i = 0; i < count; i++)
            {
                var request = new DeleteObjectsRequest
                {
                    BucketName = _tcConfig.BucketName,
                    Objects = objs.Skip(i * 1000).Take(1000).Select(p => new KeyVersion
                    {
                        Key = $"{containerName}/{p.Name}",
                        VersionId = null
                    }).ToList()
                };
                var response = await _amazonS3Client.DeleteObjectsAsync(request);
                response.HandlerError("删除对象时出错(删除目录会删除该目录下所有的文件)!");
            }
        }

        /// <summary>
        ///     获取文件信息
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName)
        {
            var request = new GetObjectRequest
            {
                BucketName = _tcConfig.BucketName,
                Key = $"{containerName}/{blobName}"
            };
            var response = await _amazonS3Client.GetObjectAsync(request);
            response.HandlerError("获取文件信息出错!");
            return new BlobFileInfo
            {
                Container = containerName,
                ContentMD5 = response.Headers.ContentMD5,
                ContentType = response.Headers.ContentType,
                ETag = response.ETag,
                Length = response.ContentLength,
                LastModified = response.LastModified,
                Name = blobName,
                Url = GetUrlByKey(response.Key)
            };
        }

        /// <summary>
        ///     获取文件的流信息
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<Stream> GetBlobStream(string containerName, string blobName)
        {
            var request = new GetObjectRequest
            {
                BucketName = _tcConfig.BucketName,
                Key = $"{containerName}/{blobName}"
            };
            var response = await _amazonS3Client.GetObjectAsync(request);
            response.HandlerError("下载文件出错!");
            return response.ResponseStream;
        }



        public Task<string> GetBlobUrl(string containerName, string blobName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _tcConfig.BucketName,
                Key = $"{containerName}/{blobName}",
                Expires = DateTime.Now
            };
            var url = _amazonS3Client.GetPreSignedURL(request);
            return Task.FromResult(url);
        }

        /// <summary>
        ///     获取授权访问链接
        /// </summary>
        /// <param name="containerName">容器名称</param>
        /// <param name="blobName">文件名称</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="isDownload">是否允许下载</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="access">访问限制</param>
        /// <returns></returns>
        public Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry, bool isDownload = false,
            string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _tcConfig.BucketName,
                Key = $"{containerName}/{blobName}",
                Expires = expiry,
                ContentType = contentType
            };

            var url = _amazonS3Client.GetPreSignedURL(request);
            return Task.FromResult(url);
        }

        /// <summary>
        ///     列出指定容器下的对象列表
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task<IList<BlobFileInfo>> ListBlobs(string containerName)
        {
            if (!string.IsNullOrWhiteSpace(containerName) && !containerName.EndsWith("/"))
            {
                containerName += "/";
            }

            var req = new ListObjectsV2Request
            {
                BucketName = _tcConfig.BucketName,
                Prefix = containerName
            };
            var resp = await _amazonS3Client.ListObjectsV2Async(req);
            resp.HandlerError("获取对象列表出错!");
            var list = resp.S3Objects
                .Select(obj =>
                    new BlobFileInfo
                    {
                        Container = containerName?.Trim('/'),
                        ETag = obj.ETag,
                        Length = obj.Size,
                        LastModified = obj.LastModified,
                        Name = obj.Key.Replace(containerName, string.Empty),
                        Url = GetUrlByKey(obj.Key),
                        //ContentMD5 = 
                    });
            return list.ToArray();
        }

        /// <summary>
        /// 根据对象Key获取Url
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetUrlByKey(string key) => $"http://{_tcConfig.BucketName}.cos.{_tcConfig.Region}.myqcloud.com/{key}";

        /// <summary>
        ///     保存对象到指定的容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="source"></param>
        public async Task SaveBlobStream(string containerName, string blobName, Stream source)
        {
            var request = new PutObjectRequest
            {
                BucketName = _tcConfig.BucketName,
                Key = $"{containerName}/{blobName}",
                InputStream = source
            };
            var response = await _amazonS3Client.PutObjectAsync(request);
            response.HandlerError("上传对象出错!");
        }

    }
}