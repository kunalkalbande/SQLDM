using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common;
using Idera.SQLdm.CWFRegister.Helpers;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Security.Encryption;

namespace Idera.SQLdm.CWFRegister.Objects
{
    class CWFDetails
    {
        public int ProductID { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string RESTuri { get; set; }

        public CWFDetails()
        {
            ProductID = Constants.NOT_REGISTERED;
            Port = 0;
        }

        public string BaseURL
        {
            get { return "http://" + HostName + ":" + Port; }
        }

        /// <summary>
        /// returns a decrypted password if its encrypted, else the password
        /// </summary>
        public static string DecryptPassword(string encrypted)
        {
            string password = encrypted;
            try
            {
                password = EncryptionHelper.QuickDecrypt(encrypted);

            }
            catch
            {

            }

            return password;
        }

        public static string EncryptPassword(string encrypted)
        {
            string password = encrypted;
            try
            {
                password = EncryptionHelper.QuickEncrypt(encrypted);

            }
            catch
            {

            }

            return password;
        }
    }
}
