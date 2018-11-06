// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : Bucket.cs
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

namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    ///     COS存储桶。
    /// </summary>
    public sealed class Bucket
    {
        /// <summary>
        ///     初始化新的<see cref="Bucket" />对象。
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="name"></param>
        /// <param name="region"></param>
        public Bucket(string appId, string name, string region)
        {
            AppId = appId;
            Name = name;
            Region = region;
        }

        /// <summary>
        ///     应用ID。
        /// </summary>
        public string AppId { get; }

        /// <summary>
        ///     名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     所在的区域。
        /// </summary>
        public string Region { get; }

        /// <summary>
        ///     返回桶的访问URL地址，不包含/路径。
        /// </summary>
        public string Url
        {
            get
            {
                if (string.IsNullOrEmpty(AppId)) throw new InvalidOperationException($"{nameof(AppId)} is null.");
                if (string.IsNullOrEmpty(Name)) throw new InvalidOperationException($"{nameof(Name)} is null.");
                if (string.IsNullOrEmpty(Region)) throw new InvalidOperationException($"{nameof(Region)} is null.");
                return $"https://{Name}-{AppId}.cos.{Region}.myqcloud.com";
            }
        }

        public override string ToString()
        {
            return Url;
        }

        /// <summary>
        ///     解析存储桶的URL地址。
        /// </summary>
        /// <param name="url">存储桶的访问域名地址。</param>
        /// <returns><see cref="Bucket" />对象。</returns>
        public static Bucket ParseUrl(string url)
        {
            var host = new Uri(url).Host;
            var a = host.Split('.');
            if (a.Length != 5) throw new ArgumentException("Invalid bucket url.", nameof(url));
            var i = a[0].LastIndexOf("-", StringComparison.CurrentCultureIgnoreCase);
            return new Bucket(a[0].Substring(i + 1), a[0].Substring(0, i), a[2]);
        }
    }
}