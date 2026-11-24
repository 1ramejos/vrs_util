#nullable enable

using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
//using ph.gov.comelec.Utilities;

namespace vrsutils;

public class CryptoUtils
{
    private const string Algorithm = "AES";
    private const string Transformation = "AES/CBC/PKCS7Padding";
    public static readonly byte[] PdfSalt =
    {
        24, 1, 14, 3, 233, 200, 193, 167, 24, 1, 14, 3, 233, 200, 193, 167
    };

    public static string ModulusString =
        "sBDo1J8GJuPTB4f89qv+8jrrskLoB+iGhN3cApMo9K7WuDa1SYDmejj4ObeZz2WGvVariL2w3+iWbUv19TsWv82XtuMoQBB6QApZFWh3JexzR2HnAkzO3RVaKhZvxDSrIg3c9gWE7f3e7w4k+6Gg2jWQ3H/dYswP70qFr2kX6T8=";
    public static string PublicExponentString = "AQAB";
    private const string CharList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    private const string RequestType = "4";

    public static Func<int, string?>? ConfigValueProvider { get; set; }

    private static RSA CreatePublicRsa()
    {
        byte[] modulus = Convert.FromBase64String(ModulusString);
        byte[] exponent = Convert.FromBase64String(PublicExponentString);
        var rsa = RSA.Create();
        rsa.ImportParameters(new RSAParameters
        {
            Modulus = NormalizeUnsigned(modulus),
            Exponent = NormalizeUnsigned(exponent)
        });
        return rsa;
    }

    private static byte[] NormalizeUnsigned(byte[] value)
    {
        if (value.Length > 1 && value[0] == 0)
            return value[1..];
        return value;
    }

    public static string EncryptString(string text)
    {
        byte[] plainText = Encoding.ASCII.GetBytes(text);
        using RSA rsa = CreatePublicRsa();
        return Convert.ToBase64String(
            rsa.Encrypt(plainText, RSAEncryptionPadding.Pkcs1));
    }

    public static string DecryptString(string text)
    {
        byte[] cipherText = Encoding.ASCII.GetBytes(text);
        using RSA rsa = CreatePublicRsa();
        return Convert.ToBase64String(
            rsa.Decrypt(cipherText, RSAEncryptionPadding.Pkcs1));
    }

    public static void DoCopy(Stream input, Stream output)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        input.CopyTo(output);
        output.Flush();
    }

    public static void Encrypt(string key, FileInfo inputFile, FileInfo outputFile)
    {
        DoCrypto(encrypt: true, key, inputFile, outputFile);
    }

    public static void Decrypt(string key, FileInfo inputFile, FileInfo outputFile)
    {
        DoCrypto(encrypt: false, key, inputFile, outputFile);
    }

    private static void DoCrypto(bool encrypt, string key, FileInfo inputFile, FileInfo outputFile)
    {
        try
        {
            byte[] keyBytes = Encoding.Unicode.GetBytes(key);
            byte[] iv = keyBytes;
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv.Length >= 16 ? iv[..16] : throw new ArgumentException("The AES IV must be at least 16 bytes.", nameof(key));
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            byte[] inputBytes = File.ReadAllBytes(inputFile.FullName);
            ICryptoTransform transform = encrypt
                ? aes.CreateEncryptor()
                : aes.CreateDecryptor();
            byte[] result = transform.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            File.WriteAllBytes(outputFile.FullName, result);
        }
        catch (Exception exception) when (exception is IOException or CryptographicException)
        {
            throw new CryptoException("Error encrypting/decrypting file", exception);
        }
    }

    private static byte[] GenerateSecretKey(string password)
    {
        using var deriveBytes = new Rfc2898DeriveBytes(
            password,
            PdfSalt,
            10000,
            HashAlgorithmName.SHA256);
        return deriveBytes.GetBytes(16);
    }

    public static void DecryptEncryptedFile(FileInfo encryptedFilePath,
        string password, string destination)
    {
        byte[] encryptedBytes = File.ReadAllBytes(encryptedFilePath.FullName);
        byte[] key = GenerateSecretKey(password);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = PdfSalt;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] decryptedBytes =
            decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        string fileName = Path.GetFileNameWithoutExtension(encryptedFilePath.Name);
        string outputPath = Path.Combine(destination, fileName + "_Decrypted.pdf");
        using var output = File.Create(outputPath);
        output.Write(PdfSalt);
        output.Write(decryptedBytes);
    }

    public static T CipherFields<T>(T applicant, string passPhase, string iv)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(applicant);
        var crypto = new Crypto();

        foreach (MemberInfo member in CipherTools.GetAllFields(applicant.GetType()))
        {
            try
            {
                object? value = member switch
                {
                    FieldInfo fieldInfo => fieldInfo.GetValue(applicant),
                    PropertyInfo { CanRead: true } property => property.GetValue(applicant),
                    _ => null
                };
                if (value is null ||
                    !Array.Exists(GlobalVariables.NEW_ENCRYPTED_FIELDS,
                        name => string.Equals(name, member.Name,
                            StringComparison.OrdinalIgnoreCase)) ||
                    value.ToString()!.Length == 0)
                {
                    continue;
                }

                string encrypted = crypto.RijndaelEncrypt(
                    value.ToString()!, passPhase, iv, 256);
                if (member is FieldInfo field)
                    field.SetValue(applicant, encrypted);
                else if (member is PropertyInfo { CanWrite: true } property)
                    property.SetValue(applicant, encrypted);
            }
            catch
            {
                // Preserve the Java implementation's field-level error isolation.
            }
        }
        return applicant;
    }

    public static string? Encrypt(string encryptedText, string passPhase, string salt, string iv)
    {
        try
        {
            byte[] ivBytes = Encoding.ASCII.GetBytes(iv);
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);
            using var deriveBytes = new Rfc2898DeriveBytes(
                passPhase,
                saltBytes,
                2,
                HashAlgorithmName.SHA1);
            byte[] key = deriveBytes.GetBytes(32);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(encryptedText);
            if (plainText.Length % aes.BlockSize / 8 != 0)
                throw new CryptographicException(
                    "Plaintext length must be a multiple of the AES block size for Java AES/CBC/NoPadding.");
            return Convert.ToBase64String(
                encryptor.TransformFinalBlock(plainText, 0, plainText.Length));
        }
        catch
        {
            return null;
        }
    }

    public static string GenerateRandomString(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        var output = new char[length];
        using var rng = RandomNumberGenerator.Create();
        Span<byte> randomByte = stackalloc byte[1];
        for (int i = 0; i < length; i++)
        {
            int number;
            do
            {
                rng.GetBytes(randomByte);
                number = randomByte[0] % CharList.Length;
            } while (number == CharList.Length);
            output[i] = CharList[number];
        }
        return new string(output);
    }

    public static string GenerateLongNumber()
    {
        string province = ConfigValueProvider?.Invoke(1) ?? string.Empty;
        string municipality = ConfigValueProvider?.Invoke(2) ?? string.Empty;
        string district = ConfigValueProvider?.Invoke(3) ?? string.Empty;
        int number = RandomNumberGenerator.GetInt32(100000, 9100000);
        return number + province + municipality + district + RequestType;
    }

    public static BigInteger GetBigInteger(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        return new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
    }

    public static string BackToString(BigInteger value)
    {
        byte[] bytes = value.ToByteArray();
        if (bytes.Length > 0 && bytes[^1] == 0)
            Array.Resize(ref bytes, bytes.Length - 1);
        Array.Reverse(bytes);
        return Encoding.UTF8.GetString(bytes);
    }
}
