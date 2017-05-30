// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : LocalStorageProvider.cs
//          description :
//  
//          created by 李文强 at  2016/09/23 9:45
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Magicodes.Storage.Local
{
    /// <summary>
    ///     本地存储提供程序
    /// </summary>
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly string _basePath;
        private readonly string _baseUrl;

        public LocalStorageProvider(string basePath, string baseUrl)
        {
            _basePath = basePath;
            _baseUrl = baseUrl;
        }

        /// <summary>
        ///     删除文件
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        public void DeleteBlob(string containerName, string blobName)
        {
            try
            {
                var path = Path.Combine(_basePath, containerName, blobName);
                File.Delete(path);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException(1002.ToStorageError(), ex);
            }
            catch (ArgumentException ex)
            {
                throw new StorageException(1004.ToStorageError(), ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new StorageException(1005.ToStorageError(), ex);
            }
            catch (NotSupportedException ex)
            {
                throw new StorageException(1004.ToStorageError(), ex);
            }
            catch (IOException ex)
            {
                throw new StorageException(1003.ToStorageError(), ex);
            }
            catch (Exception ex)
            {
                throw new StorageException(1001.ToStorageError(), ex);
            }
        }

        /// <summary>
        ///     删除文件
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public Task DeleteBlobAsync(string containerName, string blobName)
        {
            return Task.Factory.StartNew(() => DeleteBlob(containerName, blobName));
        }

        /// <summary>
        ///     删除容器
        /// </summary>
        /// <param name="containerName">容器名称</param>
        public void DeleteContainer(string containerName)
        {
            try
            {
                var path = Path.Combine(_basePath, containerName);
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException(1002.ToStorageError(), ex);
            }
            catch (ArgumentException ex)
            {
                throw new StorageException(1005.ToStorageError(), ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new StorageException(1005.ToStorageError(), ex);
            }
            catch (IOException ex)
            {
                throw new StorageException(1003.ToStorageError(), ex);
            }
            catch (Exception ex)
            {
                throw new StorageException(1001.ToStorageError(), ex);
            }
        }

        /// <summary>
        ///     删除容器
        /// </summary>
        /// <param name="containerName">容器名称</param>
        /// <returns></returns>
        public Task DeleteContainerAsync(string containerName)
        {
            return Task.Run(() => DeleteContainer(containerName));
        }

        public BlobDescriptor GetBlobDescriptor(string containerName, string blobName)
        {
            var path = Path.Combine(_basePath, containerName, blobName);

            try
            {
                var info = new FileInfo(path);

                return new BlobDescriptor
                {
                    Container = containerName,
                    ContentMD5 = "",
                    ContentType = info.Extension.GetMimeType(),
                    ETag = "",
                    LastModified = info.LastWriteTimeUtc,
                    Length = info.Length,
                    Name = info.Name,
                    Security = BlobSecurity.Private,
                    Url = info.FullName
                };
            }
            catch (Exception ex)
            {
                throw new StorageException(1001.ToStorageError(), ex);
            }
        }

        public Task<BlobDescriptor> GetBlobDescriptorAsync(string containerName, string blobName)
        {
            return Task.Run((Action) (() => GetBlobDescriptor(containerName, blobName))) as Task<BlobDescriptor>;
        }

        public string GetBlobSasUrl(string containerName, string blobName, DateTimeOffset expiry,
            bool isDownload = false, string fileName = null, string contentType = null,
            BlobUrlAccess access = BlobUrlAccess.Read)
        {
            return Path.Combine(_basePath, containerName, blobName);
        }

        public Stream GetBlobStream(string containerName, string blobName)
        {
            try
            {
                var path = Path.Combine(_basePath, containerName, blobName);
                return File.OpenRead(path);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException(1002.ToStorageError(), ex);
            }
            catch (ArgumentException ex)
            {
                throw new StorageException(1004.ToStorageError(), ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new StorageException(1005.ToStorageError(), ex);
            }
            catch (NotSupportedException ex)
            {
                throw new StorageException(1004.ToStorageError(), ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new StorageException(1004.ToStorageError(), ex);
            }
            catch (IOException ex)
            {
                throw new StorageException(1006.ToStorageError(), ex);
            }
            catch (Exception ex)
            {
                throw new StorageException(1001.ToStorageError(), ex);
            }
        }

        public async Task<Stream> GetBlobStreamAsync(string containerName, string blobName)
        {
            return await Task.Run(() => GetBlobStream(containerName, blobName));
        }

        public string GetBlobUrl(string containerName, string blobName)
        {
            return string.Format("{0}/{1}/{2}", _baseUrl, containerName, blobName);
        }

        public IList<BlobDescriptor> ListBlobs(string containerName)
        {
            var localFilesInfo = new List<BlobDescriptor>();

            try
            {
                var dir = Path.Combine(_basePath, containerName);
                var dirInfo = new DirectoryInfo(dir);
                var fileInfo = dirInfo.GetFiles();

                foreach (var f in fileInfo)
                    localFilesInfo.Add(new BlobDescriptor
                    {
                        ContentMD5 = "",
                        ETag = "",
                        ContentType = f.Extension.GetMimeType(),
                        Container = containerName,
                        LastModified = f.LastWriteTime,
                        Length = f.Length,
                        Name = f.Name,
                        Url = f.FullName,
                        Security = BlobSecurity.Private
                    });

                return localFilesInfo;
            }
            catch (Exception ex)
            {
                throw new StorageException(1001.ToStorageError(), ex);
            }
        }

        public Task<IList<BlobDescriptor>> ListBlobsAsync(string containerName)
        {
            return Task.Run((Action) (() => ListBlobs(containerName))) as Task<IList<BlobDescriptor>>;
        }

        public void SaveBlobStream(string containerName, string blobName, Stream source,
            BlobProperties properties = null)
        {
            var dir = Path.Combine(_basePath, containerName);

            try
            {
                Directory.CreateDirectory(dir);
                using (var file = File.Create(Path.Combine(dir, blobName)))
                {
                    source.CopyTo(file);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException(1002.ToStorageError(), ex);
            }
            catch (ArgumentException ex)
            {
                throw new StorageException(1004.ToStorageError(), ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new StorageException(1005.ToStorageError(), ex);
            }
            catch (NotSupportedException ex)
            {
                throw new StorageException(1004.ToStorageError(), ex);
            }
            catch (IOException ex)
            {
                throw new StorageException(1006.ToStorageError(), ex);
            }
            catch (Exception ex)
            {
                throw new StorageException(1001.ToStorageError(), ex);
            }
        }

        public async Task SaveBlobStreamAsync(string containerName, string blobName, Stream source,
            BlobProperties properties = null)
        {
            var dir = Path.Combine(_basePath, containerName);

            try
            {
                Directory.CreateDirectory(dir);
                using (var file = File.Create(Path.Combine(dir, blobName)))
                {
                    await source.CopyToAsync(file);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException(1002.ToStorageError(), ex);
            }
            catch (ArgumentException ex)
            {
                throw new StorageException(1004.ToStorageError(), ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new StorageException(1005.ToStorageError(), ex);
            }
            catch (NotSupportedException ex)
            {
                throw new StorageException(1004.ToStorageError(), ex);
            }
            catch (IOException ex)
            {
                throw new StorageException(1006.ToStorageError(), ex);
            }
            catch (Exception ex)
            {
                throw new StorageException(1001.ToStorageError(), ex);
            }
        }

        public void UpdateBlobProperties(string containerName, string blobName, BlobProperties properties)
        {
        }

        public Task UpdateBlobPropertiesAsync(string containerName, string blobName, BlobProperties properties)
        {
            return Task.FromResult(0);
        }
    }
}