// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : StorageErrorCode.cs
//          description :
//  
//          created by 李文强 at  2016/09/23 9:41
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.Storage
{
    public enum StorageErrorCode
    {
        None = 0,
        InvalidCredentials = 1000,
        GenericException = 1001,
        InvalidAccess = 1002,
        BlobInUse = 1003,
        InvalidBlobName = 1004,
        InvalidContainerName = 1005,
        ErrorOpeningBlob = 1006,
        NoCredentialsProvided = 1007
    }
}