// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : StorageException.cs
//          description :
//  
//          created by 李文强 at  2018/03/25 9:41
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//          交流QQ群（.NET 技术交流群）：85318032
//  
// ======================================================================

using System;

namespace Magicodes.Storage.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class StorageException : Exception
    {
        public StorageException(StorageError error, Exception ex) : base(error.Message, ex)
        {
            ErrorCode = error.Code;
            ProviderMessage = ex?.Message;
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// 提供程序消息
        /// </summary>
        public string ProviderMessage { get; set; }
    }
}