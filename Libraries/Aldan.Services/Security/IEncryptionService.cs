namespace Aldan.Services.Security
{
    /// <summary>
    /// Encryption service
    /// </summary>
    public interface IEncryptionService 
    {
        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        string CreateSaltKey(int size);

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="saltkey">Salk key</param>
        /// <returns>Password hash</returns>
        string CreatePasswordHash(string password, string saltkey);

        /// <summary>
        /// Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <returns>Data hash</returns>
        string CreateHash(byte [] data);

        /// <summary>
        /// Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Encrypted text</returns>
        string EncryptText(string plainText, string encryptionPrivateKey = "");

        /// <summary>
        /// Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <returns>Decrypted text</returns>
        string DecryptText(string cipherText, string encryptionPrivateKey = "");
    }
}
