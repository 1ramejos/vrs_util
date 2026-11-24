#nullable enable

namespace vrsutils;

public static class AppConfig
{
    public const int SessionTimeoutMinutes = -1;
   /* public const string WelcomeFile = "index.html";
    public const string Error403Page = "\\error_403.jsp";
    public const string Error404Page = "\\error_404.jsp";
    public const string Error500Page = "\\error_500.jsp";*/
    public const string StoreFile = "vrs_local.jks";
    public const string CertificatePath = "C:\\Program Files\\Apache Software Foundation\\Apache Tomcat 8\\crt\\";
    public const string TemporaryCertificatePath = "C:\\Program Files\\Apache Software Foundation\\Apache Tomcat 8\\temp\\";
    public const string BackupCertificatePath = "C:\\Program Files\\Apache Software Foundation\\Apache Tomcat 8\\back_cert_csk\\";
    public const string SystemDatePath = "C:\\Program Files\\Apache Software Foundation\\Apache Tomcat 8\\sdate\\";
    public const string PdfPath = "C:\\PDF\\PDF_REG\\";
    public const string ImportTemporaryPath = "tmp\\import\\";
    public const string ExportTemporaryPath = "tmp\\export\\";
    public const string BackupDataPath = "C:\\Program Files\\Apache Software Foundation\\Apache Tomcat 8\\";
    public const string CdBurnerApp = "portable";
    public const string CdBurnerExecutable = "executables\\anyburn-32bit\\AnyBurn.exe";
    public const string OldSksKeyStore = "c:\\newcerts\\sks.jks";
    public const string DBPassword = "nzxTP2MtMaSQNly1mGtqodDcgBkv8qCjmaGf8shLxJ4=";
    public const string LogDBPassword = "nzxTP2MtMaSQNly1mGtqodDcgBkv8qCjmaGf8shLxJ4=";
    public const string AppSK2 = "b7KEhgeGPhc5yCDfcHUVXAHX5IzbZrVBTCxXlyA0YUo5B/MoeF+quY08DnIQhI5n/exR1H4eXgq+x5aORvmTsKDvSE8LTdhlbiew+pZUOMHGrHTQlsuvbyudIZBx28xcwkLmZgTCNJ78TTy//gLhp+IqsxA6WkPIVw6crFJk/UY=";
    public const string AppIV2 = "MYUUlOPL8psVL7QzIjt1afraEvjWeGP7hnUqrbI3K2xkCybsvbtNR+3/oBlsNvRhKuCjNwONe4tAeyqGYtTTjkhOehfunsOsWnjF2kQgjKjq8MLFTBKe0bxJeqr9kft7Z9pJ8GfhVGIYYXLvyn6wjHsXJMfdsnVe4pKuT2ZEqxU=";
    public const string AccessCodeIV = "bYw/ouuKpr0/cYg4D0ITWMnzpoesyTnWAH2DIxqH1vwOzpgejIJCcBM2DXaUfaMdEV91JPfDXVnXBVH+xZenNDNqLMw+YpbW+c2tnjfI0sJheSKcFuposN1mVwy29a6/o3F97JyW95ZdcP/SQyzXCbPxSwLhd2pUBHKq+3UG9KI=";
    public const string AccessCodeSK = "IWCU3WKwrM8dNYaDkmmnsgTThKKfgLRFVYHz40qcQIwYThIjnrS0IoPxwwZba+G1VbZWt9JEMUQKvt0HI7zLI6+KH0Gokb3pktL5wGiT7CXrt0PbFvBULUrfVdoQgwGsK5y4jqgNbvtA7w/zd8DQNIkXrlAri/p+MZ0lQsM1SAQ=";
    public const string DBZip = "jooUk]*crwU'FD}gNJm";
    public const string SKSecureKey = "FfA+HmaIB++Z7sAXyaQpxdnVlAEHbuAW4RERZk+8oaCecoILn54FhrrJ605E4TaZmT4z1bZrB0Zik+mZA6LI4AnS4d2UOtlPeBWLpp3BkrgrUz5q3VwUGPmQpg48Pg41sfaKOLFsy72eBtuO+IvnqatWMbKBg6HQCZF4ora0mxk=";
    public const string SKVectorKey = "ZNsy4WbPGSI1yQ1aRgwIJ0L9YKdUyToIqnhYrFCU3ogz9xTTAgPDWiJrMmDqZcySRpzanh/4vwVA9FAym7J8OuRYC7LQcyKdO5h5Iz/EJXmVY7/oyJIktX/MtMZPJJ8vSGy3Rr+QG+qLG8fOF+/dA2uFenuPJK3YmXYDNDqsnmo=";
    public const string SKSOldKeyPassword = "O6CMYjicdA0QYGgCvBrlbeNJVPddLpNri2PL87nF8E/lvppa1hZiPOpSS5UYy3ez2L8Dpk+Krb1TtxoUSZs69amhXwblxmpfJ9PWAZzBQofiNQ5FgUI3mgpNi+VZ3m5aZzqF1JZiEnNKgKz+JKYFaLoKfj1iDPJn+xrIuAtJez8=";
    public const string SKSOldKeyStorePassword = "ICAoxVAiu6WVuvRxX6xlZg==";
    public const string SKSOldAppsKeyPassword = "o6BvBEJwjKKdIbEvg4nmDA==";
    public const string SKSOldsecureKey = "NzvljNs3MYfOHxE6JAfE/r0y6LBJ6RUE4xmG0vLE6AMtZ/xvDVFPaEfoqnP+6s2f1CPQPA7HmMPsNJnItrF3tmnxN6SpyWuhtx37+O9D/PevNE+6I+TbtUu81WeuT2d/LH1/239WSNRF+Leeosh/vC+AaRTIsPO69yK6wS2SngE=";
    public const string SKSOldvectorKey = "jiSHFPOJJgSbUn/xQtoJkbtoTpMb5pzOlGfR+o5m1EpjePtdTRxnoBG634mcI6UlbJ48QYADbITne8Xj4o7au5ChR6xLquK+uD2o+OVsGxIBeAQPWD3iKYZOAR0BiGbb4sFGeLM0liFZmcAlrV6hK+N/Uu4HNEMfgWq0A2nX1l0=";
    public const string scriptGeneratorSecKey = "En9WLD3PuyIw/sRxTSU+hlgSoKgLQ8gAS4JE0esJ8dpDvvF9NhTK9z7Sm1iIMGwuVUV8lA2jUZhZAxyHwvpGBtXKvQbYCkfQxydSZUNJzL6QKPpQjOkTJnTrSU/BOR3T/UvphyfmV6s8G+dVBt17KD0JxHS1UTlhKD2/0tcMqOc=";
    public const string scriptGeneratorIniVec = "eN//yD1hqtEq3YiC7/9mcHQ0zIqcAgru7UIHszcGcz9hFAmfDGFBq13oq6NiAIGsiNbBl3kc94zikZhbLlSXKjV5zP5vle0E4yaMmS7HLZzd/VnfI4bDE8xA0VhzaK9hF/AUkkNUnfpsCQ1Sb3ciQJQaXW23lczwDEsRhqam5BE=";
    public const string iReSecKey = "dtTpxgxIRsLREwkzdeAvD5ONnggp3FfxN/ir8QqhXDuVefUhhPeSdfo5X1OFDzxnhsek1x3jkEQcEul/F9bnSpOpXFyUaUQIjm3RRWHsC2H+UyVHxmBS9dRfynnp3t4xd4auoglHE+9SSru7JrXAZouJvneUUrdgV9Um/+iCSCs=";
    public const string iReIniVec = "DADgSxkfrXIwGyHOBPbwHbtn/OwY/RCOplR5SaMCkIHxme7Wf03u0Sq/W2owDVCLsqxtyWz+wDVM0UYwgMAaoMDI5+f8lmLFuJCgNKJUILouDlOCkCvp3GroUxeCbtk9lqpGh56mGlsFPNSLk/DynH/RH9nzNM9GSoA0pDtZWUY=";
    public const string fumerIniVec = "MpS3pTDrDU65m5rGmRKM2AiIc9sk13zCGVfqG6k7+KKX5qG491y2fd7reJn7QpuIjzgFQ+7pq37SGyC2NKcpfw==";
    public const string fumerImportPass = "U6kL1fV+BsJBZIMjd5VbhJcym8CYHaifVLs+3kfE1Qo=";
    public const string privateKeyFile = "C:\\Program Files\\Apache Software Foundation\\apache-tomcat-9.0.7\\webapps\\vrs_local\\keystore\\private.key";
    private sealed class VrsAppContext
    {
        private readonly string _applicationRoot;
        private readonly IReadOnlyDictionary<string, string?> _overrides;

