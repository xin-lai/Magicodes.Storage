using Abp;
using Abp.Dependency;
using Magicodes.Storage.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Storage.Abp.Core
{
    public interface IStorageManager : ISingletonDependency, IShouldInitialize
    {
        /// <summary>
        /// 存储提供程序
        /// </summary>
        IStorageProvider StorageProvider { get; set; }
    }
}
