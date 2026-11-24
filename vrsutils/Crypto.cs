#nullable enable

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace vrsutils;

public class Crypto  //public sealed class Crypto
{
    private const int DefaultKeySize = 128;
    private const int AesKeyLength = 16;
    private static readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    public byte[] AESdecrypt(byte[] cipherText, byte[] key, byte[] initialVector)
    {
        return TransformAes(cipherText, key, initialVector, encrypt: false);
    }

    public byte[] AESencrypt(byte[] plainText, byte[] key, byte[] initialVector)
    {
        return TransformAes(plainText, key, initialVector, encrypt: true);
    }

    public string AESencrypt(string plainText, string secureKey, string initializationVector)
    {
        byte[] ciphertext = AESencrypt(
            Utf8.GetBytes(plainText),
            GetKeyBytes(secureKey),
            GetKeyBytes(initializationVector));
        return Convert.ToBase64String(ciphertext);
    }

    public string AESdecrypt(string encryptedText, string secureKey, string initializationVector)
    {
        byte[] plaintext = AESdecrypt(
            DecodeBase64(encryptedText),
            GetKeyBytes(secureKey),
            GetKeyBytes(initializationVector));
        return Utf8.GetString(plaintext);
    }

    public string RijndaelEncrypt(
        string plainText,
        string secureKey,
        string initializationVector,
        int keySize)
    {
        byte[] ciphertext = RijndaelTransform(
            Utf8.GetBytes(plainText),
            Utf8.GetBytes(secureKey),
            Utf8.GetBytes(initializationVector),
            keySize,
            encrypt: true);
        return Convert.ToBase64String(ciphertext);
    }

    public string RijndaelEncrypt(
        string plainText,
        byte[] secureKey,
        string initializationVector,
        int keySize)
    {
        byte[] ciphertext = RijndaelTransform(
            Utf8.GetBytes(plainText),
            secureKey,
            Utf8.GetBytes(initializationVector),
            keySize,
            encrypt: true);
        return Convert.ToBase64String(ciphertext);
    }

    public string RijndaelDecrypt(
        string base64Text,
        string secureKey,
        string initializationVector,
        int keySize)
    {
        if (string.IsNullOrWhiteSpace(base64Text))
        {
            return string.Empty;
        }

        byte[] plaintext = RijndaelTransform(
            DecodeBase64(base64Text),
            Utf8.GetBytes(secureKey),
            Utf8.GetBytes(initializationVector),
            keySize,
            encrypt: false);
        return JavaTrim(Utf8.GetString(plaintext));
    }

    public string RijndaelDecrypt(
        string base64Text,
        byte[] secureKey,
        string initializationVector,
        int keySize)
    {
        byte[] plaintext = RijndaelTransform(
            DecodeBase64(base64Text),
            secureKey,
            Utf8.GetBytes(initializationVector),
            keySize,
            encrypt: false);
        return JavaTrim(Utf8.GetString(plaintext));
    }

    public byte[] RijndaelEncrypt(
        byte[] byteArr,
        string secureKey,
        string initializationVector,
        int keySize)
    {
        return RijndaelTransform(
            byteArr,
            Utf8.GetBytes(secureKey),
            Utf8.GetBytes(initializationVector),
            keySize,
            encrypt: true);
    }

    public byte[] RijndaelEncrypt(
        byte[] byteArr,
        byte[] secureKey,
        string initializationVector)
    {
        return RijndaelTransform(
            byteArr,
            secureKey,
            Utf8.GetBytes(initializationVector),
            256,
            encrypt: true);
    }

    public byte[] RijndaelDecrypt(
        byte[] byteArr,
        string secureKey,
        string initializationVector,
        int keySize)
    {
        return RijndaelTransform(
            byteArr,
            Utf8.GetBytes(secureKey),
            Utf8.GetBytes(initializationVector),
            keySize,
            encrypt: false);
    }

    public byte[] RijndaelDecrypt(
        byte[] byteArr,
        byte[] secureKey,
        string initializationVector)
    {
        return RijndaelTransform(
            byteArr,
            secureKey,
            Utf8.GetBytes(initializationVector),
            DefaultKeySize,
            encrypt: false);
    }

    public string RijndaelDecryptOld(string base64Text, string secureKey, string initializationVector)
    {
        byte[] plaintext = RijndaelTransform(
            DecodeBase64(base64Text),
            Utf8.GetBytes(secureKey),
            Utf8.GetBytes(initializationVector),
            DefaultKeySize,
            encrypt: false);
        return JavaTrim(Utf8.GetString(plaintext));
    }

    public string RijndaelEncryptOld(string plainText, string secureKey, string initializationVector)
    {
        byte[] ciphertext = RijndaelTransform(
            Utf8.GetBytes(plainText),
            Utf8.GetBytes(secureKey),
            Utf8.GetBytes(initializationVector),
            DefaultKeySize,
            encrypt: true);
        return Convert.ToBase64String(ciphertext);
    }

    public byte[] RijndaelDecryptOld(byte[] byteArr, string secureKey, string initializationVector)
    {
        return RijndaelTransform(
            byteArr,
            Utf8.GetBytes(secureKey),
            Utf8.GetBytes(initializationVector),
            DefaultKeySize,
            encrypt: false);
    }

    public byte[] RijndaelEncryptOld(byte[] byteArr, string secureKey, string initializationVector)
    {
        return RijndaelTransform(
            byteArr,
            Utf8.GetBytes(secureKey),
            Utf8.GetBytes(initializationVector),
            DefaultKeySize,
            encrypt: true);
    }

