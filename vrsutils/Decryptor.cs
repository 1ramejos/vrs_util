#nullable enable

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using static vrsutils.TiffDecoder;

namespace vrsutils;

public class Decryptor
{
    private const string InitVector = "\u00a4\u2022\u00a6\u2013\u00c7\u2014\u00a8\u02dc\u00c9\u2122\u00aa\u0161\u00cb\u203a\u00ac\u0153";
    private const string CharacterEncoding = "UTF-8";
    private const string SkSecretKey = "yc5VXTdfnSvZ2rvfN3j82UrLhYo5MCZj";
    private const string SkIv = "Q1wQi7p44YlUiJHc";
    private const string SnpdatIv = "i[T%6|!}LW`|RJ9F";

    private static readonly byte[] SnpdatSecretKey =
    {
        123, 137, 158, 132, 175, 38, 57, 36, 91, 157, 58, 33, 39, 53, 192, 65,
        48, 171, 231, 80, 136, 96, 101, 10, 17, 242, 10, 231, 122, 124, 130, 78
    };

    public static string? RijndaelDecryptVRS(string cipherString)
    {
        byte[] iv = Encoding.ASCII.GetBytes(InitVector);
        byte[] ciphertext = DecodeBase64(cipherString);
        byte[] keyBytes =
        {
            17, 59, 19, unchecked((byte)-41), 109, unchecked((byte)-67),
            unchecked((byte)-59), 30, unchecked((byte)-94), unchecked((byte)-121),
            55, 54, unchecked((byte)-25), 74, 71, unchecked((byte)-44)
        };

        try
        {
            byte[] cipherBytes = Decrypt(ciphertext, keyBytes, iv, 128, new Pkcs7Padding());
            return JavaTrim(Encoding.GetEncoding(CharacterEncoding).GetString(cipherBytes));
        }
        catch (Exception exception) when (IsCipherException(exception))
        {
            Console.WriteLine(exception);
            return null;
        }
    }

    public static string RijndaelDecrypt(string base64Text, string secureKey, string initializationVector, int keySize)
    {
        byte[] toDecrypt = DecodeBase64(base64Text);

        try
        {
            byte[] cipherBytes = Decrypt(
                toDecrypt,
                Encoding.GetEncoding(CharacterEncoding).GetBytes(secureKey),
                Encoding.GetEncoding(CharacterEncoding).GetBytes(initializationVector),
                keySize,
                new ZeroBytePadding());
            return JavaTrim(Encoding.GetEncoding(CharacterEncoding).GetString(cipherBytes));
        }
        catch (Exception exception) when (IsCipherException(exception))
        {
            Console.WriteLine(exception);
            return string.Empty;
        }
    }

    public static string RijndaelDecryptSnpdat(string base64Text)
    {
        return RijndaelDecrypt(base64Text, SnpdatSecretKey, Encoding.GetEncoding(CharacterEncoding).GetBytes(SnpdatIv), 128);
    }

    public static string RijndaelDecryptSK(string base64Text)
    {
        return RijndaelDecrypt(
            base64Text,
            Encoding.GetEncoding(CharacterEncoding).GetBytes(SkSecretKey),
            Encoding.GetEncoding(CharacterEncoding).GetBytes(SkIv),
            128);
    }

    public static string RijndaelDecrypt(string base64Text, byte[] sessionKey, byte[] iv, int keySize)
    {
        byte[] toDecrypt = DecodeBase64(base64Text);

        try
        {
            // The Java overload always constructs a 128-bit-block Rijndael engine.
            byte[] cipherBytes = Decrypt(toDecrypt, sessionKey, iv, 128, new ZeroBytePadding());
            return JavaTrim(Encoding.GetEncoding(CharacterEncoding).GetString(cipherBytes));
        }
        catch (Exception exception) when (IsCipherException(exception))
        {
            Console.WriteLine(exception);
            return string.Empty;
        }
    }

    public static Stream? RijndaelDecryptBlobSnpdat(Stream stream)
    {
        return RijndaelDecryptBlob(stream, SnpdatSecretKey, Encoding.GetEncoding(CharacterEncoding).GetBytes(SnpdatIv), 128);
    }

    public static Stream? RijndaelDecryptBlobSK(Stream stream)
    {
        return RijndaelDecryptBlob(
            stream,
            Encoding.GetEncoding(CharacterEncoding).GetBytes(SkSecretKey),
            Encoding.GetEncoding(CharacterEncoding).GetBytes(SkIv),
            128);
    }

