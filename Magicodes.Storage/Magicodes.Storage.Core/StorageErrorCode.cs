// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : StorageErrorCode.cs
//          description :
//  
//          created by 李文强 at  2018/03/25 9:41
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//          交流QQ群（.NET 技术交流群）：85318032
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.Storage.Core
{
    /// <summary>
    /// 错误码
    /// </summary>
    public enum StorageErrorCode
    {
        /// <summary>
        /// 没有错误
        /// </summary>
        [Display(Name = "没有错误")]
        None = 0,

        /// <summary>
        /// 无效的安全凭据
        /// </summary>
        [Display(Name = "无效的安全凭据")]
        InvalidCredentials = 1000,

        /// <summary>
        /// 提供程序出现未知错误
        /// </summary>
        [Display(Name = "提供程序出现未知错误")]
        GenericException = 1001,

        /// <summary>
        /// 无效的访问凭据
        /// </summary>
        [Display(Name = "无效的访问凭据")]
        InvalidAccess = 1002,

        /// <summary>
        /// 文件被占用
        /// </summary>
        [Display(Name = "文件被占用")]
        BlobInUse = 1003,

        /// <summary>
        /// 无效的文件名称
        /// </summary>
        [Display(Name = "无效的文件名称")]
        InvalidBlobName = 1004,

        /// <summary>
        /// 无效的容器名称
        /// </summary>
        [Display(Name = "无效的容器名称")]
        InvalidContainerName = 1005,

        /// <summary>
        /// 读取文件错误
        /// </summary>
        [Display(Name = "读取文件错误")]
        ErrorOpeningBlob = 1006,

        /// <summary>
        /// 凭据或证书错误
        /// </summary>
        [Display(Name = "凭据或证书错误")]
        NoCredentialsProvided = 1007,

        /// <summary>
        /// 没有找到该文件
        /// </summary>
        [Display(Name = "没有找到该文件")]
        FileNotFound = 1008,

        /// <summary>
        /// 没有找到该目录
        /// </summary>
        [Display(Name = "没有找到该容器")]
        ContainerNotFound = 1009
    }
}