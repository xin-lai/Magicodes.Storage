using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Magicodes.Storage.Core.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>  
        /// 获取枚举的显示内容  
        /// </summary>  
        /// <param name="en">枚举</param>  
        /// <returns>返回枚举的描述</returns>  
        public static string GetDisplayContent(this Enum en)
        {
            var type = en.GetType();   //获取类型  
            var memberInfos = type.GetMember(en.ToString());   //获取成员  
            if (memberInfos != null && memberInfos.Length > 0)
            {
                //获取特性  
                if (memberInfos[0].GetCustomAttributes(typeof(DisplayAttribute), false) is DisplayAttribute[] attrs && attrs.Length > 0)
                {
                    return attrs[0].Name ?? attrs[0].Description;    //返回当前名称  
                }
            }
            return en.ToString();
        }
    }
}
