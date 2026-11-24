using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace vrsutils
{
    public class EncryptClass
    {
        private readonly byte[] key;

        private readonly byte[] iv;

        public EncryptClass()
        {
            key = new byte[24]
            {
                6, 10, 80, 5, 21, 8, 8, 5, 76, 12,
                24, 45, 3, 31, 54, 1, 30, 81, 4, 17,
                67, 5, 19, 58
            };
            iv = new byte[8] { 28, 0, 32, 63, 54, 27, 41, 50 };
        }

        public byte[] Encrypt(string plainText)
        {
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            byte[] bytes = uTF8Encoding.GetBytes(plainText);
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
            ICryptoTransform transform = tripleDESCryptoServiceProvider.CreateEncryptor(key, iv);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            memoryStream.Position = 0L;
            checked
            {
                byte[] array = new byte[(int)(memoryStream.Length - 1) + 1];
                memoryStream.Read(array, 0, (int)memoryStream.Length);
                cryptoStream.Close();
                return array;
            }
        }

        public string Decrypt(byte[] inputInBytes)
        {
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
            ICryptoTransform transform = tripleDESCryptoServiceProvider.CreateDecryptor(key, iv);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(inputInBytes, 0, inputInBytes.Length);
            cryptoStream.FlushFinalBlock();
            memoryStream.Position = 0L;
            checked
            {
                byte[] array = new byte[(int)(memoryStream.Length - 1) + 1];
                memoryStream.Read(array, 0, (int)memoryStream.Length);
                cryptoStream.Close();
                UTF8Encoding uTF8Encoding2 = new UTF8Encoding();
                return uTF8Encoding2.GetString(array);
            }
        }
    }
}