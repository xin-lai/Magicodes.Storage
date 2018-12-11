// ======================================================================
//   
//           Copyright (C) 2018-2020 湖南心莱信息科技有限公司    
//           All rights reserved
//   
//           filename : ConfigHelper.cs
//           description :
//   
//           created by 雪雁 at  2018-12-11 21:45
//           Mail: wenqiang.li@xin-lai.com
//           QQ群：85318032（技术交流）
//           Blog：http://www.cnblogs.com/codelove/
//           GitHub：https://github.com/xin-lai
//           Home：http://xin-lai.com
//   
// ======================================================================

using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Magicodes.Storage.Tests.Helper
{
    public class ConfigHelper
    {
        public static T LoadConfig<T>(string name) where T : class, new()
        {
            var config = new T();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), name + ".json");
            if (File.Exists(filePath))
            {
                config = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            }
            else
            {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(config), Encoding.UTF8);
            }

            return config;
        }
    }
}