namespace Magicodes.Storage.AliyunOss.Core
{
    public class AliyunOssConfig
    {
        /// <summary>
        /// OSS的访问ID
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// OSS的访问密钥
        /// </summary>
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// OSS的访问地址
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        ///    存储桶名称
        /// </summary>
        public string BucketName { get; set; }
    }
}
