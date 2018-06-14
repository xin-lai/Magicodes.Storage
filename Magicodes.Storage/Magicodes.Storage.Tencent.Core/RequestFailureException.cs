using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    /// COS请求失败错误描述。
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

        public string ResourceURL { get; set; }

        public string RequestId { get; set; }

        public string TraceId { get; set; }

        public override string ToString() => $"{HttpMethod} {ResourceURL} - {HttpStatusCode}[{ErrorCode}]";
    }
}
