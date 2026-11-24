#nullable enable

using MySqlConnector;
using System.Security.Cryptography;
using System.Xml.Linq;
using static vrsutils.AppUtils;

namespace vrsutils;

public class CertificateAction
{
  //  private const int RequiredPasswordLength = 10;

    public static void ConfirmCert(string keyPass, string certPass)
    {
        if (string.IsNullOrEmpty(keyPass) || string.IsNullOrEmpty(certPass))
              {
                MessageBox.Show("Keystore & Certificate Password is required.");
                // return "failed";
              }

              if (keyPass.Length != 10 || certPass.Length != 10)
              {
            MessageBox.Show("Keystore & Certificate maximum length is 10 characters.");
              //    return "failedlength";*/
          }
       
        string logsql = "SELECT ID, VALUE FROM configuration_local.config";
        DBTools dBTool = new();
        MySqlDataReader mySqlDataReadr = dBTool.MySqlFetchDataReader(logsql, Dbconn.con_str);
       int[] arrayID = new int[50];
        string[] arrayVAL = new string[50];
        int x = 0;
        while (mySqlDataReadr.Read())
        {
               try
                {
                       arrayID[x] = mySqlDataReadr.GetInt32(mySqlDataReadr.GetOrdinal("ID"));

                arrayVAL[x] = mySqlDataReadr.GetString(mySqlDataReadr.GetOrdinal("VALUE"));
                x++;
                }
                catch (Exception ex)
                {
                          }
        }

        string appStoreFile = AppConfig.StoreFile;
        string certDirectory = AppConfig.CertificatePath;
        string appKeyStorePath =  Path.Combine(certDirectory, appStoreFile);
        string municipalKeyStoreName = arrayVAL[Array.IndexOf(arrayID, 18)];
        string municipalKeyStorePath = Path.Combine(certDirectory, municipalKeyStoreName);
        using var certificates = new CertificateManager();
        string appAlias = arrayVAL[Array.IndexOf(arrayID, 16)];
        string appStorePass = certificates.Decrypt(arrayVAL[Array.IndexOf(arrayID, 15)], AppConfig.privateKeyFile);  
        string appKeyPass = certificates.Decrypt(arrayVAL[Array.IndexOf(arrayID, 17)], AppConfig.privateKeyFile);
        string userAppAlias = arrayVAL[Array.IndexOf(arrayID, 19)];
        string user1Alias = arrayVAL[Array.IndexOf(arrayID, 20)];
        string user2Alias = arrayVAL[Array.IndexOf(arrayID, 21)];
        string municipalStorePass = DecryptConfigValue(arrayVAL[Array.IndexOf(arrayID, 22)],appKeyStorePath, appStorePass, appAlias, appKeyPass); 
        string user2KeyPass = DecryptConfigValue(arrayVAL[Array.IndexOf(arrayID, 24)], appKeyStorePath, appStorePass, appAlias, appKeyPass); 
        string combinedMunicipalStorePass = municipalStorePass + keyPass; 
        string combinedUser2KeyPass = user2KeyPass + certPass;
        string user1DecryptedPass = DecryptConfigValue(arrayVAL[Array.IndexOf(arrayID, 23)], municipalKeyStorePath, combinedMunicipalStorePass, user2Alias, combinedUser2KeyPass);
        string initializationVector = DecryptConfigValue(arrayVAL[Array.IndexOf(arrayID, 27)], 
            municipalKeyStorePath, combinedMunicipalStorePass, user2Alias, combinedUser2KeyPass);
        string secureKey = DecryptConfigValue(arrayVAL[Array.IndexOf(arrayID, 26)], municipalKeyStorePath,
            combinedMunicipalStorePass, user1Alias, user1DecryptedPass);
        string userAppKeyPass = DecryptConfigValue(arrayVAL[Array.IndexOf(arrayID, 25)], municipalKeyStorePath,
            combinedMunicipalStorePass, user1Alias, user1DecryptedPass);

        RSA appPrivateKey = certificates.GetPrivateKey(municipalKeyStorePath, combinedMunicipalStorePass, userAppAlias, userAppKeyPass);
        RSA appPublicKey = certificates.GetPublicKey(municipalKeyStorePath, combinedMunicipalStorePass, userAppAlias, userAppKeyPass);
        RSA vrsPrivateKey = certificates.GetPrivateKey(appKeyStorePath, appStorePass, appAlias, appKeyPass);
        RSA vrsPublicKey = certificates.GetPublicKey(appKeyStorePath, appStorePass, appAlias, appKeyPass);
      //     appPrivateKey.Dispose();
       //     appPublicKey.Dispose();
        //    vrsPrivateKey.Dispose();
        //    vrsPublicKey.Dispose();
       //     throw new CryptographicException($"Certificate '{user1Alias}' has no readable expiration date.");
       // }

        GlobalVariables.AppPubkey = appPublicKey;
        GlobalVariables.AppPrivkey = appPrivateKey;
        GlobalVariables.VRSPubkey = vrsPublicKey;
        GlobalVariables.VRSPrivkey = vrsPrivateKey;
        GlobalVariables.SecureKey = secureKey;
        GlobalVariables.InitVector = initializationVector;
        GlobalVariables.Provcode = arrayVAL[Array.IndexOf(arrayID, 1)];
        GlobalVariables.Citimunicode = arrayVAL[Array.IndexOf(arrayID, 2)];
        GlobalVariables.Distcode = arrayVAL[Array.IndexOf(arrayID, 3)];
    }

    private static bool HasAuthenticatedUser(IReadOnlyDictionary<string, object?>? session)
    {
        return session is { Count: > 0 } &&
            session.TryGetValue("userid", out object? userId) &&
            userId is not null &&
            !string.IsNullOrEmpty(Convert.ToString(userId));
    }

 /*   private static string DecryptConfig(
        IReadOnlyDictionary<int, string?> configValues,
        int configId,
        string keyStore,
        string storePassword,
        string alias,
        string keyPassword)
    {
        string encryptedValue = GetConfigValue(configValues, configId);
        return AppUtils.DecryptContextValue(
            keyStore,
            storePassword,
            alias,
            keyPassword,
            encryptedValue);
    }

    private static string GetConfigValue(IReadOnlyDictionary<int, string?> configValues, int configId)
    {
        if (!configValues.TryGetValue(configId, out string? value) || value is null)
        {
            throw new InvalidOperationException($"Required database configuration value {configId} is missing.");
        }

        return value;
    }*/
}
