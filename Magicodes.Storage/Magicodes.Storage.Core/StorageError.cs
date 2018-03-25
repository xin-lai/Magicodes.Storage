// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : StorageError.cs
//          description :
//  
//          created by 李文强 at  2018/03/25 9:41
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//          交流QQ群（.NET 技术交流群）：85318032
//  
// ======================================================================

using Magicodes.Storage.Core.Helper;

namespace Magicodes.Storage.Core
{
    /// <summary>
    /// 错误
    /// </summary>
    public class StorageError
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 处理程序错误消息
        /// </summary>
        public string ProviderMessage { get; set; }
    }
}