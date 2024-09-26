using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
  public static string TripleDESEncryptor(string data, string secretKey)
  {
    try
    {
      const int blockSize = 8;

      // Calculate padding needed for PKCS7
      int padDiff = blockSize - (data.Length % blockSize);
      string paddedText = data + new string((char)padDiff, padDiff);

      // Convert key to bytes and ensure it's exactly 24 bytes (TripleDES requires 192-bit key)
      byte[] encryptionKeyBytes = Encoding.UTF8.GetBytes(secretKey);
      if (encryptionKeyBytes.Length != 24)
      {
        Array.Resize(ref encryptionKeyBytes, 24); // Resize the key to fit 24 bytes for TripleDES
      }

      if (encryptionKeyBytes.Length != 24)
      {
        Array.Resize(ref encryptionKeyBytes, 24);
      }

      if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(secretKey))
      {
        using (var tripleDes = TripleDES.Create())
        {
          tripleDes.Key = encryptionKeyBytes;
          tripleDes.Mode = CipherMode.ECB;
          tripleDes.Padding = PaddingMode.PKCS7;

          byte[] dataBytes = Encoding.UTF8.GetBytes(data);

          var encryptor = tripleDes.CreateEncryptor();
          var encryptedData = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

          return Convert.ToBase64String(encryptedData);
        }
      }

      return string.Empty;
    }
    catch (Exception ex)
    {
      return ex.Message;
    }
  }

  public static string TripleDESDecryptor(string encryptedText, string key)
  {
    try
    {
      // Triple DES decryption settings
      using (var tdes = TripleDES.Create())
      {
        tdes.Key = Encoding.UTF8.GetBytes(key.Substring(0, 24)); // Ensure the key is exactly 24 bytes
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;

        // Convert the Base64 encrypted text to byte array
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

        // Decrypt the data
        ICryptoTransform decryptor = tdes.CreateDecryptor();
        byte[] decryptedBytes = decryptor.TransformFinalBlock(
          encryptedBytes,
          0,
          encryptedBytes.Length
        );

        // Convert decrypted bytes to string
        return Encoding.UTF8.GetString(decryptedBytes);
      }
    }
    catch (Exception ex)
    {
      return "Error: " + ex.Message;
    }
  }

  public static string GetHmacSha256Hash(string data, string secretKey)
  {
    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
    {
      byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

      return Convert.ToHexString(hashBytes).ToLower(); // Convert to lowercase hex string
    }
  }
}
