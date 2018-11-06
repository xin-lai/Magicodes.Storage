// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : RequestFailureException.cs
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
    ///     COS请求失败错误描述。
    /// </summary>
    public class RequestFailureException : Exception
    {
        public RequestFailureException(string method, string message) : base(message)
        {
            HttpMethod = method;
        }

        public string HttpMethod { get; set; }

        public int HttpStatusCode { get; set; }

        public string ErrorCode { get; set; }

        public string ResourceUrl { get; set; }

        public string RequestId { get; set; }

        public string TraceId { get; set; }

        public override string ToString()
        {
            return $"{HttpMethod} {ResourceUrl} - {HttpStatusCode}[{ErrorCode}]";
        }
    }
}