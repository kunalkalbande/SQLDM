// ===============================
// AUTHOR       : CWF Team - Gowrish 
// PURPOSE      : Backend Isolation
// TICKET       : SQLDM-29086
// ===============================
using BBS.TracerX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Idera.SQLdm.Service.Helpers.CWF
{
    public static class JsonHelper
    {
        private static readonly Logger LogX = Logger.GetLogger("JsonHelper");
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

        public static string ToJSON<T>(this T o) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(o.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, o);
                return (Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        public static string RemovePasswordFromJson(string json)
        {
            string pattern = "";
            string dictPattern = "{\"Key\":\".*\",\"Value\":\".*\"}";
            bool isDict = false;
            Match match = Regex.Match(json, dictPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                isDict = true;
            }
            if (isDict)
            {
                /*
                 * This pattern will extract password from a json string like,
                 * {"Key":"connectionpassword","Value":"secretpassword"}
                 */
                pattern = "{\"Key\":\".*?password.*?\",\"Value\":\"(?<password>.*?)\"}";
            }
            else
            {
                /*
                 * If the json object, is not a dict but a Product's object then
                 * we have to user a different pattern than dict.
                 * i.e. This pattern will extract password from a json string like,
                 * {"connectionpassword":"secretpassword"}
                 */
                pattern = "\".*?password.*?\":\"(?<password>.*?)\"[,}]";
            }
            MatchCollection passwords = Regex.Matches(json, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match pass in passwords)
            {
                if (pass.Success && !String.IsNullOrEmpty(pass.Groups["password"].Value))
                {
                    json = json.Replace(pass.Groups["password"].Value, "*****");
                }
            }
            return json;
        }

        public static string RemoveAuthenticationHeaderFromJson(string json)
        {
            string pattern = "";
            string dictPattern = "{\"Key\":\".*\",\"Value\":\".*\"}";
            bool isDict = false;
            Match match = Regex.Match(json, dictPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                isDict = true;
            }
            if (isDict)
            {
                /*
                 * This pattern will extract password from a json string like,
                 * {"Key":"connectionpassword","Value":"secretpassword"}
                 */
                pattern = "{\"Key\":\".*?authenticationheader.*?\",\"Value\":\"(?<authenticationheader>.*?)\"}";
            }
            else
            {
                /*
                 * If the json object, is not a dict but a Product's object then
                 * we have to user a different pattern than dict.
                 * i.e. This pattern will extract password from a json string like,
                 * {"connectionpassword":"secretpassword"}
                 */
                pattern = "\".*?authenticationheader.*?\":\"(?<authenticationheader>.*?)\"[,}]";
            }
            MatchCollection passwords = Regex.Matches(json, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match pass in passwords)
            {
                if (pass.Success && !String.IsNullOrEmpty(pass.Groups["authenticationheader"].Value))
                {
                    json = json.Replace(pass.Groups["authenticationheader"].Value, "*****");
                }
            }
            return json;
        }

        public static string RemoveAuthHeaderAndPassword(string json) {
            try
            {
                string jsonWithoutPassword = RemovePasswordFromJson(json);
                string jsonWithoutPasswordAndHeader = RemoveAuthenticationHeaderFromJson(jsonWithoutPassword);
                return jsonWithoutPasswordAndHeader;
            }
            catch (Exception e) {
                LogX.ErrorFormat("Error in removing Header and Password from json, {0}", e.Message);
            }
            return json;
        }

    }
}
