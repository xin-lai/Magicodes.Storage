using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Storage.Core;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace Magicodes.Storage.QiNiu.Core
{
    /// <summary>
    /// 七牛云存储提供程序
    /// </summary>
    public class QiNiuStorageProvider : IStorageProvider
    {
        private readonly string accessKey;
        private readonly string secretKey;
        private readonly Config config;
        private Mac mac;
        private BucketManager bucketManager;

        #region 私有方法
        private void HandlingResult(int code)
        {
            var httpCode = (HttpCode)code;
            switch (httpCode)
            {
                case HttpCode.OK:
                    break;
                case HttpCode.PARTLY_OK:
                    throw new StorageException(StorageErrorCode.PartlyOK.ToStorageError(), null);
                case HttpCode.BAD_REQUEST:
                    throw new StorageException(StorageErrorCode.GenericException.ToStorageError(), null);
                case HttpCode.AUTHENTICATION_FAILED:
                    throw new StorageException(StorageErrorCode.InvalidAccess.ToStorageError(), null);
                case HttpCode.ACCESS_DENIED:
                    throw new StorageException(StorageErrorCode.InvalidAccess.ToStorageError(), null);
                case HttpCode.OBJECT_NOT_FOUND:
                    throw new StorageException(StorageErrorCode.NotFound.ToStorageError(), null);
                case HttpCode.CRC32_CHECK_FAILEd:
                    throw new StorageException(StorageErrorCode.PostError.ToStorageError(), null);
                case HttpCode.FILE_SIZE_EXCEED:
                    throw new StorageException(StorageErrorCode.SizeError.ToStorageError(), null);
                case HttpCode.PREFETCH_FAILED:
                    throw new StorageException(StorageErrorCode.GenericException.ToStorageError(), null);
                case HttpCode.BAD_GATEWAY:
                    throw new StorageException(StorageErrorCode.NetworkError.ToStorageError(), null);
                case HttpCode.SERVER_UNAVAILABLE:
                    throw new StorageException(StorageErrorCode.GenericException.ToStorageError(), null);
                case HttpCode.SERVER_TIME_EXCEED:
                    throw new StorageException(StorageErrorCode.TimeoutError.ToStorageError(), null);
                case HttpCode.TOO_FREQUENT_ACCESS:
                    throw new StorageException(StorageErrorCode.AccessLimitError.ToStorageError(), null);
                case HttpCode.CALLBACK_FAILED:
                    throw new StorageException(StorageErrorCode.GenericException.ToStorageError(), null);
                case HttpCode.SERVER_OPERATION_FAILED:
                    throw new StorageException(StorageErrorCode.GenericException.ToStorageError(), null);
                case HttpCode.CONTENT_MODIFIED:
                    throw new StorageException(StorageErrorCode.PostError.ToStorageError(), null);
                case HttpCode.FILE_NOT_EXIST:
                    throw new StorageException(StorageErrorCode.FileNotFound.ToStorageError(), null);
                case HttpCode.FILE_EXISTS:
                    throw new StorageException(StorageErrorCode.ExistError.ToStorageError(), null);
                case HttpCode.BUCKET_COUNT_LIMIT:
                    throw new StorageException(StorageErrorCode.CountLimitError.ToStorageError(), null);
                case HttpCode.BUCKET_NOT_EXIST:
                    throw new StorageException(StorageErrorCode.ContainerNotFound.ToStorageError(), null);
                case HttpCode.INVALID_MARKER:
                    throw new StorageException(StorageErrorCode.PostError.ToStorageError(), null);
                case HttpCode.CONTEXT_EXPIRED:
                    throw new StorageException(StorageErrorCode.PostError.ToStorageError(), null);
                case HttpCode.INVALID_ARGUMENT:
                    throw new StorageException(StorageErrorCode.PostError.ToStorageError(), null);
                case HttpCode.INVALID_FILE:
                    throw new StorageException(StorageErrorCode.PostError.ToStorageError(), null);
                case HttpCode.INVALID_TOKEN:
                    throw new StorageException(StorageErrorCode.InvalidAccess.ToStorageError(), null);
                default:
                    throw new StorageException(StorageErrorCode.GenericException.ToStorageError(), null);
            }
        }
        #endregion

        public string ProviderName => "QiNiu";

        public BucketManager BucketManager { get => bucketManager; set => bucketManager = value; }
        public Mac Mac { get => mac; set => mac = value; }

        public Config Config => config;

        /// <summary>
        /// 初始化七牛云存储提供程序
        /// </summary>
        /// <param name="accessKey"></param>
        /// <param name="secretKey"></param>
        public QiNiuStorageProvider(string accessKey, string secretKey, Config config = null)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
            this.config = config;
            if (this.config == null)
            {
                this.config = new Config
                {
                    UseHttps = true,
                    UseCdnDomains = true
                };
            }
            Mac = new Mac(accessKey, secretKey);
            BucketManager = new BucketManager(Mac, config);
        }

        public Task DeleteBlob(string containerName, string blobName)
        {
            var result = BucketManager.Delete(containerName, blobName);
            HandlingResult(result.Code);
        }
        public Task DeleteContainer(string containerName) => throw new NotImplementedException();
        public Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName) => throw new NotImplementedException();
        public Task<Stream> GetBlobStream(string containerName, string blobName) => throw new NotImplementedException();
        public Task<string> GetBlobUrl(string containerName, string blobName) => throw new NotImplementedException();
        public Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry, bool isDownload = false, string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read) => throw new NotImplementedException();
        public Task<IList<BlobFileInfo>> ListBlobs(string containerName) => throw new NotImplementedException();
        public Task SaveBlobStream(string containerName, string blobName, Stream source) => throw new NotImplementedException();
    }
}
