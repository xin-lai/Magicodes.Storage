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

using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using COSXML.Model.Bucket;
using COSXML.CosException;
using Magicodes.Storage.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using COSXML.Utils;
using COSXML.Model.Tag;

namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    ///     腾讯云存储提供服务
    /// </summary>
    public class TencentStorageProvider : IStorageProvider
    {
        private readonly TencentCosConfig _tcConfig;
        private readonly CosXmlServer _cosXmlServer;

        /// <summary>
        ///     腾讯云存储对象提供构造函数
        /// </summary>
        /// <param name="tcConfig">配置信息</param>
        public TencentStorageProvider(TencentCosConfig tcConfig)
        {
            _tcConfig = tcConfig;
            var config = new CosXmlConfig.Builder()
              .SetConnectionTimeoutMs(60000)  //设置连接超时时间，单位毫秒，默认45000ms
              .SetReadWriteTimeoutMs(40000)  //设置读写超时时间，单位毫秒，默认45000ms
              .IsHttps(true)  //设置默认 HTTPS 请求
              .SetAppid(tcConfig.AppId)  //设置腾讯云账户的账户标识 APPID
              .SetRegion(tcConfig.Region)  //设置一个默认的存储桶地域
              .SetDebugLog(true)  //显示日志
              .Build();  //创建 CosXmlConfig 对象

            //初始化 QCloudCredentialProvider，COS SDK 中提供了3种方式：永久密钥、临时密钥、自定义
            QCloudCredentialProvider cosCredentialProvider = new DefaultQCloudCredentialProvider(tcConfig.SecretId, tcConfig.SecretKey, 600);


            //初始化 CosXmlServer
            _cosXmlServer = new CosXmlServer(config, cosCredentialProvider);
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
            var request = new DeleteObjectRequest(_tcConfig.BucketName, $"{containerName}/{blobName}");
            //设置签名有效时长
            request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 600);
            var response = _cosXmlServer.DeleteObject(request);
            await response.HandlerError("删除对象出错!");
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
                var request = new DeleteMultiObjectRequest(_tcConfig.BucketName);
                request.SetObjectKeys(objs.Skip(i * 1000).Take(1000).Select(p => p.Name).ToList());
                var response = _cosXmlServer.DeleteMultiObjects(request);
                await response.HandlerError("删除对象时出错(删除目录会删除该目录下所有的文件)!");
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
            var key = $"{containerName}/{blobName}";
            var request = new HeadObjectRequest(_tcConfig.BucketName, key);
            var response = _cosXmlServer.HeadObject(request);
            await response.HandlerError("获取文件信息出错!");
            return new BlobFileInfo
            {
                Container = containerName,
                //ContentMD5 = response.Headers.ContentMD5,
                ContentType = response.responseHeaders.ContainsKey("Content-Type") ? (response.responseHeaders["Content-Type"].FirstOrDefault()) : null,
                ETag = response.eTag,
                Length = response.size,
                LastModified = response.responseHeaders.ContainsKey("Last-Modified") ? DateTime.Parse(response.responseHeaders["Last-Modified"].FirstOrDefault()) : (DateTime?)null,
                Name = blobName,
                Url = GetUrlByKey(key)
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
            var request = new GetObjectBytesRequest(_tcConfig.BucketName, $"{containerName}/{blobName}");

            var response = _cosXmlServer.GetObject(request);


            await response.HandlerError("下载文件出错!");
            byte[] content = response.content;
            return new MemoryStream(content);
        }



        public Task<string> GetBlobUrl(string containerName, string blobName)
        {
            var preSignatureStruct = new PreSignatureStruct();
            preSignatureStruct.appid = _tcConfig.AppId;//腾讯云账号 APPID
            preSignatureStruct.region = _tcConfig.Region; //存储桶地域
            preSignatureStruct.bucket = _tcConfig.BucketName; //存储桶
            preSignatureStruct.key = $"{containerName}/{blobName}"; //对象键
            preSignatureStruct.httpMethod = "PUT"; //HTTP 请求方法
            preSignatureStruct.isHttps = true; //生成 HTTPS 请求 URL
            preSignatureStruct.signDurationSecond = 600; //请求签名时间为 600s
            preSignatureStruct.headers = null;//签名中需要校验的 header
            preSignatureStruct.queryParameters = null; //签名中需要校验的 URL 中请求参数

            var url = _cosXmlServer.GenerateSignURL(preSignatureStruct);
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
            var preSignatureStruct = new PreSignatureStruct();
            preSignatureStruct.appid = _tcConfig.AppId;//腾讯云账号 APPID
            preSignatureStruct.region = _tcConfig.Region; //存储桶地域
            preSignatureStruct.bucket = _tcConfig.BucketName; //存储桶
            preSignatureStruct.key = $"{containerName}/{blobName}"; //对象键
            preSignatureStruct.httpMethod = "PUT"; //HTTP 请求方法
            preSignatureStruct.isHttps = true; //生成 HTTPS 请求 URL
            preSignatureStruct.signDurationSecond = (long)(expiry - DateTime.Now).TotalSeconds; //请求签名时间为 600s
            preSignatureStruct.headers = null;//签名中需要校验的 header
            preSignatureStruct.queryParameters = null; //签名中需要校验的 URL 中请求参数

            var url = _cosXmlServer.GenerateSignURL(preSignatureStruct);
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

            var req = new GetBucketRequest(_tcConfig.BucketName);
            req.SetPrefix(containerName);
            var resp = _cosXmlServer.GetBucket(req);
            await resp.HandlerError("获取对象列表出错!");
            var list = resp.listBucket.contentsList
                .Select(obj =>
                    new BlobFileInfo
                    {
                        Container = containerName?.Trim('/'),
                        ETag = obj.eTag,
                        Length = obj.size,
                        LastModified = Convert.ToDateTime(obj.lastModified),
                        Name = obj.key.Replace(containerName, string.Empty),
                        Url = GetUrlByKey(obj.key),
                        //ContentMD5 = 
                    });
            return list.ToArray();
        }

        /// <summary>
        /// 根据对象Key获取Url
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetUrlByKey(string key) => $"https://{_tcConfig.BucketName}.cos.{_tcConfig.Region}.myqcloud.com/{key}";

        /// <summary>
        ///     保存对象到指定的容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="source"></param>
        public async Task SaveBlobStream(string containerName, string blobName, Stream source)
        {
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                source.CopyTo(ms);
                bytes = ms.ToArray();
            }

            var request = new PutObjectRequest(_tcConfig.BucketName, $"{containerName}/{blobName}", bytes)
            {
            };

            var response = _cosXmlServer.PutObject(request);

            await response.HandlerError("上传对象出错!");
        }

    }
}