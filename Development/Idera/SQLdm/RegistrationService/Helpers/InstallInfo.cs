using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
namespace Idera.SQLdm.RegistrationService.Helpers
{
    /// <summary>
    /// SQL DM 9.0 (Vineet Kumar) (License Changes plus CWF Registration) -- Added helper file for reading INI file
    /// </summary>
    public class INIFile
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string ApplicationName, string KeyName, string DefaultValue, StringBuilder ReturnString, int nSize, string FileName);

        public static string ReadValue(string SectionName, string KeyName, string FileName)
        {
            StringBuilder szStr = new StringBuilder(255);
            GetPrivateProfileString(SectionName, KeyName, "", szStr, 255, FileName);
            return szStr.ToString().Trim();
        }
    }

    /// <summary>
    /// SQL DM 9.0 (Vineet Kumar) (License Changes) -- Added this class to make serialisation available in different assembly and different type. If we dont want to do this, we will need the dll from custom actions which is not a good way to do.
    /// </summary>
    sealed class InstallInfoDeserializationBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            String currentAssembly = Assembly.GetExecutingAssembly().FullName;
            assemblyName = currentAssembly;

            typeName = typeof(InstallInfo).ToString();
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                  typeName, assemblyName));

            return typeToDeserialize;
        }
    }

    /// <summary>
    /// SQL DM 9.0 (Vineet Kumar) (License Changes) -- This class is used to hold the properties saved in file from installer.
    /// </summary>
    [Serializable]
    public class InstallInfo
    {
        public string CWFHostName { get; set; }
        public string CWFPort { get; set; }
        public string CWFServiceUserName { get; set; }
        public string CWFServicePassword { get; set; }
        public string InstanceName { get; set; }
        public string LicenseKey { get; set; }
    }
}
