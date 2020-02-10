using Abp.Configuration;
using Abp.Dependency;
using Abp.Json;
using Castle.Core.Logging;
using Magicodes.Storage.AliyunOss.Core;
using Magicodes.Storage.Core;
using Magicodes.Storage.Local.Core;
using Magicodes.Storage.Tencent.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Storage.Abp.Core
{
    public class StorageManager : IStorageManager
    {
        public ILogger Logger { get; set; }

        public StorageManager(IConfiguration appConfiguration, IIocManager iocManager)
        {
            Logger = NullLogger.Instance;
            AppConfiguration = appConfiguration;
            IocManager = iocManager;
        }

        //public IStorageProvider LocalStorageProvider { get; set; }

        public IStorageProvider StorageProvider { get; set; }

        public IConfiguration AppConfiguration { get; set; }

        public IIocManager IocManager { get; set; }

        /// <summary>
        /// 根据key从站点配置文件或设置中获取支付配置
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <returns></returns>
        private Task<TConfig> GetConfigFromConfigOrSettingsByKey<TConfig>(string key) where TConfig : class, new()
        {
            var settings = AppConfiguration?.GetSection(key: key)?.Get<TConfig>();
            if (settings != null) return Task.FromResult(settings);

            using (var obj = IocManager.ResolveAsDisposable<ISettingManager>())
            {
                var value = obj.Object.GetSettingValue(key);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return Task.FromResult<TConfig>(null);
                }
                settings = value.FromJsonString<TConfig>();
                return Task.FromResult(settings);
            }
        }

        public void Initialize()
        {
            //日志函数
            void LogAction(string tag, string message)
            {
                if (tag.Equals("error", StringComparison.CurrentCultureIgnoreCase))
                    Logger.Error(message);
                else
                    Logger.Debug(message);
            }

            #region 配置存储程序
            switch (AppConfiguration["StorageProvider:Type"])
            {
                case "LocalStorageProvider":
                    {
                        var config = GetConfigFromConfigOrSettingsByKey<LocalStorageConfig>("LocalStorageProvider").Result;

                        if (config != null)
                        {
                            if (!config.RootPath.Contains(":"))
                            {
                                var hostingEnvironment = IocManager.Resolve<IWebHostEnvironment>();
                                config.RootPath = Path.Combine(hostingEnvironment.WebRootPath, config.RootPath);
                            }
                        }
                        if (!Directory.Exists(config.RootPath)) Directory.CreateDirectory(config.RootPath);

                        StorageProvider = new LocalStorageProvider(config);
                        break;
                    }
                case "AliyunOssStorageProvider":
                    {
                        var aliyunOssConfig = GetConfigFromConfigOrSettingsByKey<AliyunOssConfig>("AliyunOssStorageProvider").Result; ;
                        StorageProvider = new AliyunOssStorageProvider(aliyunOssConfig);
                        break;
                    }
                case "TencentCosStorageProvider":
                    {
                        var config = GetConfigFromConfigOrSettingsByKey<TencentCosConfig>("TencentCosStorageProvider").Result; ;
                        StorageProvider = new TencentStorageProvider(config);
                        break;
                    }
                default:
                    break;
            }
            #endregion
        }
    }
}
