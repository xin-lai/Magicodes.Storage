// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : LocalStorageProvider.cs
//          description :
//  
//          created by 李文强 at  2018/03/25 9:45
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//          交流QQ群（.NET 技术交流群）：85318032
//  
// ======================================================================

namespace Magicodes.Storage.Local.Core
{
    using Magicodes.Storage.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// 本地存储提供程序
    /// </summary>
    public class LocalStorageProvider : IStorageProvider
    {
        /// <summary>
        /// Defines the _rootPath
        /// </summary>
        private readonly string _rootPath;

        /// <summary>
        /// Defines the _rootUrl
        /// </summary>
        private readonly string _rootUrl;

        /// <summary>
        /// Gets the ProviderName
        /// </summary>
        public string ProviderName => "Local";

        /// <summary>
        /// Gets or sets the AllowExtensionList
        /// 允许的扩展列表
        /// </summary>
        public IList<string> AllowExtensionList { get; set; }

        /// <summary>
        /// The ExceptionHandling
        /// </summary>
        /// <param name="ioAction">The ioAction<see cref="Action"/></param>
        private void ExceptionHandling(Action ioAction)
        {
            try
            {
                ioAction();
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException(StorageErrorCode.InvalidAccess.ToStorageError(), ex);
            }
            catch (ArgumentException ex)
            {
                throw new StorageException(StorageErrorCode.InvalidBlobName.ToStorageError(), ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new StorageException(StorageErrorCode.ContainerNotFound.ToStorageError(), ex);
            }
            catch (NotSupportedException ex)
            {
                throw new StorageException(StorageErrorCode.InvalidBlobName.ToStorageError(), ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new StorageException(StorageErrorCode.FileNotFound.ToStorageError(), ex);
            }
            catch (IOException ex)
            {
                throw new StorageException(StorageErrorCode.BlobInUse.ToStorageError(), ex);
            }
            catch (Exception ex)
            {
                throw new StorageException(StorageErrorCode.GenericException.ToStorageError(), ex);
            }
        }

        /// <summary>
        /// The ExceptionHandling
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ioFunc">The ioFunc<see cref="Func{T}"/></param>
        /// <returns>The <see cref="T"/></returns>
        private T ExceptionHandling<T>(Func<T> ioFunc)
        {
            try
            {
                return ioFunc();
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new StorageException(StorageErrorCode.InvalidAccess.ToStorageError(), ex);
            }
            catch (ArgumentException ex)
            {
                throw new StorageException(StorageErrorCode.InvalidBlobName.ToStorageError(), ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new StorageException(StorageErrorCode.ContainerNotFound.ToStorageError(), ex);
            }
            catch (NotSupportedException ex)
            {
                throw new StorageException(StorageErrorCode.InvalidBlobName.ToStorageError(), ex);
            }
            catch (FileNotFoundException ex)
            {
                throw new StorageException(StorageErrorCode.FileNotFound.ToStorageError(), ex);
            }
            catch (IOException ex)
            {
                throw new StorageException(StorageErrorCode.BlobInUse.ToStorageError(), ex);
            }
            catch (Exception ex)
            {
                throw new StorageException(StorageErrorCode.GenericException.ToStorageError(), ex);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalStorageProvider"/> class.
        /// </summary>
        /// <param name="rootPath">文件根路径</param>
        /// <param name="rootUrl">根Url</param>
        public LocalStorageProvider(string rootPath, string rootUrl)
        {
            _rootPath = rootPath;
            _rootUrl = rootUrl;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DeleteBlob(string containerName, string blobName)
        {
            await Task.Run(() =>
            {
                ExceptionHandling(() =>
                {
                    var path = Path.Combine(_rootPath, containerName, blobName);
                    File.Delete(path);
                });
            });
        }

        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerName">容器名称</param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task DeleteContainer(string containerName)
        {
            await Task.Run(() =>
            {
                ExceptionHandling(() =>
                {
                    var path = Path.Combine(_rootPath, containerName);
                    Directory.Delete(path, true);
                });
            });
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public async Task<BlobFileInfo> GetBlobFileInfo(string containerName, string blobName)
        {
            return await Task.Run(() => ExceptionHandling(() =>
             {
                 var path = Path.Combine(_rootPath, containerName, blobName);
                 var info = new FileInfo(path);

                 return new BlobFileInfo
                 {
                     Container = containerName,
                     ContentMD5 = "",
                     ContentType = info.Extension.GetMimeType(),
                     ETag = "",
                     LastModified = info.LastWriteTimeUtc,
                     Length = info.Length,
                     Name = info.Name,
                     Url = GetUrl(containerName, blobName)
             };
             }));
        }

        /// <summary>
        /// The GetBlobStream
        /// </summary>
        /// <param name="containerName">The containerName<see cref="string"/></param>
        /// <param name="blobName">The blobName<see cref="string"/></param>
        /// <returns>The <see cref="Task{Stream}"/></returns>
        public async Task<Stream> GetBlobStream(string containerName, string blobName)
        {
            return await Task.Run(() =>
            {
                return ExceptionHandling(() =>
                {
                    var path = Path.Combine(_rootPath, containerName, blobName);
                    return (Stream)File.OpenRead(path);
                });
            });
        }

        /// <summary>
        /// The GetBlobUrl
        /// </summary>
        /// <param name="containerName">The containerName<see cref="string"/></param>
        /// <param name="blobName">The blobName<see cref="string"/></param>
        /// <returns>The <see cref="Task{string}"/></returns>
        public Task<string> GetBlobUrl(string containerName, string blobName)
        {
            var path = Path.Combine(_rootPath, containerName, blobName);
            if (!Directory.Exists(Path.Combine(_rootPath, containerName)))
            {
                throw new StorageException(StorageErrorCode.ContainerNotFound.ToStorageError(), null);
            }

            if (!File.Exists(path))
            {
                throw new StorageException(StorageErrorCode.FileNotFound.ToStorageError(), null);
            }
            var url = GetUrl(containerName, blobName);
            return Task.FromResult(url);
        }

        private string GetUrl(string containerName, string blobName) => string.Format("{0}/{1}/{2}", _rootUrl.TrimEnd('/'), containerName, blobName);
        /// <summary>
        /// The ListBlobs
        /// </summary>
        /// <param name="containerName">The containerName<see cref="string"/></param>
        /// <returns>The <see cref="Task{IList{BlobFileInfo}}"/></returns>
        public async Task<IList<BlobFileInfo>> ListBlobs(string containerName)
        {
            return await Task.Run(() =>
            {
                return ExceptionHandling(() =>
                {
                    var localFilesInfo = new List<BlobFileInfo>();
                    var dir = Path.Combine(_rootPath, containerName);
                    var dirInfo = new DirectoryInfo(dir);
                    var fileInfo = dirInfo.GetFiles();

                    foreach (var f in fileInfo)
                        localFilesInfo.Add(new BlobFileInfo
                        {
                            ContentMD5 = "",
                            ETag = "",
                            ContentType = f.Extension.GetMimeType(),
                            Container = containerName,
                            LastModified = f.LastWriteTime,
                            Length = f.Length,
                            Name = f.Name,
                            Url = f.FullName,
                        });

                    return localFilesInfo;
                });
            });
        }

        /// <summary>
        /// The SaveBlobStream
        /// </summary>
        /// <param name="containerName">The containerName<see cref="string"/></param>
        /// <param name="blobName">The blobName<see cref="string"/></param>
        /// <param name="source">The source<see cref="Stream"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task SaveBlobStream(string containerName, string blobName, Stream source)
        {
            await Task.Run(() =>
            {
                ExceptionHandling(() =>
                {
                    var dir = Path.Combine(_rootPath, containerName);
                    Directory.CreateDirectory(dir);
                    using (var file = File.Create(Path.Combine(dir, blobName)))
                    {
                        if (AllowExtensionList != null && AllowExtensionList.Contains((Path.GetExtension(blobName) ?? "".ToLower())))
                        {
                            throw new StorageException(StorageErrorCode.UnsupportedFileType.ToStorageError(), new Exception("不支持 " + Path.GetExtension(blobName) + " 类型的文件上传，请查看允许的扩展名设置！"));
                        }
                        source.CopyTo(file);
                    }
                });
            });
        }

        /// <summary>
        /// The GetBlobUrl
        /// </summary>
        /// <param name="containerName">The containerName<see cref="string"/></param>
        /// <param name="blobName">The blobName<see cref="string"/></param>
        /// <param name="expiry">The expiry<see cref="DateTime"/></param>
        /// <param name="isDownload">The isDownload<see cref="bool"/></param>
        /// <param name="fileName">The fileName<see cref="string"/></param>
        /// <param name="contentType">The contentType<see cref="string"/></param>
        /// <param name="access">The access<see cref="BlobUrlAccess"/></param>
        /// <returns>The <see cref="Task{string}"/></returns>
        public Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry, bool isDownload = false, string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read) => throw new NotSupportedException();
    }
}
