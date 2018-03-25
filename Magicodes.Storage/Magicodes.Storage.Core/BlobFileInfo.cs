// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : BlobDescriptor.cs
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
    ///     文件对象描述
    /// </summary>
    public class BlobFileInfo
    {
        /// <summary>
        ///     内容类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        ///     内容MD5
        /// </summary>
        public string ContentMD5 { get; set; }

        public string ETag { get; set; }

        /// <summary>
        ///     大小
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        ///     最后修改时间
        /// </summary>
        public DateTime? LastModified { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     容器
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        ///     路径
        /// </summary>
        public string Url { get; set; }
    }
}