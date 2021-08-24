using System;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SQLdmCWFInstaller
{
    /// <summary>
    /// SQLdm 9.0 (Ankit Srivastava) - CWF Installer Wrapper - Created New helper class to check SQLdm installation status
    /// </summary>
    class SQLdmHelper
    {
        #region Fields
        //private static readonly Logger Log = Logger.GetLogger("SQLdmCWFInstaller_SQLdmHelper");
        internal static readonly int[] GuidRegistryFormatPattern = new[] { 8, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2 }; 
        #endregion

        /// <summary>
        /// this method checks if the given productcode/upgradecode exists or not 
        /// </summary>
        /// <param name="upgradeCode"></param>
        /// <returns></returns>
        public static string[] GetProductCodes(Guid upgradeCode,bool is64Bit)
        {
            try
            {
                // Convert the code to the format found in the registry 
                var upgradeCodeSearchString = Reverse(upgradeCode, GuidRegistryFormatPattern);

                RegistryKey hklm;//Declare the Registry Key Object
                //Get the Registry Key object
                if (is64Bit)
                    hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    hklm = Registry.LocalMachine;

                // Open the upgrade code registry key
                var upgradeCodeRegistryRoot = hklm.OpenSubKey(Constants.UpgradeCodeRegistryKey);

                if (upgradeCodeRegistryRoot == null)
                    return null;

                // Iterate over each sub-key
                foreach (var subKeyName in upgradeCodeRegistryRoot.GetSubKeyNames())
                {
                    var subkey = upgradeCodeRegistryRoot.OpenSubKey(subKeyName);

                    if (subkey == null)
                        continue;

                    // Check for a value containing the input code(product or upgrade)
                    if (subkey.Name.IndexOf(upgradeCodeSearchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return subkey.GetValueNames();
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Error("Exception thrown in IfCodeExists: ", ex);
            }
            return null;
        }

        /// <summary>
        /// This method reverses the given GUID to the pattern given
        /// </summary>
        /// <param name="value">GUID</param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        internal static string Reverse(object value, params int[] pattern)
        {
            var returnString = new StringBuilder();

            try
            {
                // Strip the hyphens
                var inputString = value.ToString().Replace("-", "");
                var index = 0;

                // Iterate over the reversal pattern
                foreach (var length in pattern)
                {
                    // Reverse the sub-string and append it
                    returnString.Append(inputString.Substring(index, length).Reverse().ToArray());

                    // Increment our posistion in the string
                    index += length;
                }
            }
            catch (Exception ex)
            {
                //Log.Error("Exception thrown in Reverse: ", ex);
            }

            return returnString.ToString();
        }
    }
}

