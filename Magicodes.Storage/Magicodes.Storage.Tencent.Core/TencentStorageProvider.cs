// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : TencentStorageProvider.cs
//           description :
//   
//           created by 雪雁 at  2018-08-02 9:59
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Magicodes.Storage.Core;

namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    ///     腾讯云存储提供服务
    /// </summary>
    public class TencentStorageProvider : IStorageProvider
    {
        private readonly Bucket _storageBucket;
        private readonly TencentCosService _tencentCos;

        /// <summary>
        ///     腾讯云存储对象提供构造函数
        /// </summary>
        /// <param name="cfg">配置信息</param>
        /// <param name="bucket">容器对象</param>
        public TencentStorageProvider(TencentCosConfig cfg, Bucket bucket)
        {
            _tencentCos = new TencentCosService(cfg, bucket);
            _storageBucket = bucket;
        }

        /// <summary>
        ///     提供服务名称
        /// </summary>
        public string ProviderName => "TencentCOS";


        /// <summary>
        ///     删除对象
        /// </summary>
        /// <param name="containerName">容器(Bucket)的地址</param>
        /// <param name="blobName">文件名称</param>
        public async Task DeleteBlob(string containerName, string blobName)
        {
            await _tencentCos.DeleteObjectAsync(_storageBucket, blobName);
        }

        /// <summary>
        ///     删除容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task DeleteContainer(string containerName)
        {
            //只有容器存在才执行删除
            if (await _tencentCos.BucketExists())
                await _tencentCos.DeleteBucketAsync();
        }

        /// <summary>
        ///     获取文件信息
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName)
        {
            return await _tencentCos.GetBlobFileInfo(blobName);
        }

        /// <summary>
        ///     获取文件的流信息
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<Stream> GetBlobStream(string containerName, string blobName)
        {
            return await _tencentCos.GetObjectAsync(blobName);
        }

        /// <summary>
        ///     获取授权访问链接
        /// </summary>
        /// <param name="containerName">容器名称</param>
        /// <param name="blobName">文件名称</param>
        /// <returns></returns>
        public Task<string> GetBlobUrl(string containerName, string blobName)
        {
            return Task.FromResult(_storageBucket.Url + $"/{blobName}");
        }

        public Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry, bool isDownload = false,
            string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read)
        {
            return Task.FromResult(_storageBucket.Url + $"/{blobName}");
        }

        /// <summary>
        ///     列出指定容器下的对象列表
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public Task<IList<BlobFileInfo>> ListBlobs(string containerName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     保存对象到指定的容器
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="source"></param>
        public async Task SaveBlobStream(string containerName, string blobName, Stream source)
        {
            await _tencentCos.PutObjectAsync(blobName, source);
        }
    }
}