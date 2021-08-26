using System.IO;
using System.Reflection;
using System.Security;
using System;
using System.ComponentModel;
using BBS.TracerX;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public static class ApplicationHelper
    {
        private static readonly Logger Log = Logger.GetLogger("ApplicationHelper");

        #region Assembly Attribute Accessors

        public static string AssemblyTitle
        {
            get
            {
                // Get all Title attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at leastCount one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public static string AssemblyDescription
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion

        #region Encryption Helpers

        public static SecureString ConvertStringToSecureString(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return null;
            }

            var secureString = new SecureString();
            char[] plaintextCharArray = plaintext.ToCharArray();

            foreach (char character in plaintextCharArray)
            {
                secureString.AppendChar(character);
            }

            return secureString;
        }

        #endregion

        #region Enum Helpers

        internal static string GetEnumDescription(object o)
        {
            Type otype = o.GetType();

            if (otype.IsEnum)
            {
                FieldInfo field = otype.GetField(Enum.GetName(otype, o));
                if (field != null)
                {
                    object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attributes.Length > 0)
                        return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            return o.ToString();
        }


        #endregion

        #region Serialization Helpers
        public static byte[] Serialize(object value, bool useCompression)
        {
            using (Log.DebugCall(string.Format("ApplicationHelper.Serialize({0})", value)))
            {
                if (useCompression)
                    return SerializeCompressed(value);

                byte[] bytes = null;
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, value);
                    bytes = stream.ToArray();
                }
                return bytes;
            }
        }

        public static byte[] SerializeCompressed(object value)
        {
            using (Log.DebugCall(string.Format("ApplicationHelper.SerializeCompressed({0})", value)))
            {
                byte[] bytes = null;

                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    DeflateStream deflater = new DeflateStream(stream, CompressionMode.Compress, true);
                    BufferedStream bufferedStream = new BufferedStream(deflater);

                    formatter.Serialize(bufferedStream, value);

                    bufferedStream.Close();
                    deflater.Close();

                    bytes = stream.ToArray();
                }
                return bytes;
            }
        }

        public static object Deserialize(byte[] serializedValue, bool useCompression)
        {
            using (Log.DebugCall("ApplicationHelper.Deserialize(this byte[] serializedValue, bool useCompression)"))
            {
                if (useCompression)
                    return DeserializeCompressed(serializedValue);

                object result = null;

                if (serializedValue == null)
                    return result;
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(serializedValue))
                {
                    Log.Debug("Deserialize from memory stream");
                    result = formatter.Deserialize(stream);
                }

                return result;
            }
        }

        public static object DeserializeCompressed(byte[] serializedValue)
        {
            using (Log.DebugCall("ApplicationHelper.DeserializeCompressed(byte[] serializedValue)"))
            {
                object result = null;
                if (serializedValue == null)
                    return result;

                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(serializedValue))
                {
                    using (DeflateStream inflater = new DeflateStream(stream, CompressionMode.Decompress))
                    {
                        Log.Debug("Deserialize from compressed memory stream");
                        result = formatter.Deserialize(inflater);
                    }
                }
                return result;
            }
        }

        #endregion

        #region Embedded Resources

        public static string GetEmbededResource(Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    //If you're coming here because you are getting this exception, you probably need to change the build 
                    //action of your resource file to Embedded Resource under Properties.  
                    throw new ApplicationException("The embedded resource was not found: " + resourceName);
                }
                try
                {

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to read embedded resource " + resourceName, e);
                }
            }
        }

        #endregion
    }
}