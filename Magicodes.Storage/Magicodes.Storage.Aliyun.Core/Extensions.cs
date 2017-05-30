using System;
using Aliyun.OSS.Common;

namespace Magicodes.Storage.Aliyun.Core
{
    public static class Extensions
    {
        //public static SharedAccessBlobPermissions ToPermissions(this BlobUrlAccess security)
        //{
        //    switch (security)
        //    {
        //        case BlobUrlAccess.Read:
        //            return SharedAccessBlobPermissions.Read;
        //        case BlobUrlAccess.Write:
        //            return SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Write;
        //        case BlobUrlAccess.Delete:
        //            return SharedAccessBlobPermissions.Delete;
        //        case BlobUrlAccess.All:
        //            return SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Delete | SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;
        //        default:
        //            return SharedAccessBlobPermissions.None;
        //    }
        //}

        public static Exception Convert(this Exception e)
        {
            var storageException = (e as OssException) ?? e.InnerException as OssException;

            if (storageException != null)
            {
                StorageErrorCode errorCode;
                switch (storageException.ErrorCode)
                {
                    case "AccessDenied":
                        errorCode = StorageErrorCode.InvalidAccess;
                        break;
                    case "InvalidAccessKeyId":
                    case "SignatureDoesNotMatch":
                        errorCode = StorageErrorCode.InvalidCredentials;
                        break;
                    case "BucketNotEmpty":
                        errorCode = StorageErrorCode.BlobInUse;
                        break;
                    case "BucketAlreadyExists":
                    case "InvalidBucketName":
                    case "NoSuchBucket":
                        errorCode = StorageErrorCode.InvalidContainerName;
                        break;
                    case "InvalidObjectName":
                    case "NoSuchKey":
                        errorCode = StorageErrorCode.InvalidBlobName;
                        break;
                    case "TooManyBuckets":
                        errorCode = StorageErrorCode.ErrorOpeningBlob;
                        break;
                    default:
                        errorCode = StorageErrorCode.GenericException;
                        break;
                }
                //ErrorCode： OSS返回给用户的错误码。
                //Message： OSS给出的详细错误信息。
                //RequestId： 用于唯一标识该次请求的UUID；当您无法解决问题时，可以凭这个RequestId来请求OSS开发工程师的帮助。
                //HostId： 用于标识访问的OSS集群（目前统一为oss.aliyuncs.com）
                //storageException.ErrorCode
                //storageException.Message

                return new StorageException(errorCode.ToStorageError(), storageException);
            }
            return e;
        }

        public static bool IsOssStorageException(this Exception e)
        {
            return e is OssException || e.InnerException is OssException;
        }
    }
}