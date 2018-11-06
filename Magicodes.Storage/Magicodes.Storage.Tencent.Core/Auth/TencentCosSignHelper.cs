// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : TencentCosSignHelper.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Magicodes.Storage.Core.Helper;

namespace Magicodes.Storage.Tencent.Core.Auth
{
    /// <summary>
    ///     腾讯云Cos签名帮助类
    /// </summary>
    public class TencentCosSignHelper
    {
        public TencentCosConfig Config;

        public TencentCosSignHelper(TencentCosConfig cfg) => Config = cfg;

        /// <summary>
        ///     生成请求签名
        /// </summary>
        /// <param name="req">http请求对象</param>
        /// <returns></returns>
        public string SignRequest(HttpRequestMessage req)
        {
            var qs = HttpUtility.ParseQueryString(req.RequestUri.Query);
            var sortedQuerys = qs.Cast<string>()
                .Select(k => new KeyValuePair<string, string>(k.ToLower(), qs[k].ToLower()))
                .OrderBy(k => k.Key);
            var sortedHeaders = req.Headers.Select(k =>
                    new KeyValuePair<string, string>(k.Key.ToLower(), Uri.EscapeDataString(k.Value.First()).ToLower()))
                .OrderBy(k => k.Key);
            var reqPayload = $"{req.Method.ToString().ToLower()}\n" +
                             $"{req.RequestUri.LocalPath}\n" +
                             $"{string.Join("&", sortedQuerys.Select(k => k.Key + "=" + k.Value))}\n" +
                             $"{string.Join("&", sortedHeaders.Select(k => k.Key + "=" + k.Value))}\n";
            // Sign
            var now = DateTimeOffset.Now;
            var signTime = $"{now.ToUnixTimeSeconds()};{now.AddSeconds(1000).ToUnixTimeSeconds()}";
            var signKey = EncryptHelper.HashHMACSHA1(Config.SecretKey, signTime);
            var payloadStr = $"sha1\n{signTime}\n{EncryptHelper.HashSHA1(reqPayload)}\n";
            var signature = EncryptHelper.HashHMACSHA1(signKey, payloadStr);
            var m = new Dictionary<string, string>
            {
                {"q-sign-algorithm", "sha1"},
                {"q-ak", Config.SecretId},
                {"q-sign-time", signTime},
                {"q-key-time", signTime},
                {"q-header-list", string.Join(";", sortedHeaders.Select(k => k.Key))},
                {"q-url-param-list", string.Join(";", sortedQuerys.Select(k => k.Key))},
                {"q-signature", signature}
            };
            return string.Join("&", m.Select(k => k.Key + "=" + k.Value));
        }
    }
}