#nullable enable

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace vrsutils;

public sealed class CertificateManager : IDisposable
{
    private RSA? _privateKey;
    private RSA? _publicKey;

    public CertificateManager()
    {
    }

    public CertificateManager(int keyLength)
    {
        using RSA rsa = RSA.Create(keyLength);
        _privateKey = RSA.Create();
        _privateKey.ImportPkcs8PrivateKey(rsa.ExportPkcs8PrivateKey(), out _);
        _publicKey = RSA.Create();
        _publicKey.ImportSubjectPublicKeyInfo(rsa.ExportSubjectPublicKeyInfo(), out _);
    }

    public void Dispose()
    {
        _privateKey?.Dispose();
        _publicKey?.Dispose();
        _privateKey = null;
        _publicKey = null;
    }

    public void CreateKeyPair(int keyLength, string privateKeyPath, string publicKeyPath)
    {
        using RSA rsa = RSA.Create(keyLength);
        File.WriteAllBytes(privateKeyPath, rsa.ExportPkcs8PrivateKey());
        File.WriteAllBytes(publicKeyPath, rsa.ExportSubjectPublicKeyInfo());
    }

    public RSA GetKeyPair(string keyStoreFile, string storePass, string alias, string keyPass)
    {
        if (IsJksStore(keyStoreFile))
        {
            return LoadJksPrivateKey(keyStoreFile, storePass, alias, keyPass);
        }

        X509Certificate2Collection certificates = LoadPkcs12(keyStoreFile, storePass, keyPass);
        try
        {
            X509Certificate2 certificate = FindCertificate(certificates, alias, requirePrivateKey: true);
            using RSA? privateKey = certificate.GetRSAPrivateKey();
            return privateKey is null
                ? throw new CryptographicException($"Certificate '{alias}' does not contain an RSA private key.")
                : CopyRsaKey(privateKey, includePrivateKey: true);
        }
        finally
        {
            DisposeCertificates(certificates);
        }
    }

    public RSA GetPublicKey(string keyStoreFile, string keyStorePass, string alias, string keyPass)
    {
        if (IsJksStore(keyStoreFile))
        {
            JksStore store = LoadJksStore(keyStoreFile, keyStorePass);
            AsymmetricKeyParameter key = store.GetKey(alias, keyPass.ToCharArray())
                ?? throw new CryptographicException($"Key alias '{alias}' was not found.");
            if (!key.IsPrivate)
            {
                throw new CryptographicException($"Key alias '{alias}' does not contain a private key.");
            }

            Org.BouncyCastle.X509.X509Certificate certificate = store.GetCertificate(alias)
                ?? throw new CryptographicException($"Certificate alias '{alias}' was not found.");
            RSA publicKey = RSA.Create();
            byte[] publicKeyInfo = SubjectPublicKeyInfoFactory
                .CreateSubjectPublicKeyInfo(certificate.GetPublicKey())
                .GetDerEncoded();
            publicKey.ImportSubjectPublicKeyInfo(publicKeyInfo, out _);
            return publicKey;
        }

        X509Certificate2Collection certificates = LoadPkcs12(keyStoreFile, keyStorePass, keyPass);
        try
        {
            X509Certificate2 certificate = FindCertificate(certificates, alias, requirePrivateKey: false);
            using RSA? publicKey = certificate.GetRSAPublicKey();
            return publicKey is null
                ? throw new CryptographicException($"Certificate '{alias}' does not contain an RSA public key.")
                : CopyRsaKey(publicKey, includePrivateKey: false);
        }
        finally
        {
            DisposeCertificates(certificates);
        }
    }

    public RSA GetPrivateKey(string keyStoreFile, string keyStorePass, string alias, string keyPass)
    {
        if (IsJksStore(keyStoreFile))
        {
            return LoadJksPrivateKey(keyStoreFile, keyStorePass, alias, keyPass);
        }

        X509Certificate2Collection certificates = LoadPkcs12(keyStoreFile, keyStorePass, keyPass);
        try
        {
            X509Certificate2 certificate = FindCertificate(certificates, alias, requirePrivateKey: true);
            using RSA? privateKey = certificate.GetRSAPrivateKey();
            return privateKey is null
                ? throw new CryptographicException($"Certificate '{alias}' does not contain an RSA private key.")
                : CopyRsaKey(privateKey, includePrivateKey: true);
        }
        finally
        {
            DisposeCertificates(certificates);
        }
    }

    public string Encrypt(string plainText, RSA publicKey)
    {
        ArgumentNullException.ThrowIfNull(publicKey);
        byte[] ciphertext = publicKey.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(ciphertext);
    }

