using System;
using System.Text;
using System.IO;
using PCLCrypto;

namespace fIT.App.Helpers
{
  /// <summary>
  /// The <see cref="Crypto"/> provides an easy way encrypt and decrypt
  /// data using a simple password.
  /// </summary>
  /// <remarks>
  /// Code based on the book "C# 3.0 in a nutshell by Joseph Albahari" (pages 630-632)
  /// and from this StackOverflow post by somebody called Brett
  /// http://stackoverflow.com/questions/202011/encrypt-decrypt-string-in-net/2791259#2791259
  /// </remarks>
  internal static class CryptoHelper
  {
    /// <summary>    
    /// Creates Salt with given length in bytes.    
    /// </summary>    
    /// <param name="lengthInBytes">No. of bytes</param>    
    /// <returns></returns>    
    public static byte[] CreateSalt(int lengthInBytes)
    {
      return WinRTCrypto.CryptographicBuffer.GenerateRandom(lengthInBytes);
    }

    /// <summary>    
    /// Creates a derived key from a comnination     
    /// </summary>    
    /// <param name="password"></param>    
    /// <param name="salt"></param>    
    /// <param name="keyLengthInBytes"></param>    
    /// <param name="iterations"></param>    
    /// <returns></returns>    
    public static byte[] CreateDerivedKey(string password, byte[] salt, int keyLengthInBytes = 32, int iterations = 1000)
    {
      byte[] key = NetFxCrypto.DeriveBytes.GetBytes(password, salt, iterations, keyLengthInBytes);
      return key;
    }

    /// <summary>    
    /// Encrypts given data using symmetric algorithm AES    
    /// </summary>    
    /// <param name="data">Data to encrypt</param>    
    /// <param name="password">Password</param>    
    /// <param name="salt">Salt</param>    
    /// <returns>Encrypted bytes</returns>    
    public static byte[] EncryptAes(string data, string password, byte[] salt)
    {
      var key = CreateDerivedKey(password, salt);

      var aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
      var symetricKey = aes.CreateSymmetricKey(key);
      var bytes = WinRTCrypto.CryptographicEngine.Encrypt(symetricKey, Encoding.UTF8.GetBytes(data));
      return bytes;
    }
    /// <summary>    
    /// Decrypts given bytes using symmetric alogrithm AES    
    /// </summary>    
    /// <param name="data">data to decrypt</param>    
    /// <param name="password">Password used for encryption</param>    
    /// <param name="salt">Salt used for encryption</param>    
    /// <returns></returns>    
    public static string DecryptAes(byte[] data, string password, byte[] salt)
    {
      var key = CreateDerivedKey(password, salt);

      var aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
      var symetricKey = aes.CreateSymmetricKey(key);
      var bytes = WinRTCrypto.CryptographicEngine.Decrypt(symetricKey, data);
      return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
    }
  }
}
