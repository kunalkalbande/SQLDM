using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.CWFRegister.Helpers
{
    public static class JsonHelper
    {

        static Logger Log = Logger.GetLogger("JsonHelper");

        public static T FromJSON<T>(string json) where T : class
        {
            using (Log.DebugCall("FromJSON"))
            {
                if (string.IsNullOrEmpty(json)) return (null);
                try
                {
                    Log.DebugFormat("JSON ={0}", json);
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
                    using (MemoryStream s = json.ToStream())
                    {
                        return (deserializer.ReadObject(s) as T);
                    }
                }
                catch (Exception ex)
                {
                    //ExceptionLogger.Log(string.Format("JsonHelper.FromJSON<{0}>({1})", typeof(T), json), ex);
                    Log.ErrorFormat("Serialize error from JSON = {0}", ex.Message);
                    return (null);
                }
            }
        }

        public static string ToJSON<T>(T obj) where T : class
        {
            using (Log.DebugCall("ToJSON"))
            {
                if (obj == null) return (string.Empty);
                try
                {
                    Log.DebugFormat("Object ={0}", obj.ToString());
                    DataContractJsonSerializer customJsonSettings = new DataContractJsonSerializer(typeof(T));
                    MemoryStream stream = new MemoryStream();

                    customJsonSettings.WriteObject(stream, obj);
                    return (Encoding.ASCII.GetString(stream.ToArray()));
                }
                catch (Exception ex)
                {
                    //ExceptionLogger.Log(string.Format("JsonHelper.FromJSON<{0}>({1})", typeof(T), json), ex);
                    Log.ErrorFormat("Serialize error to JSON = {0}",ex.Message);
                    return (null);
                }
            }
        }
    }
}
