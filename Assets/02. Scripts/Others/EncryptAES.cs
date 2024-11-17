using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

public class EncryptAES : MonoBehaviour
{
    public static readonly string key = MD5Hash("12345678901234567890123456789012");
    public static readonly string iv = MD5Hash("1234567890123456");

    const int keySize = 256;
    const int IvSize = 128;

    static string MD5Hash(string str)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(str));

        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in hash)
        {
            stringBuilder.AppendFormat("{0:x2}", b);
        }

        return stringBuilder.ToString();
    }

    public static string Encrypt256(string textToEncrypt)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.KeySize = keySize;
        rijndaelCipher.BlockSize = IvSize;
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;

        rijndaelCipher.Key = Encoding.UTF8.GetBytes(key.Substring(0, 32));
        rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv.Substring(0, 16));

        ICryptoTransform transform = rijndaelCipher.CreateEncryptor(rijndaelCipher.Key, rijndaelCipher.IV);
        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
        byte[] encryptedData = transform.TransformFinalBlock(plainText, 0, plainText.Length);

        return Convert.ToBase64String(encryptedData);
    }

    public static string Decrypt256(string textToDecrypt)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.KeySize = keySize;
        rijndaelCipher.BlockSize = IvSize;
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;

        rijndaelCipher.Key = Encoding.UTF8.GetBytes(key.Substring(0, 32));
        rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv.Substring(0, 16));

        ICryptoTransform transform = rijndaelCipher.CreateDecryptor(rijndaelCipher.Key, rijndaelCipher.IV);
        byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
        byte[] decryptedData = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        return Encoding.UTF8.GetString(decryptedData);
    }
}