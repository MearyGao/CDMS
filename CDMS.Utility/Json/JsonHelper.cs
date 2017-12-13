using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;

namespace CDMS.Utility
{
    public class JsonHelper
    {
        public static string ToJson(object o, string format = "", bool jsEncode = true)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();
            if (string.IsNullOrEmpty(format)) format = "yyyy-MM-dd HH:mm:ss";
            jss.DateFormatString = format;
            string json = JsonConvert.SerializeObject(o, jss);
            if (jsEncode) json = HttpUtility.JavaScriptStringEncode(json);
            return json;
        }

        public static object ToObject(string s)
        {
            return JsonConvert.DeserializeObject(s);
        }

        public static T ToObject<T>(string s)
        {
            return JsonConvert.DeserializeObject<T>(s);
        }
    }
}
