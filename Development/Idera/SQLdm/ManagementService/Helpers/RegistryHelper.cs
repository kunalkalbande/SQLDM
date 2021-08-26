using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
// SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration - Write Registry Keys
namespace Idera.SQLdm.ManagementService.Helpers
{
    internal static class RegistryHelper
    {
        #region Methods

        public static void WriteToRegistry(string path,string keyname,string value)
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(path))
            {                
                key.SetValue(keyname, value);                    
                key.Close();               
            }                                    
        }
        #endregion

    }
}
