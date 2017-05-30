using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Util;

namespace Magicodes.Storage.Aliyun.Core
{
    public sealed class AliyunOssStorageProvider : IStorageProvider
    {
        private readonly OssClient _ossClient;
        private readonly string _baseUrl;

        public AliyunOssStorageProvider(AliyunOssProviderOptions options)
        {
            _baseUrl = options.Endpoint;
            _ossClient = options.Config == null ? new OssClient(options.Endpoint, options.AccessKeyId, options.AccessKeySecret) : new OssClient(new Uri(options.Endpoint), options.AccessKeyId, options.AccessKeySecret, options.Config);
        }

        public void DeleteBlob(string containerName, string blobName)
        {
            AsyncHelpers.RunSync(() => DeleteBlobAsync(containerName, blobName));
        }

        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            try
            {
                await Task.Run(() => _ossClient.DeleteObject(containerName, blobName));
            }
            catch (Exception e)
            {
                if (!e.IsOssStorageException()) throw;
                var ex = e.Convert() as StorageException;
                //不存在不报错
                if (ex != null && (ex.ErrorCode == (int)StorageErrorCode.InvalidContainerName || ex.ErrorCode == (int)StorageErrorCode.InvalidBlobName))
                {
                    return;
                }
                if (ex != null) throw ex;
            }
        }

        public void DeleteContainer(string containerName)
        {
            AsyncHelpers.RunSync(() => DeleteContainerAsync(containerName));
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            try
            {
                await Task.Run(() => _ossClient.DeleteBucket(containerName));
            }
            catch (Exception e)
            {
                if (e.IsOssStorageException())
                {
                    var ex = e.Convert() as StorageException;
                    //不存在不报错
                    if (ex.ErrorCode == (int)StorageErrorCode.InvalidContainerName)
                    {
                        return;
                    }
                    throw ex;
                }
                throw;
            }
        }

        public BlobDescriptor GetBlobDescriptor(string containerName, string blobName)
        {
            return AsyncHelpers.RunSync(() => GetBlobDescriptorAsync(containerName, blobName));
        }

