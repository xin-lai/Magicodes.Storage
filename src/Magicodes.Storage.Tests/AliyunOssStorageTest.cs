// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : AliyunOssStorageTest.cs
//           description :
//   
//           created by 雪雁 at  2018-09-04 13:55
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using Magicodes.Storage.AliyunOss.Core;
using Magicodes.Storage.Tests.Helper;
using Shouldly;
using Xunit;

namespace Magicodes.Storage.Tests
{
    [Trait("Group", "阿里云存储测试")]
    public class AliyunOssStorageTest : TestBase, IDisposable
    {
        public AliyunOssStorageTest()
        {
            var config = new AliyunOssConfig
            {
                //这里使用自己的相关配置完成测试
                AccessKeyId = "",
                AccessKeySecret = "",
                Endpoint = ""
            };
            //如果没填，尝试从配置文件加载
            if (string.IsNullOrWhiteSpace(config.AccessKeyId))
            {
                config = ConfigHelper.LoadConfig<AliyunOssConfig>("AliyunOssStorage");
            }
            var storage = new AliyunOssStorageProvider(config);
            StorageProvider = new AliyunOssStorageProvider(config);
        }
        public void Dispose()
        {
        }

        [Fact(DisplayName = "阿里云_删除对象")]
        public async Task DeleteBlob_Test()
        {
            var fileName = await CreateTestFile();
            await StorageProvider.DeleteBlob(ContainerName, fileName);

        }

        private async Task<string> CreateTestFile()
        {
            var fileName = GetTestFileName();
            await StorageProvider.SaveBlobStream(ContainerName, fileName, TestStream);
            return fileName;
        }

        [Fact(DisplayName = "阿里云_删除容器")]
        public async Task DeleteContainer_Test()
        {
            var fileName = GetTestFileName();
            await StorageProvider.DeleteContainer(ContainerName);
        }

        [Fact(DisplayName = "阿里云_获取文件信息")]
        public async Task GetBlobFileInfo_Test()
        {
            var fileName = await CreateTestFile();
            var result = await StorageProvider.GetBlobFileInfo(ContainerName, fileName);
            result.Name.ShouldBe(fileName);
            result.Length.ShouldBeGreaterThan(0);
            result.Url.ShouldNotBeNullOrWhiteSpace();
            result.ETag.ShouldNotBeNull();
            result.ContentType.ShouldNotBeNull();
        }

        [Fact(DisplayName = "获取文件的流信息")]
        public async Task GetBlobStream_Test()
        {
            var fileName = await CreateTestFile();
            var result = await StorageProvider.GetBlobStream(ContainerName, fileName);
            result.ShouldNotBeNull();

        }

        [Fact(DisplayName = "阿里云_获取授权访问链接")]
        public async Task GetBlobUrl_Test()
        {
            var fileName = await CreateTestFile();
            var result = await StorageProvider.GetBlobUrl(ContainerName, fileName, DateTime.Now);
            result.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "阿里云_获取访问链接(两个参数)")]
        public async Task GetBlobUrl1_Test()
        {
            var fileName = await CreateTestFile();
            var result = await StorageProvider.GetBlobUrl(ContainerName, fileName);
            result.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "阿里云_列出指定容器下的对象列表")]
        public async Task ListBlobs_Test()
        {
            var fileName = await CreateTestFile();
            var result = await StorageProvider.ListBlobs(ContainerName);
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
        }

        [Fact(DisplayName = "阿里云_本地文件上传测试")]
        public async Task SaveBlobStream_Test()
        {
            var testFileName = GetTestFileName();
            await StorageProvider.SaveBlobStream(ContainerName, testFileName, TestStream);
            var result = await StorageProvider.GetBlobFileInfo(ContainerName, testFileName);
            result.ShouldNotBeNull();
            result.Name.ShouldNotBeNullOrWhiteSpace();
            result.Name.ShouldBe(testFileName);
        }
    }
}