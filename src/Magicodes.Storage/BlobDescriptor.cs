// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : BlobDescriptor.cs
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
    ///     容器描述
    /// </summary>
    public class BlobDescriptor
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
        public DateTimeOffset? LastModified { get; set; }

        /// <summary>
        ///     安全设置类型
        /// </summary>
        public BlobSecurity Security { get; set; }

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