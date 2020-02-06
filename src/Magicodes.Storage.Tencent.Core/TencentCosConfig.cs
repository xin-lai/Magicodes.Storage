// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : TencentCosConfig.cs
//           description :
//   
//           created by 雪雁 at  2018-08-02 9:59
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    ///     腾讯云COS配置类
    /// </summary>
    public class TencentCosConfig
    {
        /// <summary>
        ///     应用ID。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        ///     秘钥id
        /// </summary>
        public string SecretId { get; set; }

        /// <summary>
        ///     秘钥Key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        ///     区域
        /// </summary>
        public string Region { get; set; } = "ap-guangzhou";

        /// <summary>
        ///    存储桶名称
        /// </summary>
        public string BucketName { get; set; }
    }
}