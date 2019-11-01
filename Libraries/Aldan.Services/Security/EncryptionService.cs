using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Aldan.Core.Configuration;

namespace Aldan.Services.Security
{
    /// <summary>
    /// Encryption service
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        private AldanConfig _aldanConfig;

        public EncryptionService(AldanConfig aldanConfig)
        {
            _aldanConfig = aldanConfig;
        }

        #region Utilities

        private byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    var toEncrypt = Encoding.Unicode.GetBytes(data);
                    cs.Write(toEncrypt, 0, toEncrypt.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }

        private string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs, Encoding.Unicode))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        public virtual string CreateSaltKey(int size)
        {
            //generate a cryptographic random number
            using (var provider = new RNGCryptoServiceProvider())
            {
                var buff = new byte[size];
                provider.GetBytes(buff);

                // Return a Base64 string representation of the random number
                return Convert.ToBase64String(buff);
            }
        }

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="saltkey">Salk key</param>
        /// <returns>Password hash</returns>
        public virtual string CreatePasswordHash(string password, string saltkey)
        {
            return CreateHash(Encoding.UTF8.GetBytes(string.Concat(password, saltkey)));
        }

        /// <summary>
        /// Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <returns>Data hash</returns>
        public virtual string CreateHash(byte[] data)
        {

            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName("SHA512");
            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name");

            var hashByteArray = algorithm.ComputeHash(data);
            return BitConverter.ToString(hashByteArray).Replace("-", string.Empty);
        }

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Encrypted text</returns>
        public virtual string EncryptText(string plainText, string encryptionPrivateKey = "")
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            if (string.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _aldanConfig.Security.EncryptionKey;

            using (var provider = new TripleDESCryptoServiceProvider())
            {
                provider.Key = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(0, 16));
                provider.IV = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(8, 8));

                var encryptedBinary = EncryptTextToMemory(plainText, provider.Key, provider.IV);
                return Convert.ToBase64String(encryptedBinary);
            }
        }

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Decrypted text</returns>
        public virtual string DecryptText(string cipherText, string encryptionPrivateKey = "")
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            if (string.IsNullOrEmpty(encryptionPrivateKey))
                encryptionPrivateKey = _aldanConfig.Security.EncryptionKey;

            using (var provider = new TripleDESCryptoServiceProvider())
            {
                provider.Key = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(0, 16));
                provider.IV = Encoding.ASCII.GetBytes(encryptionPrivateKey.Substring(8, 8));

                var buffer = Convert.FromBase64String(cipherText);
                return DecryptTextFromMemory(buffer, provider.Key, provider.IV);
            }
        }

        #endregion
    }
}