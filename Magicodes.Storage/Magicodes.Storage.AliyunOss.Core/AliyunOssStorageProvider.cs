using Aliyun.OSS;
using Aliyun.OSS.Util;
using Magicodes.Storage.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Storage.AliyunOss.Core
{
    public class AliyunOssStorageProvider : IStorageProvider
    {
        private readonly AliyunOssConfig _cfg;
        private readonly string _baseUrl;
        private readonly OssClient _ossClient;

        public AliyunOssStorageProvider(AliyunOssConfig cfg)
        {
            _cfg = cfg;
            _ossClient = new OssClient(cfg.Endpoint, cfg.AccessKeyId, cfg.AccessKeySecret);

            _baseUrl = $"https://{cfg.BucketName}.{cfg.Endpoint}";
        }

        public string ProviderName => "AliyunOss";

        /// <summary>
        ///     保存对象到指定的容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="source"></param>
        public async Task SaveBlobStream(string containerName, string blobName, Stream source)
        {
            try
            {
                await Task.Run(() =>
                    {
                        var key = $"{containerName}/{blobName}";
                        var md5 = OssUtils.ComputeContentMd5(source, source.Length);
                        var objectMeta = new ObjectMetadata();
                        objectMeta.AddHeader("Content-MD5", md5);
                        objectMeta.UserMetadata.Add("Content-MD5", md5);
                        _ossClient.PutObject(_cfg.BucketName, key, source, objectMeta).HandlerError("上传对象出错");
                    });
            }
            catch (Exception ex)
            {
                throw new StorageException(StorageErrorCode.PostError.ToStorageError(),
                    new Exception(ex.ToString()));
            }
        }

        /// <summary>
        ///     获取对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<Stream> GetBlobStream(string containerName, string blobName)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var key = $"{containerName}/{blobName}";
                    var blob = _ossClient.GetObject(_cfg.BucketName, key).HandlerError("获取对象出错");
                    if (blob == null || blob.ContentLength == 0)
                    {
                        throw new StorageException(StorageErrorCode.FileNotFound.ToStorageError(),
                            new Exception("没有找到该文件"));
                    }
                    return blob.Content;
                });
            }
            catch (Exception ex)
            {
                throw new StorageException(StorageErrorCode.ErrorOpeningBlob.ToStorageError(),
                    new Exception(ex.ToString()));
            }
        }

        /// <summary>
        ///     获取文件链接
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<string> GetBlobUrl(string containerName, string blobName) => await Task.Run(() => $"{_baseUrl}/{containerName}/{blobName}");

        /// <summary>
        ///     获取对象属性
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var key = $"{containerName}/{blobName}";
                    var result = _ossClient.GetObjectMetadata(_cfg.BucketName, key);
                    return new BlobFileInfo
                    {
                        Container = containerName,
                        ETag = result.ETag,
                        LastModified = result.LastModified,
                        Name = blobName,
                        Length = result.ContentLength,
                        Url = string.Format(_baseUrl, containerName, blobName),
                        ContentMD5 = result.ContentMd5,
                        ContentType = result.ContentType
                    };
                });
            }
            catch (Exception ex)
            {
                throw new StorageException(StorageErrorCode.PostError.ToStorageError(),
                    new Exception(ex.ToString()));
            }
        }

        /// <summary>
        ///     列出指定容器下的对象列表
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task<IList<BlobFileInfo>> ListBlobs(string containerName)
        {
            var blobFileInfos = new List<BlobFileInfo>();
            try
            {
                return await Task.Run(() =>
                {
                    if (!string.IsNullOrWhiteSpace(containerName) && !containerName.EndsWith("/"))
                    {
                        containerName += "/";
                    }
                    var listObjectsRequest = new ListObjectsRequest(_cfg.BucketName)
                    {
                        Prefix = containerName
                    };
                    var result = _ossClient.ListObjects(listObjectsRequest).HandlerError("获取对象列表出错！");
                    foreach (var summary in result.ObjectSummaries)
                    {
                        blobFileInfos.Add(new BlobFileInfo
                        {
                            Container = summary.BucketName,
                            ETag = summary.ETag,
                            LastModified = summary.LastModified,
                            Name = summary.Key,
                            Length = summary.Size,
                            Url = string.Format(_baseUrl, summary.BucketName, summary.Key)
                        });
                    }

                    return blobFileInfos;
                });
            }
            catch (Exception ex)
            {
                throw new StorageException(StorageErrorCode.PostError.ToStorageError(),
                    new Exception(ex.ToString()));
            }
        }

        /// <summary>
        ///     删除对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        public async Task DeleteBlob(string containerName, string blobName)
        {
            try
            {
                await Task.Run(() =>
                {
                    var key = $"{containerName}/{blobName}";
                    _ossClient.DeleteObject(_cfg.BucketName, key);

                });
            }
            catch (Exception ex)
            {
                throw new StorageException(StorageErrorCode.PostError.ToStorageError(),
                    new Exception(ex.ToString()));
            }
        }

        /// <summary>
        ///     删除目录（会删除下面所有的文件）
        /// </summary>
        /// <param name="containerName"></param>
        public async Task DeleteContainer(string containerName)
        {
            try
            {
                //删除目录等于删除该目录下的所有文件
                var blobs = await ListBlobs(containerName);
                await Task.Run(() =>
                {
                    var count = blobs.Count / 1000 + (blobs.Count % 1000 > 0 ? 1 : 0);

                    for (var i = 0; i < count; i++)
                    {
                        var request = new DeleteObjectsRequest(_cfg.BucketName,
                            blobs.Skip(i * 1000).Take(1000).Select(p => $"{containerName}/{p.Name}").ToList());

                        _ossClient.DeleteObjects(request).HandlerError("删除对象时出错(删除目录会删除该目录下所有的文件)!");
                    }
                });
            }
            catch (Exception ex)
            {
                throw new StorageException(StorageErrorCode.PostError.ToStorageError(),
                    new Exception(ex.ToString()));
            }
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
        public async Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry,
            bool isDownload = false,
            string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read)
        {
            try
            {
                var httpMethod = SignHttpMethod.Get;
                return await Task.Run(() =>
                {
                    switch (access)
                    {
                        case BlobUrlAccess.Read:
                            httpMethod = SignHttpMethod.Get;
                            break;
                        case BlobUrlAccess.All:
                        case BlobUrlAccess.Write:
                            httpMethod = SignHttpMethod.Put;
                            break;
                        case BlobUrlAccess.Delete:
                            httpMethod = SignHttpMethod.Delete;
                            break;
                        default:
                            throw new StorageException(StorageErrorCode.InvalidAccess.ToStorageError(),
                                new Exception("无效的访问凭据"));
                    }

                    var req = new GeneratePresignedUriRequest(containerName, blobName, httpMethod)
                    {
                        Expiration = expiry
                    };
                    var url = _ossClient.GeneratePresignedUri(req);
                    return url.AbsoluteUri;
                });
            }
            catch (Exception ex)
            {
                throw new StorageException(StorageErrorCode.PostError.ToStorageError(),
                    new Exception(ex.ToString()));
            }
        }
    }
}