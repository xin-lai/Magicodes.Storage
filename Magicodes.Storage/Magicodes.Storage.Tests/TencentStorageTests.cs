using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Magicodes.Storage.Tencent.Core;
using System.IO;
using System.Threading.Tasks;
using Magicodes.Storage.Core;

namespace Magicodes.Storage.Tests
{
    [Trait("Group", "腾讯云存储测试")]
    public class TencentStorageTests : TestBase, IDisposable
    {
        public TencentStorageTests()
        {
            var cosConfig = new TencentCosConfig()
            {
                //这里使用自己的腾讯云相关配置
                AppId = "",
                SecretId = "",
                SecretKey = ""
            };
            var bucket = new Bucket(cosConfig.AppId, "t1", "ap-chengdu");
            var tencentStorage = new TencentStorageProvider(cosConfig, bucket);
            StorageProvider = tencentStorage;
        }

        [Fact(DisplayName = "腾讯云_目录删除测试")]
        public async Task DeleteContainer_Test()
        {
            await StorageProvider.DeleteContainer("t1");
        }

        [Fact(DisplayName = "腾讯云_获取对象属性信息")]
        public async Task GetBlobFileInfo_Test()
        {
            await StorageProvider.GetBlobFileInfo("", "JSON解析工具.rar");
        }

        [Fact(DisplayName = "腾讯云_本地文件上传测试")]
        public async Task SaveBlobStream_Test()
        {         
            await StorageProvider.SaveBlobStream("", "1.txt", TestStream);

        }

        public void Dispose()
        {

        }
    }
}
