// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : IStorageProvider.cs
//          description :
//  
//          created by 李文强 at  2016/09/23 9:41
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
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
        /// <param name="properties"></param>
        void SaveBlobStream(string containerName, string blobName, Stream source, BlobProperties properties = null);
        /// <summary>
        /// 保存对象到指定的容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="source"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        Task SaveBlobStreamAsync(string containerName, string blobName, Stream source, BlobProperties properties = null);
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Stream GetBlobStream(string containerName, string blobName);
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<Stream> GetBlobStreamAsync(string containerName, string blobName);
        /// <summary>
        /// 获取Url
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        string GetBlobUrl(string containerName, string blobName);
        /// <summary>
        /// 获取SAS Url
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="expiry"></param>
        /// <param name="isDownload"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        string GetBlobSasUrl(string containerName, string blobName, DateTimeOffset expiry, bool isDownload = false,
            string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read);
        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        BlobDescriptor GetBlobDescriptor(string containerName, string blobName);
        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task<BlobDescriptor> GetBlobDescriptorAsync(string containerName, string blobName);
        /// <summary>
        /// 列出指定容器下的对象列表
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        IList<BlobDescriptor> ListBlobs(string containerName);
        /// <summary>
        /// 列出指定容器下的对象列表
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        Task<IList<BlobDescriptor>> ListBlobsAsync(string containerName);
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        void DeleteBlob(string containerName, string blobName);
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        Task DeleteBlobAsync(string containerName, string blobName);
        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerName"></param>
        void DeleteContainer(string containerName);
        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        Task DeleteContainerAsync(string containerName);
        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="properties"></param>
        void UpdateBlobProperties(string containerName, string blobName, BlobProperties properties);
        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        Task UpdateBlobPropertiesAsync(string containerName, string blobName, BlobProperties properties);
    }
}