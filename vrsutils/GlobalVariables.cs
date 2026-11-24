#nullable enable

using System.Security.Cryptography;

//using ph.gov.comelec.vrs.app.tools;
//using ph.gov.comelec.vrs.security;

namespace vrsutils;

public class GlobalVariables
{
    public const string RAPKEY = "cy;.4K!Q]aA^>`R@~^2U";
    public const string USER_SK = "v/P0f~VKh*IwFspPpxnqzSXbXixu*jFN";
    public const string USER_IV = "ckJSkPY!lWDzC$eO*czH@sx?M8wlhpEB";
    public const string APP_SK1 = ":QKvrr5*ho";
    public const string APP_IV1 = "~/mOetx?aG";
    public static string DatabasePassword;
    public static string LogDatabasePassword;
    public static readonly string[] VRS_ENCRYPTED_FIELDS =
    {
        "formId", "lastname", "firstname", "maternalname", "sex", "civilstatus", "resstreet",
        "dobmonth", "dobday", "dobyear", "vinp1", "vinp2", "vinp3", "suffix"
    };

    public static readonly string[] NEW_ENCRYPTED_FIELDS =
    {
        "lastname", "firstname", "maternalname", "sex", "civilstatus", "resstreet", "dobmonth",
        "dobday", "dobyear", "vinp1", "vinp2", "vinp3", "suffix"
    };

    public static readonly string[] SK_ENCRYPTED_FIELDS =
    {
        "lastname", "firstname", "maternalname", "sex", "civilstatus", "dobmonth", "dobday",
        "dobyear", "resstreet", "flastname", "ffirstname", "fmaternalname", "mlastname",
        "mfirstname", "mmaternalname", "spousename", "vinp1", "vinp2", "vinp3", "email", "tin",
        "suffix"
    };

    public static readonly string[] SK_ENCRYPTED_FIELDS_FOR_DECRYPTION =
    {
        "formId", "lastname", "firstname", "maternalname", "sex", "civilstatus", "resstreet",
        "dobmonth", "dobday", "dobyear", "vinp1", "vinp2", "vinp3", "suffix"
    };

    public static readonly string[] SK_ENCRYPTED_FIELDS_FOR_REENCRYPTION =
    {
        "lastname", "firstname", "maternalname", "sex", "civilstatus", "resstreet", "dobmonth",
        "dobday", "dobyear", "vinp1", "vinp2", "vinp3", "suffix"
    };

    public static readonly string[] NEW_BLOB_FIELDS = { "p1", "s1", "f1", "f2", "f6", "f7" };
    public static readonly string[] FULL_BLOB_FIELDS =
    {
        "p1", "s1", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "f10"
    };
    public static readonly string[] BLOB_FIELDS = { "p1", "s1" };

    public const string CONFIDENTIALITY_NOTICE =
        "CONFIDENTIALITY NOTICE: All authorized recipients of any personal data, personal information, privileged information and sensitive personal information contained in this document, including other pertinent documents attached thereto that are shared by the Commission on Elections in compliance with the existing laws and rules, and in conformity with the Data Privacy Act of 2012 ( R.A. No. 10173 ) and its implementing Rules and Regulations, as well as the pertinent Circulars of the National Privacy Commission, are similarly bound to comply with the said laws, rules and regulations, relating to data privacy, security, confidentiality, protection and accountability.";

    private static readonly object StateLock = new();
    private static string? _databasePassword;
    private static string? _logDatabasePassword;
    private static string? _secureKey;
    private static string? _initVector;
    private static string? _skSecureKey;
    private static string? _skInitVector;
    private static string? _premigSecureKey;
    private static string? _premigInitVector;
    private static RSA? _premigVrsPublicKey;
    private static RSA? _premigVrsPrivateKey;
    private static RSA? _vrsPublicKey;
    private static RSA? _vrsPrivateKey;
    private static RSA? _appPublicKey;
    private static RSA? _appPrivateKey;
    private static string _realPath = string.Empty;
    private static string? _provCode;
    private static string? _distCode;
    private static string? _citiMunCode;
    private static string? _daysLeft;
    private static DateTime? _certEndDate;
    private static string? _machineReady;

    private readonly Func<int, string?>? _getConfigValue;
    private readonly Func<int, string?>? _getPremigConfigValue;


  
    public static string? SecureKey { get { lock (StateLock) return _secureKey; } set { lock (StateLock) _secureKey = value; } }
    public static string? InitVector { get { lock (StateLock) return _initVector; } set { lock (StateLock) _initVector = value; } }
    public static string? SkSecureKey { get { lock (StateLock) return _skSecureKey; } set { lock (StateLock) _skSecureKey = value; } }
    public static string? SkInitVector { get { lock (StateLock) return _skInitVector; } set { lock (StateLock) _skInitVector = value; } }
    public static string? PremigSecureKey { get { lock (StateLock) return _premigSecureKey; } set { lock (StateLock) _premigSecureKey = value; } }
    public static string? PremigInitVector { get { lock (StateLock) return _premigInitVector; } set { lock (StateLock) _premigInitVector = value; } }
    public static RSA? PremigVRSPubkey { get { lock (StateLock) return _premigVrsPublicKey; } set { lock (StateLock) _premigVrsPublicKey = value; } }
    public static RSA? PremigVRSPrivkey { get { lock (StateLock) return _premigVrsPrivateKey; } set { lock (StateLock) _premigVrsPrivateKey = value; } }
    public static RSA? VRSPubkey { get { lock (StateLock) return _vrsPublicKey; } set { lock (StateLock) _vrsPublicKey = value; } }
    public static RSA? VRSPrivkey { get { lock (StateLock) return _vrsPrivateKey; } set { lock (StateLock) _vrsPrivateKey = value; } }
    public static RSA? AppPubkey { get { lock (StateLock) return _appPublicKey; } set { lock (StateLock) _appPublicKey = value; } }
    public static RSA? AppPrivkey { get { lock (StateLock) return _appPrivateKey; } set { lock (StateLock) _appPrivateKey = value; } }
    public static string RealPath { get { lock (StateLock) return _realPath; } set { lock (StateLock) _realPath = value; } }
    public static string? Provcode { get { lock (StateLock) return _provCode; } set { lock (StateLock) _provCode = value; } }
    public static string? Distcode { get { lock (StateLock) return _distCode; } set { lock (StateLock) _distCode = value; } }
    public static string? Citimunicode { get { lock (StateLock) return _citiMunCode; } set { lock (StateLock) _citiMunCode = value; } }
    public static string? Daysleft { get { lock (StateLock) return _daysLeft; } set { lock (StateLock) _daysLeft = value; } }
    public static DateTime? CertEndDate { get { lock (StateLock) return _certEndDate; } set { lock (StateLock) _certEndDate = value; } }
    public static string? MachineReady { get { lock (StateLock) return _machineReady; } set { lock (StateLock) _machineReady = value; } }

    public static string GetAppSecureKey()
    {
        using CertificateManager certificates = new();
        string tmp = certificates.Decrypt(AppConfig.AccessCodeSK, AppConfig.privateKeyFile);
        return tmp;
    }

    public static string GetAppInitVector()
    {
           using CertificateManager certificates = new();
       string tmp =  certificates.Decrypt(AppConfig.AccessCodeIV,AppConfig.privateKeyFile);
        return tmp;
    }

}