    public static Stream? RijndaelDecryptBlobString(Stream stream, string sessionKey, string iv, int keySize)
    {
        return RijndaelDecryptBlob(
            stream,
            Encoding.GetEncoding(CharacterEncoding).GetBytes(sessionKey),
            Encoding.GetEncoding(CharacterEncoding).GetBytes(iv),
            256);
    }

    public static byte[] TestByteArray()
    {
        return Encoding.GetEncoding(CharacterEncoding).GetBytes("TEST BYTE ARRAY");
    }

    public static Stream? RijndaelDecryptBlob(Stream stream, byte[] sessionKey, byte[] iv, int keySize)
    {
        using var encryptedStream = new MemoryStream();
        stream.CopyTo(encryptedStream);
        byte[] byteArr = encryptedStream.ToArray();

        try
        {
            byte[] cipherBytes = Decrypt(byteArr, sessionKey, iv, keySize, new ZeroBytePadding());
            if (cipherBytes.Length < 1000)
            {
                if (!OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    throw new PlatformNotSupportedException("TIFF-to-JPEG conversion requires Windows 6.1 or later.");
                }

                return TiffDecoder.ConvertDecryptedImageToJpegStream(cipherBytes);
            }

            return new MemoryStream(cipherBytes, writable: false);
        }
        catch (Exception exception) when (IsCipherException(exception))
        {
            Console.WriteLine($"Error in Decryption to java {exception}");
            return null;
        }
    }

    public static string HashcodeGenerator(string value)
    {
        byte[] digest = MD5.HashData(Encoding.GetEncoding(CharacterEncoding).GetBytes(value));
        return new BigInteger(digest, isUnsigned: true, isBigEndian: true).ToString("x", CultureInfo.InvariantCulture);
    }

    public static int GetAge(string birthDate, string electionDate)
    {
        const string format = "MM-dd-yyyy";
        if (!DateTime.TryParseExact(birthDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob) ||
            !DateTime.TryParseExact(electionDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime election))
        {
            return 0;
        }

        int age = election.Year - dob.Year;
        if (dob.Month > election.Month || dob.Month == election.Month && dob.Day > election.Day)
        {
            age--;
        }

        return age;
    }

    internal static byte[] DecryptWithZeroPadding(byte[] input, byte[] sessionKey, byte[] iv, int blockSize)
    {
        return Decrypt(input, sessionKey, iv, blockSize, new ZeroBytePadding());
    }

    private static byte[] Decrypt(
        byte[] input,
        byte[] sessionKey,
        byte[] iv,
        int blockSize,
        IBlockCipherPadding padding)
    {
        var cipher = new PaddedBufferedBlockCipher(
            new CbcBlockCipher(new RijndaelEngine(blockSize)),
            padding);
        cipher.Init(false, new ParametersWithIV(new KeyParameter(sessionKey), iv));

        byte[] output = new byte[cipher.GetOutputSize(input.Length)];
        int outputLength = cipher.ProcessBytes(input, 0, input.Length, output, 0);
        cipher.DoFinal(output, outputLength);
        return output;
    }

    private static byte[] DecodeBase64(string value)
    {
        var filtered = new StringBuilder(value.Length);
        foreach (char character in value)
        {
            if (character >= 'A' && character <= 'Z' ||
                character >= 'a' && character <= 'z' ||
                character >= '0' && character <= '9' ||
                character == '+' || character == '/' || character == '=')
            {
                filtered.Append(character);
            }
        }

        string normalized = filtered.ToString();
        int remainder = normalized.Length % 4;
        if (remainder == 1)
        {
            throw new FormatException("Invalid Base64 input.");
        }

        if (remainder != 0)
        {
            normalized = normalized.PadRight(normalized.Length + (4 - remainder), '=');
        }

        return Convert.FromBase64String(normalized);
    }

    private static string JavaTrim(string value)
    {
        int start = 0;
        int end = value.Length;
        while (start < end && value[start] <= '\u0020')
        {
            start++;
        }

        while (end > start && value[end - 1] <= '\u0020')
        {
            end--;
        }

        return value[start..end];
    }

    private static bool IsCipherException(Exception exception)
    {
        return exception is ArgumentException
            or InvalidOperationException
            or DataLengthException
            or InvalidCipherTextException;
    }
}
