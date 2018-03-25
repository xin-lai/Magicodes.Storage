using System;
using Magicodes.Storage.Local.Core;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Magicodes.Storage.Core;

namespace Magicodes.Storage.Tests
{
    public class LocalStorageTests : TestBase
    {
        private const string ContainerName = "Container";
        private string rootPath;
        private string rootUrl;

        public LocalStorageTests()
        {
            rootPath = Path.Combine(Directory.GetCurrentDirectory(), "test");
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            rootUrl = "/";
            StorageProvider = new LocalStorageProvider(rootPath, rootUrl);
        }

        [Fact(DisplayName = "本地文件删除测试")]
        public async Task DeleteBlob_Test()
        {
            var containerPath = Path.Combine(rootPath, ContainerName);
            if (!Directory.Exists(containerPath)) Directory.CreateDirectory(containerPath);

            File.WriteAllText(Path.Combine(containerPath, "1.txt"), "test");
            File.WriteAllText(Path.Combine(containerPath, "2.txt"), "test");
            File.WriteAllText(Path.Combine(containerPath, "3.txt"), "test");

            await StorageProvider.DeleteBlob(ContainerName, "1.txt");
            await StorageProvider.DeleteBlob(ContainerName, "2.txt");
            await StorageProvider.DeleteBlob(ContainerName, "3.txt");

            await Assert.ThrowsAnyAsync<StorageException>(async () =>
               await StorageProvider.DeleteBlob("AAAAAAAAAAAAA", "1.txt")
            );
        }
    }
}

