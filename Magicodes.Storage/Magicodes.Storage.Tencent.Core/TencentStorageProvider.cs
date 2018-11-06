using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Magicodes.Storage.Core;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Linq;
using Amazon.Runtime;
/// <summary>
/// 腾讯云存储实现
/// </summary>
namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    /// 腾讯云存储提供服务
    /// </summary>
    public class TencentStorageProvider : IStorageProvider
    {
        /// 提供服务名称
        /// </summary>
        public string ProviderName => "TencentCOS";
        private readonly TencentCosService TencentCos;
        private readonly Bucket StorageBucket;
        private readonly BlobFileInfo BlobFileInfo;
        private readonly string _serverPath;
        private readonly AmazonS3Client amazonS3Client;
        /// <summary>
        /// 腾讯云存储对象提供构造函数
        /// </summary>
        /// <param name="cfg">配置信息</param>
        /// <param name="bucket">容器对象</param>
        /// <param name="backChannel"></param>
        public TencentStorageProvider(TencentCosConfig cfg, Bucket bucket)
        {
            StorageBucket = bucket;
            AmazonS3Config config = new AmazonS3Config
            {
                //注意要将<region>替换为相对应的region，如ap-beijing，ap-guangzhou...
                //ServiceURL = $"http://{bucket.Name}.cos.{bucket.Region}.myqcloud.com",
                ServiceURL = $"http://cos.{bucket.Region}.myqcloud.com",
            };
            amazonS3Client = new AmazonS3Client(
                    cfg.SecretId,
                    cfg.SecretKey,
                    config
                    );
        }


        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="containerName">容器(Bucket)的地址</param>
        /// <param name="blobName">文件名称</param>
        public async Task DeleteBlob(string containerName, string blobName)
        {
            // throw new NotImplementedException();
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = StorageBucket.Name,
                Key = $"{containerName}/{blobName}"
            };
            DeleteObjectResponse response = await amazonS3Client.DeleteObjectAsync(request);
          
        }

        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task DeleteContainer(string containerName)
        {

            DeleteBucketRequest request = new DeleteBucketRequest
            {
              //BucketName = StorageBucket.Name
               BucketName = $"{containerName}"
            };
            var response = await amazonS3Client.DeleteBucketAsync(request);
        }
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = StorageBucket.Name,
                Key = $"{containerName}/{blobName}"
            };
            GetObjectResponse response = await amazonS3Client.GetObjectAsync(request);
            CheckResult(response);
            return new BlobFileInfo()
            {
                Container = containerName,
                //ContentMD5 = response.
                //ContentType = response.
                ETag = response.ETag,
                Length = response.ContentLength,
                LastModified = response.LastModified,
                Name = blobName,
                //Url = response.u
            };

        }

        /// <summary>
        /// 获取文件的流信息
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<Stream> GetBlobStream(string containerName, string blobName)
        {
            var request = new GetObjectRequest
            {
                BucketName = StorageBucket.Name,
                Key = $"{containerName}/{blobName}"
            };
            GetObjectResponse response = await amazonS3Client.GetObjectAsync(request);
            CheckResult(response);
            return response.ResponseStream;

        }

        /// <summary>
        /// 获取授权访问链接
        /// </summary>
        /// <param name="containerName">容器名称</param>
        /// <param name="blobName">文件名称</param>
        /// <param name="expiry">过期时间</param>
        /// <param name="isDownload">是否允许下载</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="access">访问限制</param>
        /// <returns></returns>
        #region      url
        public Task<string> GetBlobUrl(string containerName, string blobName)
        {

            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
            {
                BucketName = StorageBucket.Name,
                Key = $"{containerName}/{blobName}",
                Expires=DateTime.Now
            };
            string url = amazonS3Client.GetPreSignedURL(request);
            return Task.FromResult(url);
        }
        #endregion
        public Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry, bool isDownload = false, string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read)
        {
            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
            {
                BucketName = StorageBucket.Name,
                Key = $"{containerName}/{blobName}",
                Expires = expiry,
                ContentType = contentType
            };

            string url = amazonS3Client.GetPreSignedURL(request);
            return Task.FromResult(url);
        }

        /// <summary>
        /// 列出指定容器下的对象列表
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
                BucketName = StorageBucket.Name,
                Prefix = containerName
            };
            var resp = await amazonS3Client.ListObjectsV2Async(req);
            CheckResult(resp);
            var list = resp.S3Objects
                .Select(obj =>
                     new BlobFileInfo
                     {
                         Container = obj.BucketName,
                         ETag = obj.ETag,
                         Length = obj.Size,
                         LastModified = obj.LastModified
                     });
            return list.ToArray();
        }

        /// <summary>
        /// 保存对象到指定的容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="source"></param>
        public async Task SaveBlobStream(string containerName, string blobName, Stream source)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = StorageBucket.Name,
                Key = $"{containerName}/{blobName}",
                InputStream = source,
            };
            var response = await amazonS3Client.PutObjectAsync(request);
            CheckResult(response);
        }

        /// <summary>
        /// 判断是否通过
        /// </summary>
        /// <param name="response"></param>
        private static void CheckResult(AmazonWebServiceResponse response)
        {
            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new StorageException(new StorageError()
                {
                    Code = Convert.ToInt32(response.HttpStatusCode),
                    Message = null
                }, null);
            }
        }
    }
}
