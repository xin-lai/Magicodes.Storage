// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : TestBase.cs
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
using System.Text;
using Magicodes.Storage.Core;

namespace Magicodes.Storage.Tests
{
    public class TestBase
    {
        public TestBase()
        {
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "demo.txt");
            //File.WriteAllText(path, "demo");
            var str = "demo";
            var array = Encoding.UTF8.GetBytes(str);
            TestStream = new MemoryStream(array);
            ContainerName = "magicodes";
        }

        protected IStorageProvider StorageProvider { get; set; }

        public Stream TestStream { get; set; }

        protected string ContainerName { get; set; }

        protected string GetTestFileName()
        {
            return Guid.NewGuid().ToString("N") + ".txt";
        }

        protected string GetTestContent()
        {
            return "Test";
        }

        protected string GetTestContainerName()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}