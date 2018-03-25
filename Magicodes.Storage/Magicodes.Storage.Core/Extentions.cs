using System;
using System.Collections.Generic;
using System.Text;
using Magicodes.Storage.Core.Helper;

namespace Magicodes.Storage.Core
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extentions
    {
        /// <summary>
        /// 根据错误类型返回错误异常
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static StorageError ToStorageError(this StorageErrorCode code) => new StorageError()
        {
            Code = (int)code,
            Message = code.GetDisplayContent()
        };
    }
}