        public async Task<BlobDescriptor> GetBlobDescriptorAsync(string containerName, string blobName)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var props = _ossClient.GetObjectMetadata(containerName, blobName);
                    var userMeta = props.UserMetadata;
                    return new BlobDescriptor
                    {
                        Name = blobName,
                        Container = containerName,
                        Url = _baseUrl + "/" + containerName + "/" + blobName,
                        ContentType = props.ContentType,
                        ContentMD5 = userMeta != null && userMeta.ContainsKey("ContentMD5") ? userMeta["ContentMD5"] : null,
                        ETag = props.ETag,
                        LastModified = props.LastModified,
                        Length = props.ContentLength,
                    };
                });
            }
            catch (Exception e)
            {
                if (e.IsOssStorageException())
                {
                    throw e.Convert();
                }
                else
                {
                    var webEx = e as WebException;
                    if (webEx?.Status == System.Net.WebExceptionStatus.ProtocolError)
                    {
                        throw new StorageException(new StorageError() { Code = (int)StorageErrorCode.InvalidCredentials, Message = "访问被拒绝，请检查配置！" }, webEx);
                    }
                }
                throw;
            }
        }

        public Stream GetBlobStream(string containerName, string blobName)
        {
            return AsyncHelpers.RunSync(() => GetBlobStreamAsync(containerName, blobName));
        }

        public async Task<Stream> GetBlobStreamAsync(string containerName, string blobName)
        {
            try
            {
                return await Task.Run(() =>
                 {
                     var blob = _ossClient.GetObject(containerName, blobName);
                     return blob.Content;
                 });
                //var blob = _ossClient.GetObject(containerName, blobName);
                //return Task.FromResult<Stream>(blob.Content);
            }
            catch (Exception e)
            {
                if (e.IsOssStorageException())
                {
                    throw e.Convert();
                }
                else
                {
                    var webEx = e as WebException;
                    if (webEx?.Status == System.Net.WebExceptionStatus.ProtocolError)
                    {
                        throw new StorageException(new StorageError() { Code = (int)StorageErrorCode.InvalidCredentials, Message = "访问被拒绝，请检查配置！" }, webEx);
                    }
                }
                throw;
            }
        }

        public string GetBlobUrl(string containerName, string blobName)
        {
            return _baseUrl + "/" + containerName + "/" + blobName;
        }

        public string GetBlobSasUrl(string containerName, string blobName, DateTimeOffset expiry, bool isDownload = false,
            string filename = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read)
        {
            var req = new GeneratePresignedUriRequest(containerName, blobName)
            {
                Expiration = expiry.DateTime
            };
            switch (access)
            {
                case BlobUrlAccess.None:
                case BlobUrlAccess.Read:
                    req.Method = SignHttpMethod.Get;
                    break;
                case BlobUrlAccess.Write:
                    req.Method = SignHttpMethod.Put;
                    break;
                case BlobUrlAccess.Delete:
                    throw new NotSupportedException("BlobUrlAccess.Delete");
                case BlobUrlAccess.All:
                    throw new NotSupportedException("BlobUrlAccess.All");
            }
            if (!string.IsNullOrWhiteSpace(contentType))
                req.ContentType = contentType;

            if (!string.IsNullOrWhiteSpace(filename))
                req.ResponseHeaders.ContentDisposition = "inline;filename=" + filename;

            var uri = _ossClient.GeneratePresignedUri(req);
            return uri.ToString();
        }

        public IList<BlobDescriptor> ListBlobs(string containerName)
        {
            return AsyncHelpers.RunSync(() => ListBlobsAsync(containerName));
        }

        public async Task<IList<BlobDescriptor>> ListBlobsAsync(string containerName)
        {
            List<BlobDescriptor> list = null;
            try
            {
                await Task.Run(() =>
                {
                    var listObjectsRequest = new ListObjectsRequest(containerName);
                    var result = _ossClient.ListObjects(listObjectsRequest);

                    var security = BlobSecurity.Public;
                    list = result.ObjectSummaries.Select(p =>
                    {
                        var blob = GetBlobDescriptor(containerName, p.Key);
                        //var blob = new BlobDescriptor()
                        //{
                        //    Name = p.Key,
                        //    Container = containerName,
                        //    //ContentMD5=
                        //    //ContentType = p.Owne
                        //    ETag = p.ETag,
                        //    LastModified = p.LastModified,
                        //    Length = p.Size,
                        //    Security = security,
                        //    Url = _baseUrl + "/" + containerName + "/" + p.Key
                        //};
                        return blob;
                    }).ToList();
                });
            }
            catch (Exception e)
            {
                if (e.IsOssStorageException())
                {
                    throw e.Convert();
                }
                else
                {
                    var webEx = e as WebException;
                    if (webEx?.Status == System.Net.WebExceptionStatus.ProtocolError)
                    {
                        throw new StorageException(new StorageError() { Code = (int)StorageErrorCode.InvalidCredentials, Message = "访问被拒绝，请检查配置！" }, webEx);
                    }
                }
                throw;
            }

            return list;
        }

        public void SaveBlobStream(string containerName, string blobName, Stream source, BlobProperties properties = null)
        {
            AsyncHelpers.RunSync(() => SaveBlobStreamAsync(containerName, blobName, source, properties));
        }

        public async Task SaveBlobStreamAsync(string containerName, string blobName, Stream source, BlobProperties properties = null)
        {
            try
            {
                await Task.Run(() =>
                {
                    var exist = _ossClient.DoesBucketExist(containerName);
                    if (!exist)
                    {
                        _ossClient.CreateBucket(containerName);
                    }
                    var md5 = OssUtils.ComputeContentMd5(source, source.Length);
                    var objectMeta = new ObjectMetadata();
                    objectMeta.AddHeader("Content-MD5", md5);
                    objectMeta.UserMetadata.Add("Content-MD5", md5);
                    if (properties != null)
                    {
                        objectMeta.ContentType = properties.ContentType;
                    }
                    _ossClient.PutObject(containerName, blobName, source, objectMeta);
                });
            }
            catch (Exception ex)
            {
                if (ex.IsOssStorageException())
                {
                    throw ex.Convert();
                }
                throw;
            }
        }

        public void UpdateBlobProperties(string containerName, string blobName, BlobProperties properties)
        {
            AsyncHelpers.RunSync(() => UpdateBlobPropertiesAsync(containerName, blobName, properties));
        }

        public async Task UpdateBlobPropertiesAsync(string containerName, string blobName, BlobProperties properties)
        {
            try
            {
                await Task.Run(() =>
                {
                    var meta = _ossClient.GetObjectMetadata(containerName, blobName);
                    meta.ContentType = properties.ContentType;
                    _ossClient.ModifyObjectMeta(containerName, blobName, meta);
                });
            }
            catch (Exception e)
            {
                if (e.IsOssStorageException())
                {
                    throw e.Convert();
                }
                else
                {
                    var webEx = e as WebException;
                    if (webEx?.Status == System.Net.WebExceptionStatus.ProtocolError)
                    {
                        throw new StorageException(new StorageError() { Code = (int)StorageErrorCode.InvalidCredentials, Message = "访问被拒绝，请检查配置！" }, webEx);
                    }
                }
                throw;
            }
        }
    }
}