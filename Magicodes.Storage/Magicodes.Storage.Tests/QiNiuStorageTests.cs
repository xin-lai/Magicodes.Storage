// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : QiNiuStorageTests.cs
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
using System.Threading.Tasks;
using Magicodes.Storage.Core;
using Magicodes.Storage.QiNiu.Core;
using Qiniu.Storage;
using Qiniu.Util;
using Shouldly;
using Xunit;

namespace Magicodes.Storage.Tests
{
    [Trait("Group", "七牛云存储测试")]
    public class QiNiuStorageTests : TestBase, IDisposable
    {
        private readonly string localPath;
        private readonly QiNiuStorageProvider qiNiuStorageProvider;

        public QiNiuStorageTests()
        {
            localPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            if (!Directory.Exists(localPath)) Directory.CreateDirectory(localPath);

            //七牛云注册即有免费空间,请替换为自己的
            qiNiuStorageProvider = new QiNiuStorageProvider("33Ag3A3lSRrZ3VHyaCravN61g5DWhz6C24I5Zp_r",
                "qbepDnLi6Y4b4DH9XchxP-qTKddZeA5iLyZitE-U");

            StorageProvider = qiNiuStorageProvider;
        }

        public void Dispose()
        {
            //TODO:清理数据
        }

        [Theory(DisplayName = "文件删除测试")]
        [InlineData("container1", "blob1.txt")]
        public async Task DeleteBlob_Test(string containerName, string blobName)
        {
            var bucketsResult = qiNiuStorageProvider.BucketManager.Buckets(true);

            //由于没有提供创建空间的API，请去管理后台手动创建
            bucketsResult.Result.Contains(containerName).ShouldBe(true);

            await Assert.ThrowsAnyAsync<StorageException>(async () =>
                await StorageProvider.DeleteBlob(containerName, blobName)
            );

            var putPolicy = new PutPolicy
            {
                Scope = containerName
            };
            putPolicy.SetExpires(120);
            putPolicy.DeleteAfterDays = 1;

            var token = Auth.CreateUploadToken(qiNiuStorageProvider.Mac, putPolicy.ToJsonString());
            var um = new UploadManager(qiNiuStorageProvider.Config);

            var testFilePath = Path.Combine(localPath, blobName);
            File.WriteAllText(testFilePath, GetTestContent());
            var result = um.UploadFile(testFilePath, blobName, token, new PutExtra());
            if (result.Code == 200) await StorageProvider.DeleteBlob(containerName, blobName);
        }
    }
}