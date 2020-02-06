using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Magicodes.Storage.Core.Helper
{
    /// <summary>
    /// 加密帮助类
    /// </summary>
    public class EncryptHelper
    {
        /// <summary>
        /// 对明文进行SHA1加密
        /// </summary>
        /// <param name="content">明文</param>
        /// <returns></returns>
        public static string HashSHA1(string content)
        {
            var buff = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(content));
            return string.Concat(buff.Select(k => k.ToString("x2")));
        }

        /// <summary>
        /// 对明文进行SHA加密
        /// </summary>
        /// <param name="key">加密秘钥key</param>
        /// <param name="content">待加密明文</param>
        /// <returns></returns>
        public static string HashHMACSHA1(string key, string content)
        {
            var buff = new HMACSHA1(Encoding.UTF8.GetBytes(key)).ComputeHash(Encoding.UTF8.GetBytes(content));
            return string.Concat(buff.Select(k => k.ToString("x2")));
        }
    }
}
