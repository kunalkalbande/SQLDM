using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration
{
    [Serializable]
    public sealed class WmiConnectionInfo : ISerializable
    {
        private string _machineName;
        private string _username;
        private string _password;
        private string _authority;

        public string MachineName { get { return (_machineName); } set { _machineName = value; } }

        public string Username { get { return (_username); } set { _username = value; } }

        [XmlIgnore]
        public string Password { internal get { return (_password); } set { _password = value; } }

        public string Authority { get { return (_authority); } set { _authority = value; } }

        public string EncryptedPassword
        {
            get
            {
                if (String.IsNullOrEmpty(_password)) return null;

                byte[] bytes = Encoding.Unicode.GetBytes(_password);
                byte[] protectedPassword = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(protectedPassword);
            }

            set
            {
                if (String.IsNullOrEmpty(value)) _password = String.Empty;
                else
                {
                    byte[] bytes = Convert.FromBase64String(value);
                    byte[] decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
                    _password = Encoding.Unicode.GetString(decrypted);
                }
            }
        }

        public WmiConnectionInfo()
        {
        }

        private WmiConnectionInfo(SerializationInfo info, StreamingContext context)
        {
            MachineName = info.GetString("_machineName");
            Username = info.GetString("_username");
            EncryptedPassword = info.GetString("_password");
            Authority = info.GetString("_authority");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_machineName", _machineName);
            info.AddValue("_username", _username);
            info.AddValue("_password", EncryptedPassword);
            info.AddValue("_authority", _authority);
        }

        public static WmiConnectionInfo DeserializeXml(XmlNode element)
        {
            if (element == null) return null;
            var info = new WmiConnectionInfo();
            var attribute = GetXmlAttributeValue(element, "MachineName");
            if (attribute != null) info.MachineName = attribute.Value;
            attribute = GetXmlAttributeValue(element, "Username");
            if (attribute != null) info.Username = attribute.Value;
            attribute = GetXmlAttributeValue(element, "Password");
            if (attribute != null) info.EncryptedPassword = attribute.Value;
            attribute = GetXmlAttributeValue(element, "Authority");
            if (attribute != null) info.Authority = attribute.Value;
            return (info);
        }

        private static XmlAttribute GetXmlAttributeValue(XmlNode element, string attributeName)
        {
            if (element == null || string.IsNullOrEmpty(attributeName)) return null;

            try
            {
                return element.Attributes[attributeName];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public XmlElement SerializeXml(XmlDocument xmlDocument)
        {
            if (xmlDocument == null) throw new ArgumentNullException("xmlDocument");

            var element = xmlDocument.CreateElement("WmiConnectionInfo");

            var attribute = xmlDocument.CreateAttribute("MachineName");
            attribute.Value = MachineName;
            element.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("Username");
            attribute.Value = Username;
            element.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("Password");
            attribute.Value = EncryptedPassword;
            element.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("Authority");
            attribute.Value = Authority;
            element.Attributes.Append(attribute);

            return (element);
        }

        public WmiConnectionInfo Clone() { return (MemberwiseClone() as WmiConnectionInfo); }
        public override string ToString(){return string.Format("MachineName:{0}  Username:{1}", MachineName, Username);}
    }
}
