using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Installer_form_application
{
    internal static class StringExtensions
    {
        public static MemoryStream ToStream(this string s)
        {
            MemoryStream stream = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            stream.Write(bytes, 0, bytes.Length);
            if (stream.Position > 0) stream.Position = 0;
            return (stream);
        }
    }

    public static class JsonHelper
    {
        public static T FromJSON<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json)) return (null);
            try
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
                using (MemoryStream s = json.ToStream())
                {
                    return (deserializer.ReadObject(s) as T);
                }
            }
            catch (Exception ex)
            {
                //ExceptionLogger.Log(string.Format("JsonHelper.FromJSON<{0}>({1})", typeof(T), json), ex);
                return (null);
            }
        }

    }
}
