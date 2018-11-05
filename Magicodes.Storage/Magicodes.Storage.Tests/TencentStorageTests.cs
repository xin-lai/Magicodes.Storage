using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Magicodes.Storage.Tencent.Core;
using System.IO;
using System.Threading.Tasks;
using Magicodes.Storage.Core;
using Shouldly;


namespace Magicodes.Storage.Tests
{

    [Trait("Group", "腾讯云存储测试")]
    public class TencentStorageTests : TestBase, IDisposable
    {
        private string rootPath;
        TencentStorageProvider TencentStorage;
        public TencentStorageTests()
        {

            TencentCosConfig cosConfig = new TencentCosConfig()
            {
                //这里使用自己的腾讯云相关配置
                AppId = "1256819585",
                SecretId = "AKIDVoumQq0ziDVZP0qTKwJTQ7lCPO9hf7ya",
                SecretKey = "WnoZ4KK5appZ4JJciZDdomZ9VcVAbnlX",
            };

            Bucket bucket = new Bucket(cosConfig.AppId, "mtest-1256819585", "ap-chengdu");
            TencentStorage = new TencentStorageProvider(cosConfig, bucket);
            StorageProvider = TencentStorage;
        }

        [Fact(DisplayName = "腾讯云_删除容器")]
        public async Task DeleteContainer_Test()
        {
            await StorageProvider.DeleteContainer("SaveBlob");
        }
        [Fact(DisplayName = "腾讯云_删除对象")]
        public async Task DeleteBlob_Test()
        {
            await StorageProvider.SaveBlobStream("SaveBlob", "1.docx", TestStream);
            await StorageProvider.DeleteBlob("SaveBlob", "1.docx");
        }
        [Fact(DisplayName = "腾讯云_获取文件信息")]
        public async Task GetBlobFileInfo_Test()
        {
            await StorageProvider.GetBlobFileInfo("SaveBlob", "1.txt");
        }

        [Fact(DisplayName = "腾讯云_本地文件上传测试")]
        public async Task SaveBlobStream_Test()
        {
            await StorageProvider.SaveBlobStream("SaveBlob", "1.txt", TestStream);

        }
        [Fact(DisplayName = "腾讯云_列出指定容器下的对象列表")]
        public async Task ListBlobs_Test()
        {
            var result = await StorageProvider.ListBlobs("SaveBlob/");
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);

        }
        [Fact(DisplayName = "腾讯云_获取授权访问链接")]
        public async Task GetBlobUrl_Test()
        {
            var result = await StorageProvider.GetBlobUrl("SaveBlob", "1.txt", DateTime.Now);
            result.ShouldNotBeNull();
        }
        [Fact(DisplayName = "腾讯云_获取访问链接(两个参数)")]
        public async Task GetBlobUrl1_Test()
        {
            var result = await StorageProvider.GetBlobUrl("SaveBlob", "1.txt");
            result.ShouldNotBeNull();
        }

        [Fact(DisplayName = "获取文件的流信息")]
        public async Task GetBlobStream_Test()
        {
            await StorageProvider.GetBlobStream("SaveBlob", "1.txt");
        }

        public void Dispose()
        {

        }
    }
}
