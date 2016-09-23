// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Extentions.cs
//          description :
//  
//          created by 李文强 at  2016/09/23 9:41
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicodes.Storage
{
    public static class Extentions
    {
        private static readonly Dictionary<int, string> Errors = new Dictionary<int, string>
        {
            {
                1000,
                "无效的安全凭据"
            },
            {
                1001,
                "提供程序出现未知错误"
            },
            {
                1002,
                "无效的访问凭据"
            },
            {
                1003,
                "Blob 正在使用"
            },
            {
                1004,
                "无效的 blob 或者 container 名称"
            },
            {
                1005,
                "无效的 container 名称."
            },
            {
                1006,
                "打开 blob 时出现错误"
            }
        };

        public static StorageError ToStorageError(this int code)
        {
            return Errors
                .Where(x => x.Key == code)
                .Select(x => new StorageError {Code = x.Key, Message = x.Value})
                .FirstOrDefault();
        }

        public static StorageError ToStorageError(this StorageErrorCode code)
        {
            return Errors
                .Where(x => x.Key == (int) code)
                .Select(x => new StorageError {Code = x.Key, Message = x.Value})
                .FirstOrDefault();
        }

        public static List<T2> SelectToListOrEmpty<T1, T2>(this IEnumerable<T1> e, Func<T1, T2> f)
        {
            if (e == null)
                return new List<T2>();

            return e.Select(f).ToList();
        }

        public static List<T1> WhereToListOrEmpty<T1>(this IEnumerable<T1> e, Func<T1, bool> f)
        {
            if (e == null)
                return new List<T1>();

            return e.Where(f).ToList();
        }
    }
}