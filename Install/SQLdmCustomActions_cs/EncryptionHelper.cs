using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace CustomActions
{
    public static class EncryptionHelper
    {
        private const string constKey = "SQLdm-Key";
        private const string encryptionKeyFromCWF = "Idera-CWF";

        #region SQLdmSecurityAlgo
        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param name="plaintext">The plaintext.</param>
        /// <returns></returns>
        public static string QuickEncrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return (plaintext);
            }
            string encrypted;
            try
            {
                using (TripleDES algorithm = System.Security.Cryptography.TripleDESCryptoServiceProvider.Create())
                {
                    try
                    {
                        algorithm.Mode = CipherMode.ECB;
                        algorithm.Key = GenerateKey(algorithm);
                        using (ICryptoTransform transform = algorithm.CreateEncryptor())
                        {
                            byte[] buffer = UnicodeEncoding.Unicode.GetBytes(plaintext);
                            buffer = transform.TransformFinalBlock(buffer, 0, buffer.Length);
                            encrypted = Convert.ToBase64String(buffer);
                        }
                    }
                    finally
                    {
                        algorithm.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return (encrypted);
        }

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <returns></returns>
        public static string QuickDecrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return (encryptedText);
            }
            string plaintext;
            using (TripleDES algorithm = System.Security.Cryptography.TripleDESCryptoServiceProvider.Create())
            {
                try
                {
                    algorithm.Mode = CipherMode.ECB;
                    algorithm.Key = GenerateKey(algorithm);
                    // Decrypt encrypted byte buffer and return ASCII string
                    using (ICryptoTransform transform = algorithm.CreateDecryptor())
                    {
                        // Base64 decode and decrypt the encrypted string
                        byte[] buffer = Convert.FromBase64String(encryptedText);
                        buffer = transform.TransformFinalBlock(buffer, 0, buffer.Length);
                        plaintext = UnicodeEncoding.Unicode.GetString(buffer);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    algorithm.Clear();
                }
            }
            return (plaintext);
        }

        /// <summary>
        /// Generates the key.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <returns></returns>
        /// <remarks>
        /// This version of Generate key always returns the password encoded to 192 bits.
        /// Other versions rolling around inside Idera are probably going to return 128 bits 
        /// if the key will fit in 128 bits (16 bytes for the division impaired).
        /// This version matches the helper in java.
        /// </remarks>
        private static byte[] GenerateKey(SymmetricAlgorithm algorithm)
        {
            var sTemp = constKey.PadRight(24, ' ');
            // convert the secret key to byte array
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        #endregion

        #region IderaCWF Security Algos
        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param name="plaintext">The plaintext.</param>
        /// <returns></returns>
        public static string QuickEncryptForCWF(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                return (plaintext);
            }
            string encrypted;
            try
            {
                using (TripleDES algorithm = System.Security.Cryptography.TripleDESCryptoServiceProvider.Create())
                {
                    try
                    {
                        algorithm.Mode = CipherMode.ECB;
                        algorithm.Key = GenerateKeyForCWF(algorithm);
                        using (ICryptoTransform transform = algorithm.CreateEncryptor())
                        {
                            byte[] buffer = UnicodeEncoding.Unicode.GetBytes(plaintext);
                            buffer = transform.TransformFinalBlock(buffer, 0, buffer.Length);
                            encrypted = Convert.ToBase64String(buffer);
                        }
                    }
                    finally
                    {
                        algorithm.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return (encrypted);
        }

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <returns></returns>
        public static string QuickDecryptFromCWF(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return (encryptedText);
            }
            string plaintext;
            using (TripleDES algorithm = System.Security.Cryptography.TripleDESCryptoServiceProvider.Create())
            {
                try
                {
                    algorithm.Mode = CipherMode.ECB;
                    algorithm.Key = GenerateKeyForCWF(algorithm);
                    // Decrypt encrypted byte buffer and return ASCII string
                    using (ICryptoTransform transform = algorithm.CreateDecryptor())
                    {
                        // Base64 decode and decrypt the encrypted string
                        byte[] buffer = Convert.FromBase64String(encryptedText);
                        buffer = transform.TransformFinalBlock(buffer, 0, buffer.Length);
                        plaintext = UnicodeEncoding.Unicode.GetString(buffer);
                    }
                }
                finally
                {
                    algorithm.Clear();
                }
            }
            return (plaintext);
        }

        /// <summary>
        /// Generates the key.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <returns></returns>
        /// <remarks>
        /// This version of Generate key always returns the password encoded to 192 bits.
        /// Other versions rolling around inside Idera are probably going to return 128 bits 
        /// if the key will fit in 128 bits (16 bytes for the division impaired).
        /// This version matches the helper in java.
        /// </remarks>
        private static byte[] GenerateKeyForCWF(SymmetricAlgorithm algorithm)
        {
            var sTemp = encryptionKeyFromCWF.PadRight(24, ' ');
            // convert the secret key to byte array
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        #endregion
    }
}
