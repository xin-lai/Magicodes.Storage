using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Storage.Core.Helper
{


    /// <summary>
    /// 日期时间帮助类
    /// </summary>
    public static class DateTimeHelper
    { 

        /// <summary> 
        /// 获取指定时间的Unix时间戳 10位
        /// </summary> 
        /// <returns></returns> 
        public static long GetTimeStampTen(this DateTime date)
        {
            return (date.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        /// 将时间戳转换为日期类型，并格式化
        /// </summary>
        /// <param name="longDateTime"></param>
        /// <returns></returns>
        private static string LongDateTimeToDateTimeString(this string longDateTime)
        {
            //用来格式化long类型时间的,声明的变量
            long unixDate;
            DateTime start;
            DateTime date;
            //ENd

            unixDate = long.Parse(longDateTime);
            start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            date = start.AddMilliseconds(unixDate).ToLocalTime();
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>  
        /// 获取时间戳  13位
        /// </summary>  
        /// <returns></returns>  
        public static long GetTimeStamp(this DateTime date)
        { 
            TimeSpan ts = date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds * 1000);
        }
    }
}
