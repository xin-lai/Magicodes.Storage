// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : Extentions.cs
//           description :
//   
//           created by 雪雁 at  2018-12-11 20:19
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System;
using System.Threading.Tasks;
using COSXML.Model;
using Magicodes.Storage.Core;

namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    ///     扩展方法
    /// </summary>
    public static class Extentions
    {
        /// <summary>
        ///     根据错误类型返回错误异常
        /// </summary>
        /// <returns></returns>
        public static Task HandlerError(this CosResult response, string friendlyMessage = null)
        {
            var code = (int)response.httpCode;
            if (code < 300 || code >= 600) return Task.FromResult(0);

            var message = response.httpMessage;
            throw new StorageException(
                new StorageError { Code = code, Message = friendlyMessage ?? message, ProviderMessage = message },
                new Exception($"腾讯云存储错误！"));
        }
    }
}