    public string Encrypt(string plainText, string publicKeyPath)
    {
        using RSA publicKey = RSA.Create();
        publicKey.ImportSubjectPublicKeyInfo(File.ReadAllBytes(publicKeyPath), out _);
        return Encrypt(plainText, publicKey);
    }

    public string Decrypt(string cipherText, RSA privateKey)
    {
        ArgumentNullException.ThrowIfNull(privateKey);

        byte[] plaintext = privateKey.Decrypt(
            Convert.FromBase64String(cipherText),
            RSAEncryptionPadding.Pkcs1);

        return Encoding.UTF8.GetString(plaintext);
    }

    public string Decrypt(string cipherText, string privateKeyPath)
    {
        using RSA privateKey = RSA.Create();
        privateKey.ImportPkcs8PrivateKey(File.ReadAllBytes(privateKeyPath), out _);
        return Decrypt(cipherText, privateKey);
    }

    public void WriteToFile(string privateKeyPathName, string publicKeyPathName)
    {
        if (_privateKey is null || _publicKey is null)
        {
            throw new InvalidOperationException("This instance was not initialized with a generated key pair.");
        }

        File.WriteAllBytes(privateKeyPathName, _privateKey.ExportPkcs8PrivateKey());
        File.WriteAllBytes(publicKeyPathName, _publicKey.ExportSubjectPublicKeyInfo());
    }

    public static int GenerateCerts(string keyStoreFile, string keyStorePass, string alias, string keyPassword, int keySize)
    {
        var startInfo = new ProcessStartInfo("keytool")
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        string[] arguments =
        {
            "-genkey", "-keyAlg", "RSA", "-alias", alias, "-keystore", keyStoreFile,
            "-storepass", keyStorePass, "-keypass", keyPassword, "-validity", "376",
            "-keysize", keySize.ToString(CultureInfo.InvariantCulture), "-dname",
            "CN=COMELEC, OU=ITD, O=SPD, L=MANILA, S=NCR, C=PH"
        };
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = Process.Start(startInfo)
            ?? throw new IOException("Unable to start keytool.");
        string standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0 && !string.IsNullOrWhiteSpace(standardError))
        {
            Console.Error.WriteLine(standardError);
        }

