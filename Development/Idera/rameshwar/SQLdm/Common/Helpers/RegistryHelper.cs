// ===============================
// AUTHOR       : CWF Team - Gowrish 
// PURPOSE      : Backend Isolation
// TICKET       : SQLDM-29086
// ===============================
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Helpers
{
    public static class RegistryHelper
    {
        public static void SetValueInRegistry(string name, Object value)
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)))
            {
                RegistryKey key = hklm.OpenSubKey(Common.Constants.REGISTRY_PATH, true);
                if (key == null)
                    key = hklm.CreateSubKey(Common.Constants.REGISTRY_PATH);
                key.SetValue(name, value);
            }
        }

        public static Object GetValueFromRegistry(string name)
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)))
            {
                try
                {
                    RegistryKey key = hklm.OpenSubKey(Common.Constants.REGISTRY_PATH, true);
                    return key.GetValue(name);
                }
                catch (Exception e)
                {
                    return String.Empty;
                }
            }
        }

        public static Object GetValueFromRegistry(string name, string path)
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)))
            {
                try
                {
                    RegistryKey key = hklm.OpenSubKey(path, true);
                    return key.GetValue(name);
                }
                catch (Exception e)
                {
                    return String.Empty;
                }
            }
        }

        public static void UpdateMicrosoftRegistryValues(string name, string path, Object value)
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)))
            {
                RegistryKey key = hklm.OpenSubKey(path, true);
                key.SetValue(name, value);
            }
        }

        public static bool CheckMicrosoftRegistryValues(string name, string path)
        {
            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)))
            {
                 RegistryKey key = hklm.OpenSubKey(path, true);
                 if (key.GetValue(name) != null)
                 {
                       return true;
                 }
                return false;
            }
        }
    }
}
