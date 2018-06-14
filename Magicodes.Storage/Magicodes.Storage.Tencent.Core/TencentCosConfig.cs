using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    /// 腾讯云COS配置类
    /// </summary>
    public class TencentCosConfig
    {
        /// <summary>
        /// 应用ID。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 秘钥id
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        /// 秘钥Key
        /// </summary>
        public string SecretKey { get; set; }
    }
}
