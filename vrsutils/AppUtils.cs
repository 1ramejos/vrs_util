#nullable enable

using System.Security.Cryptography;

namespace vrsutils;

public static class AppUtils
{
   
    public static void Appinit()
    {
       FixKeyLength();

        using CertificateManager certificateManager = new();
      //  string privateKeyPath = "D:\\vrsutils\\vrsutils\\vrsutils\\keystore\\private.key";
        string appSecretKey = GlobalVariables.APP_SK1 +
            certificateManager.Decrypt(AppConfig.AppSK2, AppConfig.privateKeyFile);
        string appInitializationVector = GlobalVariables.APP_IV1 +
            certificateManager.Decrypt(AppConfig.AppIV2, AppConfig.privateKeyFile);
        vrsutils.Crypto crypto = new();
        string? databasePassword = crypto.RijndaelDecrypt(AppConfig.DBPassword, appSecretKey, appInitializationVector, 256);
        string? logDatabasePassword = crypto.RijndaelDecrypt(AppConfig.LogDBPassword, appSecretKey, appInitializationVector, 256);
        
        GlobalVariables.DatabasePassword = databasePassword;
        GlobalVariables.LogDatabasePassword = logDatabasePassword;
    }

    public static void FixKeyLength()
    {
        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        if (aes.KeySize != 256)
        {
            throw new CryptographicException("The current runtime does not support 256-bit AES keys.");
        }
    }

    public static string DecryptContextValue(string keyStore, string storePass, string alias, string keyPass, string textToDecrypt)
    {
        using CertificateManager certificateManager = new();
        using RSA privateKey = certificateManager.GetPrivateKey(keyStore, storePass, alias, keyPass);
        return certificateManager.Decrypt(textToDecrypt, privateKey);
    }

    public static string EncryptContextValue(
        string keyStore,
        string storePass,
        string alias,
        string keyPass,
        string textToEncrypt)
    {
        using var certificateManager = new CertificateManager();
        using RSA publicKey = certificateManager.GetPublicKey(keyStore, storePass, alias, keyPass);
        return certificateManager.Encrypt(textToEncrypt, publicKey);
    }




   public static string DecryptConfigValue(string encryptedValue,
       string keyStore, string storePass, string alias, string keyPass)
    {   
        return DecryptContextValue(keyStore, storePass, alias, keyPass, encryptedValue);
    }

    public static string DecryptPremigConfigValue(
        string encryptedValue,
        string keyStore,
        string storePass,
        string alias,
        string keyPass)
    {
       // ArgumentNullException.ThrowIfNull(environment);
      //  string encryptedValue = environment.GetPremigConfigValue(configId)
     //       ?? throw new InvalidOperationException($"Premigration configuration value {configId} was not found.");
        return DecryptContextValue(keyStore, storePass, alias, keyPass, encryptedValue);
    }
}