    public void DecryptFileToPath(
        FileInfo inputFile,
        FileInfo outputFile,
        string secureKey,
        string vectorKey,
        int keySize)
    {
        ArgumentNullException.ThrowIfNull(inputFile);
        ArgumentNullException.ThrowIfNull(outputFile);
        byte[] inputBytes = File.ReadAllBytes(inputFile.FullName);
        byte[] outputBytes = RijndaelDecrypt(inputBytes, secureKey, vectorKey, keySize);
        File.WriteAllBytes(outputFile.FullName, outputBytes);
    }

    public void EncryptFileToPath(
        FileInfo inputFile,
        FileInfo outputFile,
        string secureKey,
        string vectorKey,
        int keySize)
    {
        ArgumentNullException.ThrowIfNull(inputFile);
        ArgumentNullException.ThrowIfNull(outputFile);
        byte[] inputBytes = File.ReadAllBytes(inputFile.FullName);
        byte[] outputBytes = RijndaelEncrypt(inputBytes, secureKey, vectorKey, keySize);
        File.WriteAllBytes(outputFile.FullName, outputBytes);

        string inputFileNameNoExtension = Path.GetFileNameWithoutExtension(inputFile.Name);
        string outputDirectory = outputFile.DirectoryName
            ?? throw new IOException("The output file must have a parent directory.");

        var keysFile = new FileInfo(Path.Combine(
            outputDirectory,
            $"KEYS_{inputFileNameNoExtension}.txt"));
        GenerateKeysParamTxtFile(keysFile, secureKey, vectorKey, keySize);

        var hashFile = new FileInfo(Path.Combine(
            outputDirectory,
            $"HASH_{inputFileNameNoExtension}.txt"));
        GenHashToFile("MD5", outputFile, hashFile);
    }

    public void GenerateKeysParamTxtFile(
        FileInfo txtFile,
        string secureKey,
        string initializationVector,
        int keySize)
    {
        ArgumentNullException.ThrowIfNull(txtFile);
        using var writer = new StreamWriter(txtFile.FullName, append: false, Utf8);
        writer.WriteLine($"Secret Key: {secureKey}");
        writer.WriteLine($"Initialization Vector: {initializationVector}");
        writer.Write($"Key Size: {keySize}");
    }

    public char[] GenKey(int length)
    {
        if (length < 4)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Key length must be at least four characters.");
        }

        const string upperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
        const string specialCharacters = "!@#$*%?";
        const string numbers = "1234567890";
        string allCharacters = upperCaseLetters + lowerCaseLetters + specialCharacters + numbers;

        var password = new char[length];
        password[0] = lowerCaseLetters[RandomNumberGenerator.GetInt32(lowerCaseLetters.Length)];
        password[1] = upperCaseLetters[RandomNumberGenerator.GetInt32(upperCaseLetters.Length)];
        password[2] = specialCharacters[RandomNumberGenerator.GetInt32(specialCharacters.Length)];
        password[3] = numbers[RandomNumberGenerator.GetInt32(numbers.Length)];
        for (int index = 4; index < password.Length; index++)
        {
            password[index] = allCharacters[RandomNumberGenerator.GetInt32(allCharacters.Length)];
        }

        return password;
    }

    public string GetFileChecksum(string strDigest, FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);
        using HashAlgorithm algorithm = CreateDigestAlgorithm(strDigest);
        using FileStream stream = file.OpenRead();
        return Convert.ToHexString(algorithm.ComputeHash(stream)).ToLowerInvariant();
    }

    public void GenHashToFile(string strDigest, FileInfo inputFile, FileInfo txtFile)
    {
        ArgumentNullException.ThrowIfNull(inputFile);
        ArgumentNullException.ThrowIfNull(txtFile);
        string hash = GetFileChecksum(strDigest, inputFile);
        File.WriteAllText(txtFile.FullName, $"{strDigest} Hash: {hash}", Utf8);
    }

    private static byte[] TransformAes(byte[] input, byte[] key, byte[] iv, bool encrypt)
    {
        ArgumentNullException.ThrowIfNull(input);
        using Aes aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key;
        aes.IV = iv;

        using ICryptoTransform transform = encrypt ? aes.CreateEncryptor() : aes.CreateDecryptor();
        return transform.TransformFinalBlock(input, 0, input.Length);
    }

    private static byte[] RijndaelTransform(
        byte[] input,
        byte[] key,
        byte[] iv,
        int blockSize,
        bool encrypt)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(iv);

        var cipher = new PaddedBufferedBlockCipher(
            new CbcBlockCipher(new RijndaelEngine(blockSize)),
            new ZeroBytePadding());
        cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(key), iv));

        byte[] output = new byte[cipher.GetOutputSize(input.Length)];
        int outputLength = cipher.ProcessBytes(input, 0, input.Length, output, 0);
        outputLength += cipher.DoFinal(output, outputLength);
        return outputLength == output.Length ? output : output[..outputLength];
    }

    private static byte[] GetKeyBytes(string value)
    {
        byte[] source = Utf8.GetBytes(value);
        byte[] key = new byte[AesKeyLength];
        Array.Copy(source, key, Math.Min(source.Length, key.Length));
        return key;
    }

    private static byte[] DecodeBase64(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Convert.FromBase64String(value);
    }

    private static HashAlgorithm CreateDigestAlgorithm(string algorithmName)
    {
        string normalizedName = algorithmName.Replace("-", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();

        return normalizedName switch
        {
            "MD5" => MD5.Create(),
            "SHA1" => SHA1.Create(),
            "SHA256" => SHA256.Create(),
            "SHA384" => SHA384.Create(),
            "SHA512" => SHA512.Create(),
            _ => throw new CryptographicException($"Unsupported digest algorithm '{algorithmName}'.")
        };
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
}