        public VrsAppContext(string applicationRoot, IReadOnlyDictionary<string, string?>? overrides)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(applicationRoot);
            _applicationRoot = Path.GetFullPath(applicationRoot);
            _overrides = overrides ?? new Dictionary<string, string?>();
        }

        public string? GetInitParameter(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (_overrides.TryGetValue(name, out string? overriddenValue))
            {
                return overriddenValue;
            }

            string? environmentValue = Environment.GetEnvironmentVariable($"VRS_{name}");
            if (environmentValue is not null)
            {
                return environmentValue;
            }

            return name switch
            {
                "StoreFile" => StoreFile,
                "CertPath" => CertificatePath,
                "TempCertPath" => TemporaryCertificatePath,
                "BackupCertPath" => BackupCertificatePath,
                "SystemDTPath" => SystemDatePath,
                "pdfPath" => PdfPath,
                "ImporTempPath" => ImportTemporaryPath,
                "ExportTempPath" => ExportTemporaryPath,
                "BackupDataPath" => BackupDataPath,
                "cdBurnerApp" => CdBurnerApp,
                "cdBurnerDir" => CdBurnerExecutable,
                "SKSOldKeyStore" => OldSksKeyStore,
                _ => null
            };
        }

        public string GetRealPath(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            string relativePath = path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return Path.GetFullPath(Path.Combine(_applicationRoot, relativePath));
        }
    }
}
