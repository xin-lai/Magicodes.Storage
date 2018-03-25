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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Magicodes.Storage.Core;

namespace Magicodes.Storage.Local.Core
{
    /// <summary>
    ///     本地存储提供程序
    /// </summary>
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly string _rootPath;
        private readonly string _rootUrl;

        /// <summary>
        /// 
        /// </summary>
        public string ProviderName => "Local";

        #region 私有方法
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
        #endregion

        /// <summary>
        /// 本地存储
        /// </summary>
        /// <param name="rootPath">文件根路径</param>
        /// <param name="rootUrl">根Url</param>
        public LocalStorageProvider(string rootPath, string rootUrl)
        {
            _rootPath = rootPath;
            _rootUrl = rootUrl;
        }

        /// <summary>
        ///     删除文件
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
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
        ///     删除容器
        /// </summary>
        /// <param name="containerName">容器名称</param>
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
                     Url = info.FullName
                 };
             }));
        }

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
            return Task.FromResult(string.Format("{0}/{1}/{2}", _rootUrl, containerName, blobName));
        }

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
                        source.CopyTo(file);
                    }
                });
            });
        }

        public Task<string> GetBlobUrl(string containerName, string blobName, DateTime expiry, bool isDownload = false, string fileName = null, string contentType = null, BlobUrlAccess access = BlobUrlAccess.Read) => throw new NotSupportedException();
    }
}