        return process.ExitCode;
    }

    public static DateTime? GetCertExpiryDate(string keyStoreFile, string storePass, string alias)
    {
        if (IsJksStore(keyStoreFile))
        {
            Org.BouncyCastle.X509.X509Certificate certificate = LoadJksStore(keyStoreFile, storePass)
                .GetCertificate(alias)
                ?? throw new CryptographicException($"Certificate alias '{alias}' was not found.");
            return certificate.NotAfter;
        }

        X509Certificate2Collection certificates = LoadPkcs12(keyStoreFile, storePass);
        try
        {
            return FindCertificate(certificates, alias, requirePrivateKey: false).NotAfter;
        }
        finally
        {
            DisposeCertificates(certificates);
        }
    }

    public void SaveKeyPair(string path, RSA keyPair)
    {
        ArgumentNullException.ThrowIfNull(keyPair);
        File.WriteAllBytes(Path.Combine(path, "public.key"), keyPair.ExportSubjectPublicKeyInfo());
        File.WriteAllBytes(Path.Combine(path, "private.key"), keyPair.ExportPkcs8PrivateKey());
    }

    public static int ChangePasswordP12(
        string srcFileDir,
        string srcFileName,
        string destFileDir,
        string destFileName,
        string oldPass,
        string newPass)
    {
        string oldFilePath = Path.Combine(srcFileDir, srcFileName);
        if (!File.Exists(oldFilePath))
        {
            return 1;
        }

        string newFilePath = Path.Combine(destFileDir, destFileName);
        try
        {
            X509Certificate2Collection certificates = LoadPkcs12(oldFilePath, oldPass);
            try
            {
                X509Certificate2 certificate = certificates.Cast<X509Certificate2>()
                    .FirstOrDefault(candidate => candidate.HasPrivateKey)
                    ?? throw new CryptographicException("The PKCS#12 file does not contain a private key.");
                File.WriteAllBytes(newFilePath, certificate.Export(X509ContentType.Pkcs12, newPass));
            }
            finally
            {
                DisposeCertificates(certificates);
            }
            return 0;
        }
        catch (Exception exception) when (exception is IOException or CryptographicException or ArgumentException or InvalidOperationException)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    public static void RunCmd(string command)
    {
        string[] parts = SplitCommandLine(command);
        if (parts.Length == 0)
        {
            throw new ArgumentException("Command cannot be empty.", nameof(command));
        }

        var startInfo = new ProcessStartInfo(parts[0])
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        foreach (string argument in parts.Skip(1))
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = Process.Start(startInfo)
            ?? throw new IOException($"Unable to start command '{parts[0]}'.");
        Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();
        process.WaitForExit();
        string standardOutput = standardOutputTask.GetAwaiter().GetResult();
        string standardError = standardErrorTask.GetAwaiter().GetResult();
        if (process.ExitCode != 0)
        {
            throw new IOException($"Command '{parts[0]}' exited with code {process.ExitCode}: {standardError}{standardOutput}");
        }
    }

    public static void DeleteIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public static void RemoveLines(string filePath, int linesToRemove)
    {
        if (linesToRemove <= 0)
        {
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        if (linesToRemove > lines.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(linesToRemove), "Cannot remove more lines than the file contains.");
        }

        File.WriteAllLines(filePath, lines.Skip(linesToRemove));
    }

    private static X509Certificate2Collection LoadPkcs12(string path, string password, string? keyPassword = null)
    {
        byte[] fileContents = File.ReadAllBytes(path);
        try
        {
            return X509CertificateLoader.LoadPkcs12Collection(
                fileContents,
                password,
                X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
        }
        catch (CryptographicException) when (!string.IsNullOrEmpty(keyPassword) && keyPassword != password)
        {
            return X509CertificateLoader.LoadPkcs12Collection(
                fileContents,
                keyPassword,
                X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
        }
    }

    private static bool IsJksStore(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return new JksStore().Probe(stream);
    }

    private static JksStore LoadJksStore(string path, string storePassword)
    {
        var store = new JksStore();
        using FileStream stream = File.OpenRead(path);
        store.Load(stream, storePassword.ToCharArray());
        return store;
    }

    private static RSA LoadJksPrivateKey(string path, string storePassword, string alias, string keyPassword)
    {
        JksStore store = LoadJksStore(path, storePassword);
        AsymmetricKeyParameter key = store.GetKey(alias, keyPassword.ToCharArray())
            ?? throw new CryptographicException($"Key alias '{alias}' was not found.");
        if (!key.IsPrivate)
        {
            throw new CryptographicException($"Key alias '{alias}' does not contain a private key.");
        }

        byte[] privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(key).GetDerEncoded();
        RSA rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(privateKeyInfo, out _);
        return rsa;
    }

    private static void DisposeCertificates(X509Certificate2Collection certificates)
    {
        foreach (X509Certificate2 certificate in certificates)
        {
            certificate.Dispose();
        }
    }

    private static RSA CopyRsaKey(RSA key, bool includePrivateKey)
    {
        RSA copy = RSA.Create();
        if (includePrivateKey)
        {
            copy.ImportPkcs8PrivateKey(key.ExportPkcs8PrivateKey(), out _);
        }
        else
        {
            copy.ImportSubjectPublicKeyInfo(key.ExportSubjectPublicKeyInfo(), out _);
        }

        return copy;
    }

    private static X509Certificate2 FindCertificate(
        X509Certificate2Collection certificates,
        string alias,
        bool requirePrivateKey)
    {
        X509Certificate2[] candidates = certificates.Cast<X509Certificate2>()
            .Where(certificate => !requirePrivateKey || certificate.HasPrivateKey)
            .ToArray();

        X509Certificate2? aliasMatch = candidates.FirstOrDefault(certificate =>
            string.Equals(certificate.FriendlyName, alias, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(certificate.GetNameInfo(X509NameType.SimpleName, false), alias, StringComparison.OrdinalIgnoreCase));

        if (aliasMatch is not null)
        {
            return aliasMatch;
        }

        if (candidates.Length == 1)
        {
            return candidates[0];
        }

        throw new CryptographicException(
            candidates.Length == 0
                ? "The PKCS#12 file contains no matching certificate."
                : $"The alias '{alias}' does not uniquely identify a certificate in the PKCS#12 file.");
    }

    private static string[] SplitCommandLine(string command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var parts = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;
        bool hasToken = false;

        foreach (char character in command)
        {
            if (character == '"')
            {
                inQuotes = !inQuotes;
                hasToken = true;
            }
            else if (char.IsWhiteSpace(character) && !inQuotes)
            {
                if (hasToken)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                    hasToken = false;
                }
            }
            else
            {
                current.Append(character);
                hasToken = true;
            }
        }

        if (inQuotes)
        {
            throw new ArgumentException("Command contains an unterminated quoted argument.", nameof(command));
        }

        if (hasToken)
        {
            parts.Add(current.ToString());
        }

        return parts.ToArray();
    }
}
