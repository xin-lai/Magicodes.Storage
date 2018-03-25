// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : IStorageProvider.cs
//          description :
//  
//          created by 李文强 at  2018/03/25 9:41
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
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
    /// 程序提供程序
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// 保存对象到指定的容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="source"></param>
        Task SaveBlobStream(string containerName, string blobName, Stream source);

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<Stream> GetBlobStream(string containerName, string blobName);

        /// <summary>
        /// 获取Url
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<string> GetBlobUrl(string containerName, string blobName);

        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName);
              
        /// <summary>
        /// 列出指定容器下的对象列表
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        Task<IList<BlobFileInfo>> ListBlobs(string containerName);

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        Task DeleteBlob(string containerName, string blobName);

        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerName"></param>
        Task DeleteContainer(string containerName);
    }
}