using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Magicodes.Storage.Core;
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
        /// <summary>
        /// 提供服务名称
        /// </summary>
        public string ProviderName => "TencentCOS";
        private readonly TencentCosService TencentCos;
        private readonly Bucket StorageBucket;

        /// <summary>
        /// 腾讯云存储对象提供构造函数
        /// </summary>
        /// <param name="cfg">配置信息</param>
        /// <param name="bucket">容器对象</param>
        /// <param name="backChannel"></param>
        public TencentStorageProvider(TencentCosConfig cfg, Bucket bucket)
        {
            TencentCos = new TencentCosService(cfg, bucket);
            StorageBucket = bucket;
        }


        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="containerName">容器(Bucket)的地址</param>
        /// <param name="blobName">文件名称</param>
        public async Task DeleteBlob(string containerName, string blobName)
        {
            await TencentCos.DeleteObjectAsync(StorageBucket, blobName);
        }

        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task DeleteContainer(string containerName)
        {
            //只有容器存在才执行删除
            if (await TencentCos.BucketExists())
                await TencentCos.DeleteBucketAsync();
        }
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName)
        {
            return await TencentCos.GetBlobFileInfo(blobName);
        }

        /// <summary>
        /// 获取文件的流信息
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<Stream> GetBlobStream(string containerName, string blobName)
        {
            return await TencentCos.GetObjectAsync(blobName);
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
        public Task<string> GetBlobUrl(string containerName, string blobName)
        {
            return Task.FromResult(StorageBucket.Url + $"/{blobName}");
        }

        public Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry, bool isDownload = false, string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read)
        {
            return Task.FromResult(StorageBucket.Url + $"/{blobName}");
        }

        /// <summary>
        /// 列出指定容器下的对象列表
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public Task<IList<BlobFileInfo>> ListBlobs(string containerName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存对象到指定的容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="source"></param>
        public async Task SaveBlobStream(string containerName, string blobName, Stream source)
        {
            await TencentCos.PutObjectAsync(blobName, source);
        }
    }
}
