using Aliyun.OSS.Common;

namespace Magicodes.Storage.Aliyun.Core
{
    public class AliyunOssProviderOptions
    {
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string Endpoint { get; set; }
        public ClientConfiguration Config { get; set; }
    }
}