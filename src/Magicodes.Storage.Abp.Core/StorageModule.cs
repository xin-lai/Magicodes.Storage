using Abp.Modules;
using Abp.Reflection.Extensions;
using System;

namespace Magicodes.Storage.Abp.Core
{
    public class StorageModule : AbpModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StorageModule).GetAssembly());
        }

        public override void PostInitialize()
        {
        }
    }
}
