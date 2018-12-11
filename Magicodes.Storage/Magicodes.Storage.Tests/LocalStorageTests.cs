// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : LocalStorageTests.cs
//           description :
//   
//           created by 雪雁 at  2018-06-07 10:31
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Magicodes.Storage.Core;
using Magicodes.Storage.Local.Core;
using Shouldly;
using Xunit;

namespace Magicodes.Storage.Tests
{
    [Trait("Group", "本地存储测试")]
    public class LocalStorageTests : TestBase, IDisposable
    {
        public LocalStorageTests()
        {
            rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            rootUrl = "/";
            StorageProvider = new LocalStorageProvider(rootPath, rootUrl);
        }

        public void Dispose()
        {
            //TODO：数据清理
        }

        private readonly string rootPath;
        private readonly string rootUrl;

        [Fact(DisplayName = "本地文件删除测试")]
        public async Task DeleteBlob_Test()
        {
            var containerPath = Path.Combine(rootPath, ContainerName);
            if (!Directory.Exists(containerPath)) Directory.CreateDirectory(containerPath);

            File.WriteAllText(Path.Combine(containerPath, "1.txt"), GetTestContent());
            File.WriteAllText(Path.Combine(containerPath, "2.txt"), GetTestContent());
            File.WriteAllText(Path.Combine(containerPath, "3.txt"), GetTestContent());

            await StorageProvider.DeleteBlob(ContainerName, "1.txt");
            await StorageProvider.DeleteBlob(ContainerName, "2.txt");
            await StorageProvider.DeleteBlob(ContainerName, "3.txt");

            await Assert.ThrowsAnyAsync<StorageException>(async () =>
                await StorageProvider.DeleteBlob("AAAAAAAAAAAAA", "notfound.txt")
            );
        }

        [Fact(DisplayName = "本地目录删除测试")]
        public async Task DeleteContainer_Test()
        {
            var containerName = GetTestContainerName();
            var containerPath = Path.Combine(rootPath, containerName);
            if (!Directory.Exists(containerPath)) Directory.CreateDirectory(containerPath);

            await StorageProvider.DeleteContainer(containerName);

            await Assert.ThrowsAnyAsync<StorageException>(async () =>
                await StorageProvider.DeleteContainer("AAAAAAAAAAAAA")
            );
        }


        [Fact(DisplayName = "本地文件详情获取测试")]
        public async Task GetBlobFileInfo_Test()
        {
            var containerPath = Path.Combine(rootPath, ContainerName);
            if (!Directory.Exists(containerPath)) Directory.CreateDirectory(containerPath);

            File.WriteAllText(Path.Combine(containerPath, "1.txt"), GetTestContent());

            var result = await StorageProvider.GetBlobFileInfo(ContainerName, "1.txt");

            result.ShouldNotBeNull();
            result.Name.ShouldBe("1.txt");
            result.Container.ShouldBe(ContainerName);
            result.Length.ShouldBeGreaterThan(0);
            result.ContentType.ShouldNotBeNullOrWhiteSpace();

            await Assert.ThrowsAnyAsync<StorageException>(async () =>
                await StorageProvider.GetBlobFileInfo(ContainerName, "notfound.txt")
            );
        }

        [Fact(DisplayName = "本地签名链接获取测试")]
        public async Task GetBlobSasUrl_Test()
        {
            var containerPath = Path.Combine(rootPath, ContainerName);
            if (!Directory.Exists(containerPath)) Directory.CreateDirectory(containerPath);

            await Assert.ThrowsAnyAsync<NotSupportedException>(async () =>
                await StorageProvider.GetBlobUrl(ContainerName, "notfound.txt", DateTime.Now.AddDays(1))
            );
        }

        [Fact(DisplayName = "本地文件流获取测试")]
        public async Task GetBlobStream_Test()
        {
            var containerPath = Path.Combine(rootPath, ContainerName);
            if (!Directory.Exists(containerPath)) Directory.CreateDirectory(containerPath);

            File.WriteAllText(Path.Combine(containerPath, "1.txt"), GetTestContent());

            var result = await StorageProvider.GetBlobStream(ContainerName, "1.txt");

            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);

            await Assert.ThrowsAnyAsync<StorageException>(async () =>
                await StorageProvider.GetBlobStream(ContainerName, "notfound.txt")
            );
        }

        [Fact(DisplayName = "本地文件Url获取测试")]
        public async Task GetBlobUrl_Test()
        {
            var containerPath = Path.Combine(rootPath, ContainerName);
            if (!Directory.Exists(containerPath)) Directory.CreateDirectory(containerPath);

            var testName = GetTestFileName();
            File.WriteAllText(Path.Combine(containerPath, testName), GetTestContent());
            Thread.Sleep(10);

            var result = await StorageProvider.GetBlobUrl(ContainerName, testName);

            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
            result.ShouldContain("/" + ContainerName + "/");

            await Assert.ThrowsAnyAsync<StorageException>(async () =>
                await StorageProvider.GetBlobUrl(ContainerName, "notfound.txt")
            );
        }

        [Fact(DisplayName = "本地文件列表获取测试")]
        public async Task ListBlobs_Test()
        {
            var containerPath = Path.Combine(rootPath, ContainerName);
            Directory.CreateDirectory(containerPath);

            File.WriteAllText(Path.Combine(containerPath, GetTestFileName()), GetTestContent());
            File.WriteAllText(Path.Combine(containerPath, GetTestFileName()), GetTestContent());
            File.WriteAllText(Path.Combine(containerPath, GetTestFileName()), GetTestContent());
            Thread.Sleep(10);

            var result = await StorageProvider.ListBlobs(ContainerName);

            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
        }

        [Fact(DisplayName = "本地文件上传测试")]
        public async Task SaveBlobStream_Test()
        {
            var containerPath = Path.Combine(rootPath, ContainerName);
            if (Directory.Exists(containerPath)) Directory.Delete(containerPath, true);
            Directory.CreateDirectory(containerPath);

            await StorageProvider.SaveBlobStream(ContainerName, "1.txt", TestStream);

            File.Exists(Path.Combine(containerPath, "1.txt")).ShouldBe(true);
        }
    }
}