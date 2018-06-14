using Magicodes.Storage.Core;
using Magicodes.Storage.Tencent.Core.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Magicodes.Storage.Tencent.Core
{
    /// <summary>
    /// 腾讯云COS服务
    /// </summary>
    public class TencentCosService
    {
        public readonly TencentCosConfig Config = null;
        public readonly Bucket StorageBucket = null;
        private readonly HttpClient BackChannel;
        private TencentCosSignHelper SignHelper = null;

        public TencentCosService(TencentCosConfig cfg, Bucket bucket, HttpClient backChannel = null)
        {
            Config = cfg ?? throw new ArgumentNullException($"参数{nameof(cfg)}为空!");
            StorageBucket = bucket ?? throw new ArgumentNullException($"参数{nameof(bucket)}为空!");
            BackChannel = backChannel ?? new HttpClient();
            SignHelper = new TencentCosSignHelper(cfg);
        }

        /// <summary>
        /// Bucket是否存在
        /// </summary>
        /// <returns></returns>

        public async Task<bool> BucketExists()
        {
            var endpoint = StorageBucket.Url + "/";
            var req = new HttpRequestMessage(HttpMethod.Head, endpoint);
            using (var resp = await SendAsync(req))
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// ListBucketsAsync返回所有存储空间列表
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public async Task<Bucket[]> ListBucketsAsync(string endpoint = "https://service.cos.myqcloud.com/")
        {
            var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
            using (var resp = await SendAsync(req))
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    RequestFailure(HttpMethod.Get, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }

                var doc = new XmlDocument();
                doc.LoadXml(await resp.Content.ReadAsStringAsync());
                var buckets = new List<Bucket>();
                foreach (XPathNavigator elem in doc.DocumentElement.CreateNavigator().Select("//Buckets/Bucket"))
                {
                    var fullname = elem.SelectSingleNode("Name").InnerXml;
                    var i = fullname.LastIndexOf("-");
                    var bucket = new Bucket(
                        appId: fullname.Substring(i + 1),
                        name: fullname.Substring(0, i),
                        region: elem.SelectSingleNode("Location").InnerXml);
                    buckets.Add(bucket);
                }
                return buckets.ToArray();
            }
        }

        /// <summary>
        /// PutBucketAsync创建一个新的存储桶(Bucket)。
        /// https://cloud.tencent.com/document/product/436/7738
        /// </summary>
        /// <param name="name">桶名称</param>
        /// <param name="region">桶在区域</param>
        /// <param name="header">自定义附加请求的标头</param>
        /// <returns></returns>
        public async Task<Bucket> PutBucketAsync(string name, string region, Dictionary<string, string> headers = null)
        {
            var bucket = new Bucket(Config.AppId, name, region);
            var endpoint = bucket.Url + "/";
            var req = new HttpRequestMessage(HttpMethod.Put, endpoint);
            if (headers?.Count > 0)
            {
                foreach (var header in headers)
                {
                    req.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            using (var resp = await SendAsync(req))
            {
                var payload = await resp.Content.ReadAsStringAsync();
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }
                return bucket;
            }
        }

        /// <summary>
        /// DeleteBucketAsync删除一个指定的存储桶(Bucket)。
        /// </summary>
        /// <param name="name">桶名称</param>
        /// <param name="region">桶在区域</param>
        /// <returns></returns>
        public async Task DeleteBucketAsync()
        {
            //DELETE / HTTP/1.1
            var endpoint = StorageBucket.Url + "/";
            var req = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            using (var resp = await SendAsync(req))
            {
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        break;
                    default:
                        RequestFailure(HttpMethod.Delete, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                        break;
                }
            }
        }

        /// <summary>
        /// PutObjectAsync上传文件到指定的URL。
        /// https://cloud.tencent.com/document/product/436/7749
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="content"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public async Task PutObjectAsync(string objectName, Stream content, 
            Dictionary<string, string> headers = null)
        {
            var endpoint = StorageBucket.Url + $"/{objectName}";

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            var req = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = new StreamContent(content)
            };
            if (headers?.Count > 0)
            {
                foreach (var header in headers)
                {
                    req.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            using (var resp = await SendAsync(req))
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// 获取文件描述信息
        /// </summary>
        /// <param name="objectName">文件名称</param>
        /// <returns></returns>
        public async Task<BlobFileInfo> GetBlobFileInfo(string objectName)
        {
            var endpoint = StorageBucket.Url + $"/{objectName}";
            BlobFileInfo fileInfo = new BlobFileInfo();
            var req = new HttpRequestMessage(HttpMethod.Head, endpoint);
            using (var resp = await SendAsync(req))
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    fileInfo.Container = StorageBucket.Name;
                    fileInfo.Name = objectName;
                    fileInfo.ETag = resp.Headers.ETag.ToString();
                }
                else
                    RequestFailure(HttpMethod.Head, resp.StatusCode, await resp.Content.ReadAsStringAsync());
            }
            return fileInfo;
        }

        /// <summary>
        /// GetObjectAsync读取上传的文件内容。
        /// https://cloud.tencent.com/document/product/436/7753
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Stream> GetObjectAsync(string objectName)
        {
            //DELETE / HTTP/1.1
            var endpoint = StorageBucket.Url + $"/{objectName}";
            var req = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var resp = await SendAsync(req);
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
            }
            return await resp.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// DeleteObjectAsync删除指定的文件。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task DeleteObjectAsync(Bucket bucket, string objectName)
        {
            string url = string.Concat(bucket.Url, "/", objectName);
            var req = new HttpRequestMessage(HttpMethod.Delete, url);

            using (var resp = await SendAsync(req))
            {
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        break;
                    default:
                        RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
                        break;
                }
            }
        }
        //public async Task DeleteObjectAsync(string url)
        //{
        //    var req = new HttpRequestMessage(HttpMethod.Delete, url);
        //    using (var resp = await SendAsync(req))
        //    {
        //        switch (resp.StatusCode)
        //        {
        //            case HttpStatusCode.OK:
        //            case HttpStatusCode.NoContent:
        //                break;
        //            default:
        //                RequestFailure(HttpMethod.Put, resp.StatusCode, await resp.Content.ReadAsStringAsync());
        //                break;
        //        }
        //    }
        //}


        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="req">请求对象</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req)
        {
            req.Headers.Host = req.RequestUri.Host;
            req.Headers.TryAddWithoutValidation("Authorization", SignHelper.SignRequest(req));
            return await BackChannel.SendAsync(req);
        }

        /// <summary>
        /// 请求失败处理
        /// </summary>
        /// <param name="method"></param>
        /// <param name="respStatusCode"></param>
        /// <param name="respContent"></param>
        private void RequestFailure(HttpMethod method, HttpStatusCode respStatusCode, string respContent)
        {
            var doc = new XmlDocument();
            doc.LoadXml(respContent);
            var root = doc.DocumentElement.CreateNavigator().SelectSingleNode("//Error");
            var ex = new RequestFailureException(method.ToString(), root.SelectSingleNode("Message").InnerXml)
            {
                HttpStatusCode = (int)respStatusCode,
                ErrorCode = root.SelectSingleNode("Code").InnerXml,
                ResourceURL = root.SelectSingleNode("Resource").InnerXml,
                RequestId = root.SelectSingleNode("RequestId").InnerXml,
                TraceId = root.SelectSingleNode("TraceId").InnerXml,
            };
            throw new StorageException(StorageErrorCode.PostError.ToStorageError(),
              new Exception(ex.ToString()));
        }

        //private StorageException GetStorageExcept(HttpMethod method, HttpStatusCode respStatusCode,
        //    RequestFailureException ex)
        //{
        //    StorageException storageException = null;
        //    //PostError
        //    //switch (ex.ErrorCode)
        //    //{
        //    //    case "BadDigest": //提供的x-cos-SHA-1值与服务端收到的文件SHA-1值不符合
        //    //        storageException = new StorageException(StorageErrorCode.NoCredentialsProvided.ToStorageError(),
        //    //         new Exception(ex.ToString()));
        //    //        break;
        //    //    case "EntityTooSmall": //上传的文件大小 不足要求的最小值，常见于分片上传
        //    //    case "EntityTooLarge": //上传的文件大小超过要求的最大值
        //    //    case "ImcompleteBody":
        //    //        storageException = new StorageException(StorageErrorCode.SizeError.ToStorageError(),
        //    //       new Exception(ex.ToString()));
        //    //        break;
        //    //    case "IncorrectNumberOfFilesInPostRequest": //Post请求每次只允许上传一个文件
        //    //        storageException = new StorageException(StorageErrorCode.PostError.ToStorageError(),
        //    //    new Exception(ex.ToString()));
        //    //        break;
        //    //    default:
        //    //        storageException = new StorageException(StorageErrorCode.GenericException.ToStorageError(),
        //    //            new Exception(ex.ToString()));
        //    //        break;
        //    //}
        //}

    }
}
