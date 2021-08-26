using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Idera.SQLdm.CollectionService.Helpers
{
    internal static class EncryptedAssemblyLoader
    {
        private static Guid ID = new Guid(0x35170957, (short)0x1955, (short)0x5AA5, 66, 66, 83, 73, 100, 101, 114, 97);
        
        static EncryptedAssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        internal static Assembly Load(string resourceNamespace, string assemblyName)
        {
            string fullName = resourceNamespace;
            if (!fullName.EndsWith("."))
                fullName += ".";
            fullName += assemblyName;

            AssemblyName assName = new AssemblyName(fullName);
            return Assembly.Load(assName);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly me = Assembly.GetExecutingAssembly();

            using (Stream stream = me.GetManifestResourceStream(args.Name))
            {
                if (stream == null) return null;

                try
                {
                    long size = stream.Length;
                    byte[] bytes = new byte[size];
                    stream.Read(bytes, 0, (int)size);

                    using (TripleDESCryptoServiceProvider algorithm = new TripleDESCryptoServiceProvider())
                    {
                        Rfc2898DeriveBytes deriver = new Rfc2898DeriveBytes(ID.ToString(), ID.ToByteArray(), 256);
                        byte[] passwordBytes = deriver.GetBytes(16);
                        byte[] initializationVector = ID.ToByteArray();

                        // we want to use guid as the initialization vector so set blocksize to match
                        using (ICryptoTransform decryptor = algorithm.CreateDecryptor(passwordBytes, initializationVector))
                        {
                            using (Stream msDecrypt = new MemoryStream(bytes), csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] clearBytes = new byte[bytes.Length - decryptor.OutputBlockSize];
                                csDecrypt.Read(clearBytes, 0, clearBytes.Length);
                                return Assembly.Load(clearBytes);
                            }
                        }
                    }

                    return Assembly.Load(bytes);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return null;
        }
    }
}
