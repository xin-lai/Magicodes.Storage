// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : BlobUrlAccess.cs
//          description :
//  
//          created by 李文强 at  2016/09/23 9:41
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;

namespace Magicodes.Storage
{
    /// <summary>
    ///     容器地址访问类型（允许多个状态组合）
    /// </summary>
    [Flags]
    public enum BlobUrlAccess
    {
        None = 0,
        Read = 1,
        Write = 2,
        Delete = 4,
        All = Read | Write | Delete
    }
}