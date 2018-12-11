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
using Amazon.Runtime;
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
        public static void HandlerError(this AmazonWebServiceResponse response, string friendlyMessage = null)
        {
            var code = (int) response.HttpStatusCode;
            if (code < 300 || code >= 600) return;
            var message = response.ResponseMetadata.Metadata["Message"];
            var requestId = response.ResponseMetadata.Metadata["RequestId"];
            var traceId = response.ResponseMetadata.Metadata["TraceId"];
            var resource = response.ResponseMetadata.Metadata["Resource"];
            throw new StorageException(
                new StorageError {Code = code, Message = friendlyMessage ?? message, ProviderMessage = message},
                new Exception($"腾讯云存储错误,详细信息:RequestId:{requestId},traceId:{traceId},resource:{resource}"));
        }
    }
}