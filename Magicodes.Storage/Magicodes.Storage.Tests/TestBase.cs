using System;
using System.Collections.Generic;
using System.Text;
using Magicodes.Storage.Core;
using System.IO;

namespace Magicodes.Storage.Tests
{
    public class TestBase
    {
        protected IStorageProvider StorageProvider { get; set; }

        public Stream TestStream { get; set; }

        protected string ContainerName { get; set; }

        public TestBase()
        {
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "demo.txt");
            //File.WriteAllText(path, "demo");
            var str = "demo";                        
            var array = Encoding.UTF8.GetBytes(str);
            TestStream = new MemoryStream(array);
            ContainerName = GetTestContainerName();
        }

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
