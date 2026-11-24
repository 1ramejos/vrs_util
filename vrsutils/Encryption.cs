/*using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace vrsutils
{
    public class Encryption
    {
        private RijndaelManaged alg;

        private readonly UTF8Encoding textConverter;

        private readonly string secureKey;

        private readonly string vectorKey;

        private readonly int keySize;

        /*   private readonly byte[] Key;

           private readonly byte[] IV;

           private readonly byte[] bytes;

           private readonly byte[] bytes2;*/

        public Encryption()
        {
            textConverter = new UTF8Encoding();
            alg = new RijndaelManaged();
            secureKey = "rhLQiArvwFU0LyfkUa0HpEq50XGGLGka";
            vectorKey = "UmrizBLbPCYSkJXmv9CrwFW3KBTm2VCu";
            keySize = 256;

            /*    Key = Encoding.UTF8.GetBytes(secureKey);
                IV = Encoding.UTF8.GetBytes(vectorKey);
                bytes = Encoding.UTF8.GetBytes(secureKey);
                bytes2 = Encoding.UTF8.GetBytes(vectorKey);*/
        }

        public string CreateEncryptedString(string myString)
        {
            alg = new RijndaelManaged();
            alg.Padding = PaddingMode.Zeros;
            alg.Mode = CipherMode.CBC;
            alg.BlockSize = keySize;
            alg.KeySize = keySize;
            alg.Key = Encoding.UTF8.GetBytes(secureKey);
            alg.IV = Encoding.UTF8.GetBytes(vectorKey);
            ICryptoTransform transform = alg.CreateEncryptor(alg.Key, alg.IV);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            StreamWriter streamWriter = new StreamWriter(cryptoStream);
            streamWriter.Write(myString);
            streamWriter.Flush();
            cryptoStream.FlushFinalBlock();
            byte[] array = memoryStream.ToArray();
            memoryStream.Position = 0L;
            memoryStream.Read(array, 0, checked((int)memoryStream.Length));
            return Convert.ToBase64String(array);

            /*  var engine = new RijndaelEngine(256);
              CbcBlockCipher blockCipher = new CbcBlockCipher(engine);
              var cipher = new PaddedBufferedBlockCipher(blockCipher, new ZeroBytePadding());
              cipher.Init(true, new ParametersWithIV(new KeyParameter(Key), IV));
              byte[] input = Encoding.UTF8.GetBytes(myString);
              var res = cipher.DoFinal(input);
              return Convert.ToBase64String(res);*/
        }

        public string DecryptString(string EncText)
        {
            string result;
            try
            {
                byte[] buffer = Convert.FromBase64String(EncText);
                byte[] bytes = Encoding.UTF8.GetBytes(secureKey);
                byte[] bytes2 = Encoding.UTF8.GetBytes(vectorKey);
                string text = "";
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
                {
                    rijndaelManaged.BlockSize = keySize;
                    rijndaelManaged.KeySize = keySize;
                    rijndaelManaged.Key = bytes;
                    rijndaelManaged.IV = bytes2;
                    rijndaelManaged.Padding = PaddingMode.Zeros;
                    rijndaelManaged.Mode = CipherMode.CBC;
                    ICryptoTransform transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                    MemoryStream stream = new MemoryStream(buffer);
                    CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
                    StreamReader streamReader = new StreamReader(stream2);
                    text = streamReader.ReadToEnd().TrimEnd('/', '\0', ' ');
                }
                result = text;
            }
            catch// (Exception ex)
            {
                result = EncText;
            }
            return result;

            /* byte[] buffer = Convert.FromBase64String(EncText);
             var engine = new RijndaelEngine(256);
             CbcBlockCipher blockCipher = new CbcBlockCipher(engine);
             var cipher = new PaddedBufferedBlockCipher(blockCipher, new ZeroBytePadding());
             cipher.Init(false, new ParametersWithIV(new KeyParameter(bytes), bytes2));
             var res = cipher.DoFinal(buffer);
             return Encoding.UTF8.GetString(res).ToString().TrimEnd('/', '\0', ' ');*/
        }

        public byte[] EncryptByte(byte[] byteArr)
        {
            alg = new RijndaelManaged();
            alg.Padding = PaddingMode.Zeros;
            alg.Mode = CipherMode.CBC;
            alg.BlockSize = keySize;
            alg.KeySize = keySize;
            alg.Key = Encoding.UTF8.GetBytes(secureKey);
            alg.IV = Encoding.UTF8.GetBytes(vectorKey);
            ICryptoTransform transform = alg.CreateEncryptor(alg.Key, alg.IV);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
            cryptoStream.Write(byteArr, 0, byteArr.Length);
            cryptoStream.FlushFinalBlock();
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return result;
        }

        public byte[] DecryptByte(byte[] byteArr)
        {
            alg = new RijndaelManaged();
            alg.Padding = PaddingMode.Zeros;
            alg.Mode = CipherMode.CBC;
            alg.BlockSize = keySize;
            alg.KeySize = keySize;
            alg.Key = Encoding.UTF8.GetBytes(secureKey);
            alg.IV = Encoding.UTF8.GetBytes(vectorKey);
            byte[] result;
            try
            {
                ICryptoTransform transform = alg.CreateDecryptor(alg.Key, alg.IV);
                MemoryStream memoryStream = new MemoryStream(byteArr);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
                byte[] array = new byte[checked(byteArr.Length + 1)];
                int num = cryptoStream.Read(array, 0, array.Length);
                memoryStream.Close();
                cryptoStream.Close();
                result = array;
            }
            catch //(Exception ex)
            {
                result = byteArr;
            }
            return result;
        }
    }
}