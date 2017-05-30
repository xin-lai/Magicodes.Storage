// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : NullStorageProvider.cs
//          description :
//  
//          created by 李文强 at  2016/10/04 20:35
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub：https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Magicodes.Storage
{
    /// <summary>
    /// 空存储提供程序实现，已增加程序的容错能力
    /// </summary>
    public class NullStorageProvider : IStorageProvider
    {
        public void SaveBlobStream(string containerName, string blobName, Stream source,
            BlobProperties properties = null)
        {
        }

        public Task SaveBlobStreamAsync(string containerName, string blobName, Stream source,
            BlobProperties properties = null)
        {
            return Task.FromResult(0);
        }

        public Stream GetBlobStream(string containerName, string blobName)
        {
            return null;
        }

        public Task<Stream> GetBlobStreamAsync(string containerName, string blobName)
        {
            return Task.FromResult((Stream) null);
        }

        public string GetBlobUrl(string containerName, string blobName)
        {
            return null;
        }

        public string GetBlobSasUrl(string containerName, string blobName, DateTimeOffset expiry,
            bool isDownload = false,
            string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read)
        {
            return null;
        }

        public BlobDescriptor GetBlobDescriptor(string containerName, string blobName)
        {
            return null;
        }

        public Task<BlobDescriptor> GetBlobDescriptorAsync(string containerName, string blobName)
        {
            return Task.FromResult((BlobDescriptor) null);
        }

        public IList<BlobDescriptor> ListBlobs(string containerName)
        {
            return null;
        }

        public Task<IList<BlobDescriptor>> ListBlobsAsync(string containerName)
        {
            return Task.FromResult((IList<BlobDescriptor>) null);
        }

        public void DeleteBlob(string containerName, string blobName)
        {
        }

        public Task DeleteBlobAsync(string containerName, string blobName)
        {
            return Task.FromResult(0);
        }

        public void DeleteContainer(string containerName)
        {
        }

        public Task DeleteContainerAsync(string containerName)
        {
            return Task.FromResult(0);
        }

        public void UpdateBlobProperties(string containerName, string blobName, BlobProperties properties)
        {
        }

        public Task UpdateBlobPropertiesAsync(string containerName, string blobName, BlobProperties properties)
        {
            return Task.FromResult(0);
        }
    }
}