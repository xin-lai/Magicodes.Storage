using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Storage.Core
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
