//------------------------------------------------------------------------------
// <copyright file="Cipher.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Security.Encryption
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public static class Cipher
    {   // Initialization Vector Bytes      garbage          garbage       garbage  B  B  S  I   d   e   r  a   
        private static Guid ID = new Guid(0x35170957, (short)0x1955, (short)0x5AA5, 66, 66, 83, 73, 100, 101, 114, 97);

        #region DllImport
        [DllImport("Manage.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Crypt(byte[] inBuffer,
                                                    int inSize,
                                                    byte[] outBuffer,
                                                    int outSize,
                                                    byte[] key,
                                                    int keySize,
                                                    byte[] IV,
                                                    bool encrypt);
        #endregion 


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

            int keySize;
            byte[] cipherBytes = new byte[0];
            byte[] clearBytes = new UTF8Encoding().GetBytes(password);

            // get the key and initialization vector values
            byte[] initializationVector = ID.ToByteArray();
            byte[] instanceName = new UTF8Encoding(false, false).GetBytes(instance);
            instanceName = FixupKey(instanceName, out keySize);
            Encryptor crypt = new Encryptor(instanceName, keySize, initializationVector);

            // add the length of the plain bytes
            crypt.WriteByte((byte)clearBytes.Length);

            // append the UTF8 encoded password bytes
            crypt.Write(clearBytes, clearBytes.Length);

            // append some garbage
            byte[] garbage = new byte[8];
            Random random = new Random();
            random.NextBytes(garbage);
            crypt.Write(garbage, garbage.Length);
            cipherBytes = crypt.GetEncryptedData();

            // return the encrypted value as a base64 string 
            return Convert.ToBase64String(cipherBytes);
        }

        private static byte[] FixupKey(byte[] input, out int keySize)
        {
            //We are using AES. the only valid key sizes are 16, 24 and 32 bytes.
            int blockSize = 8; //in bytes
            int minKeySize = 16; //in bytes
            int maxKeySize = 32; //in bytes

            byte[] output = null;
            int inputLength = input.Length;

            if (input.Length < minKeySize)
            {
                keySize = minKeySize;
                output = new byte[minKeySize];
                input.CopyTo(output,0);

                for (int i = inputLength; i < output.Length; i++)
                {
                    output[i] = output[i - inputLength];
                }
            } else
            if (input.Length > maxKeySize)
            {
                keySize = maxKeySize;
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
                {
                    keySize = input.Length;
                    return input;
                }

                int nbytes = (input.Length / blockSize + 1) * blockSize;
                keySize = nbytes;
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

            int keySize;
            byte[] clearBytes = new byte[0];

            // convert the base64 encoded password to a byte array
            byte[] cipherBytes = Convert.FromBase64String(password);

            // get the key and initialization vector values
            byte[] initializationVector = ID.ToByteArray();
            byte[] instanceName = new UTF8Encoding(false, false).GetBytes(instance);
            instanceName = FixupKey(instanceName, out keySize);
            Encryptor crypt = new Encryptor(instanceName, keySize, initializationVector);
            crypt.SetEncryptedData(cipherBytes);

            // read the length of the password bytes
            int length = crypt.ReadByte();

            // read the password bytes
            clearBytes = crypt.Read(length);

            // return the decrypted value
            return new UTF8Encoding().GetString(clearBytes, 0, clearBytes.Length);
        }

        public static byte[] GetSHAHash(string data)
        {
            return BBS.License.BBSLic.GetHash(data);
        }

        private class Encryptor
        {
            private List<byte[]> outputData;
            private byte[] decryptedData;
            private byte[] key;
            private int keySize;
            private byte[] IV;
            private int readOffset;

            public Encryptor(byte[] key, int keySize, byte[] IV)
            {
                this.key = key;
                this.keySize = keySize;
                this.IV = IV;
                readOffset = 0;
                outputData = new List<byte[]>();
            }

            public void WriteByte(byte data)
            {
                byte[] inputData = new byte[1];
                inputData[0] = data;
                this.outputData.Add(inputData);
            }

            public void Write(byte[] data, int size)
            {
                this.outputData.Add(data);
            }

            public byte ReadByte()
            {
                return decryptedData[readOffset++];
            }

            public byte[] Read(int length)
            {
                byte[] output = new byte[length];
                Buffer.BlockCopy(decryptedData, readOffset, output, 0, length);
                readOffset += length;
                return output;
            }

            public byte[] GetEncryptedData()
            {
                byte[] output = null;
                int totalSize = 0;
                int outputOffset = 0;

                //this will append all the data to be encrypted into one buffer before the encryption
                foreach (byte[] data in outputData)
                {
                    totalSize += data.Length;
                }

                //bail out, there is nothing to encrypt
                if (totalSize <= 0)
                    return null;

                output = new byte[totalSize];

                foreach (byte[] data in outputData)
                {
                    Buffer.BlockCopy(data, 0, output, outputOffset, data.Length);
                    outputOffset += data.Length;
                }
                outputData.Clear();
                byte[] outBuffer = new byte[output.Length * 2];

                //Get decrypted array of bytes.
                int outSize = Cipher.Crypt(output, output.Length, outBuffer, outBuffer.Length, key, keySize, IV, true);

                byte[] fromEncrypt = new byte[outSize];
                Buffer.BlockCopy(outBuffer, 0, fromEncrypt, 0, outSize);
                return fromEncrypt;
            }

            public bool SetEncryptedData(byte[] data)
            {
                readOffset = 0;
                byte[] outBuffer = new byte[data.Length * 2];

                //Get decrypted array of bytes.
                int outSize = Cipher.Crypt(data, data.Length, outBuffer, outBuffer.Length, key, keySize, IV, false);

                decryptedData = new byte[outSize];
                Buffer.BlockCopy(outBuffer, 0, decryptedData, 0, outSize);
                return true;
            }
        }
    }
}
