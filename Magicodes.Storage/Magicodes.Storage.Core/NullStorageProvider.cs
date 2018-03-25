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
//          交流QQ群（.NET 技术交流群）：85318032
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Magicodes.Storage.Core
{
    /// <summary>
    /// 空存储提供程序实现，以增加程序的容错能力
    /// </summary>
    public class NullStorageProvider : IStorageProvider
    {
        public Task DeleteBlob(string containerName, string blobName) => Task.FromResult(0);
        public Task DeleteContainer(string containerName) => Task.FromResult(0);
        public Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName) => Task.FromResult(default(BlobFileInfo));
        public Task<Stream> GetBlobStream(string containerName, string blobName) => Task.FromResult(default(Stream));
        public Task<string> GetBlobUrl(string containerName, string blobName) => Task.FromResult(default(string));
        public Task<IList<BlobFileInfo>> ListBlobs(string containerName) => Task.FromResult(default(IList<BlobFileInfo>));
        public Task SaveBlobStream(string containerName, string blobName, Stream source) => Task.FromResult(0);
    }
}