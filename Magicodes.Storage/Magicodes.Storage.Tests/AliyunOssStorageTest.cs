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
using System.Threading.Tasks;
using Magicodes.Storage.AliyunOss.Core;
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
                AccessKeyId = "",
                AccessKeySecret = "",
                Endpoint = ""
            };
            StorageProvider = new AliyunOssStorageProvider(config);
        }

        public void Dispose()
        {
        }

        [Theory(DisplayName = "阿里云文件上传")]
        [InlineData("dzsshow", "aliyunblob1.txt")]
        public async Task SaveBlobStream_Test(string containerName, string blobName)
        {
            await StorageProvider.SaveBlobStream(containerName, blobName, TestStream);
        }

        [Theory(DisplayName = "阿里云文件获取")]
        [InlineData("dzsshow", "aliyunblob1.txt")]
        public async Task GetBlobStream_Test(string containerName, string blobName)
        {
            await StorageProvider.GetBlobStream(containerName, blobName);
        }

        [Theory(DisplayName = "阿里云获取文件路径")]
        [InlineData("dzsshow", "aliyunblob1.txt")]
        public async Task GetBlobUrl_Test(string containerName, string blobName)
        {
            await StorageProvider.GetBlobUrl(containerName, blobName);
        }

        [Theory(DisplayName = "阿里云获取文件")]
        [InlineData("dzsshow", "aliyunblob1.txt")]
        public async Task GetBlobFileInfo_Test(string containerName, string blobName)
        {
            await StorageProvider.GetBlobFileInfo(containerName, blobName);
        }

        [Theory(DisplayName = "阿里云获取文件列表")]
        [InlineData("dzsshow")]
        public async Task ListBlobs_Test(string containerName)
        {
            await StorageProvider.ListBlobs(containerName);
        }

        [Theory(DisplayName = "阿里云删除文件")]
        [InlineData("dzsshow", "aliyunblob1.txt")]
        public async Task DeleteBlob_Test(string containerName, string blobName)
        {
            await StorageProvider.DeleteBlob(containerName, blobName);
        }

        [Theory(DisplayName = "阿里云删除容器")]
        [InlineData("dzsshow")]
        public async Task DeleteContainer_Test(string containerName)
        {
            await StorageProvider.DeleteContainer(containerName);
        }

        [Theory(DisplayName = "阿里云获取文件路径")]
        [InlineData("dzsshow", "aliyunblob1.txt")]
        public async Task GetBlobUrlAccess_Test(string containerName, string blobName)
        {
            var expiry = DateTime.Now.AddHours(1);
            await StorageProvider.GetBlobUrl(containerName, blobName, expiry);
        }
    }
}