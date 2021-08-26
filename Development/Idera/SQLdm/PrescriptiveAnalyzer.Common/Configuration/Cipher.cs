    //------------------------------------------------------------------------------
    // <copyright file="Cipher.cs" company="Idera, Inc.">
    //     Copyright (c) Idera, Inc. All rights reserved.
    // </copyright>
    //------------------------------------------------------------------------------
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Idera.SQLdoctor.Common.Configuration
{

    public static class Cipher
    {   // Initialization Vector Bytes      garbage          garbage       garbage  B  B  S  I   d   e   r  a   
        private static Guid ID = new Guid(0x35170957, (short)0x1955, (short)0x5AA5, 66, 66, 83, 73, 100, 101, 114, 97);

        /// <summary>
        /// Encrypt a password using the Management Service Instance Name as the password and 
        /// the hard coded GUID as the initialization vector.  Garbage is also added
        /// to the password to prevent the same password from having the same encrypted
        /// value.
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Base64 encoded encryped password</returns>
        public static string EncryptPassword(string instance, string password)
        {
            if (password == null || password.Length == 0 || password.Length > 128)
                return "";

            byte[] cipherBytes = new byte[0];
            byte[] clearBytes = new UTF8Encoding().GetBytes(password);

            // get the key and initialization vector values
            byte[] initializationVector = ID.ToByteArray();
            byte[] instanceName = new UTF8Encoding(false, false).GetBytes(instance);

            using (SymmetricAlgorithm algorithm = Rijndael.Create())
            {
                // we want to use guid as the initialization vector so set blocksize to match
                algorithm.BlockSize = 128;
                instanceName = FixupKey(instanceName, algorithm.LegalKeySizes[0]);

                using (ICryptoTransform encryptor = algorithm.CreateEncryptor(instanceName, initializationVector))
                {
                    using (Stream msEncrypt = new MemoryStream(),
                           csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // add the length of the plain bytes
                        csEncrypt.WriteByte((byte)clearBytes.Length);

                        // append the UTF8 encoded password bytes
                        csEncrypt.Write(clearBytes, 0, clearBytes.Length);

                        // append some garbage
                        byte[] garbage = new byte[8];
                        Random random = new Random();
                        random.NextBytes(garbage);
                        csEncrypt.Write(garbage, 0, garbage.Length);

                        ((CryptoStream)csEncrypt).FlushFinalBlock();

                        cipherBytes = ((MemoryStream)msEncrypt).ToArray();
                    }
                }
            }
            // return the encrypted value as a base64 string 
            return Convert.ToBase64String(cipherBytes);
        }

        private static byte[] FixupKey(byte[] input, KeySizes legalKeySizes)
        {
            int blockSize = legalKeySizes.SkipSize / 8;
            int minKeySize = legalKeySizes.MinSize / 8;
            int maxKeySize = legalKeySizes.MaxSize / 8;

            byte[] output = null;
            int inputLength = input.Length;

            if (input.Length < minKeySize)
            {
                output = new byte[minKeySize];
                input.CopyTo(output, 0);

                for (int i = inputLength; i < output.Length; i++)
                {
                    output[i] = output[i - inputLength];
                }
            }
            else
                if (input.Length > maxKeySize)
                {
                    output = new byte[maxKeySize];
                    for (int i = 0; i < input.Length; i++)
                    {
                        int j = i % maxKeySize;
                        output[j] = output[j] ^= input[i];
                    }
                }
                else
                {
                    if (input.Length % blockSize == 0)
                        return input;

                    int nbytes = (input.Length / blockSize + 1) * blockSize;

                    output = new byte[nbytes];
                    input.CopyTo(output, 0);

                    for (int i = inputLength; i < output.Length; i++)
                    {
                        output[i] = output[i - inputLength];
                    }
                }

            return output;
        }

        /// <summary>
        /// Decrypt a password using the Management Service Instance Name as the password and 
        /// the hard coded GUID as the initialization vector.  
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Plaintext password</returns>
        public static string DecryptPassword(string instance, string password)
        {
            if (String.IsNullOrEmpty(password))
                return "";

            byte[] clearBytes = new byte[0];

            // convert the base64 encoded password to a byte array
            byte[] cipherBytes = Convert.FromBase64String(password);

            // get the key and initialization vector values
            byte[] initializationVector = ID.ToByteArray();
            byte[] instanceName = new UTF8Encoding(false, false).GetBytes(instance);

            using (SymmetricAlgorithm algorithm = Rijndael.Create())
            {
                // we want to use guid as the initialization vector so set blocksize to match
                algorithm.BlockSize = 128;
                instanceName = FixupKey(instanceName, algorithm.LegalKeySizes[0]);

                using (ICryptoTransform decryptor = algorithm.CreateDecryptor(instanceName, initializationVector))
                {
                    // setup a byte stream with the encrypted bytes
                    using (Stream msDecrypt = new MemoryStream(cipherBytes),
                                  csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // read the length of the password bytes
                        int length = csDecrypt.ReadByte();

                        // read the password bytes
                        clearBytes = new byte[length];
                        csDecrypt.Read(clearBytes, 0, length);
                    }
                }
            }

            // return the decrypted value
            return new UTF8Encoding().GetString(clearBytes, 0, clearBytes.Length);
        }
    }
}

