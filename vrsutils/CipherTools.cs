#nullable enable

using System.Reflection;


namespace vrsutils;

public static class CipherTools
{
    private static readonly byte[] MasterKey =
    {
        116, 189, 175, 163, 55, 230, 40, 208, 115, 150, 163, 86, 99,
        25, 220, 17, 191, 78, 76, 213, 27, 129, 162, 214, 208, 36, 211,
        58, 54, 32, 84, 73
    };

    public static IReadOnlyList<MemberInfo> GetAllFields(Type type)
    {
        var fields = new List<MemberInfo>();
        for (Type? current = type; current is not null; current = current.BaseType)
        {
            fields.AddRange(current.GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly));
            fields.AddRange(current.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly));
        }
        return fields;
    }

    private static object? GetValue(MemberInfo member, object instance)
    {
        return member switch
        {
            FieldInfo field => field.GetValue(instance),
            PropertyInfo { CanRead: true } property => property.GetValue(instance),
            _ => null
        };
    }

    private static bool SetValue(MemberInfo member, object instance, object? value)
    {
        try
        {
            if (member is FieldInfo field)
            {
                field.SetValue(instance, value);
                return true;
            }
            if (member is PropertyInfo { CanWrite: true } property)
            {
                property.SetValue(instance, value);
                return true;
            }
        }
        catch
        {
            // Preserve the Java implementation's field-level error isolation.
        }
        return false;
    }

    private static string MemberName(MemberInfo member) => member.Name;

    private static bool Contains(IReadOnlyCollection<string> names, MemberInfo member)
    {
        foreach (string name in names)
        {
            if (string.Equals(name, MemberName(member), StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static T CipherFields<T>(T voter, bool isCipher, string secureKey, string vectorKey, int keySize)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(voter);
        var crypto = new Crypto();
        IReadOnlyCollection<string> encryptedFields =
            voter.GetType().Name.Contains("VoterDemoSk", StringComparison.Ordinal)
                ? GlobalVariables.SK_ENCRYPTED_FIELDS
                : GlobalVariables.NEW_ENCRYPTED_FIELDS;

        foreach (MemberInfo member in GetAllFields(voter.GetType()))
        {
            try
            {
                object? value = GetValue(member, voter);
                if (Contains(encryptedFields, member))
                {
                    if (value is not null && value.ToString()!.Length > 0)
                    {
                        string transformed = isCipher
                            ? crypto.RijndaelEncrypt(value.ToString()!, secureKey, vectorKey, keySize)
                            : crypto.RijndaelDecrypt(value.ToString()!, secureKey, vectorKey, keySize);
                        SetValue(member, voter, transformed);
                    }
                    else if (!isCipher)
                    {
                        SetValue(member, voter, string.Empty);
                    }
                    continue;
                }

                if (!Contains(GlobalVariables.NEW_BLOB_FIELDS, member) || value is not byte[] bytes)
                    continue;

                byte[] transformedBytes = isCipher
                    ? crypto.RijndaelEncrypt(bytes, secureKey, vectorKey, keySize)
                    : crypto.RijndaelDecrypt(bytes, secureKey, vectorKey, keySize);
                SetValue(member, voter, transformedBytes);
            }
            catch
            {
                // Preserve the Java implementation's per-field error isolation.
            }
        }
        return voter;
    }

    public static T CipherFieldsPremig<T>(T voter, bool isCipher, string secureKey, string vectorKey, int keySize)
        where T : class
    {
        return CipherFields(voter, isCipher, secureKey, vectorKey, keySize);
    }

    public static T CipherOldVRSFields<T>(T voter, bool isCipher, byte[] secureKey, string vectorKey, int keySize)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(voter);
        var crypto = new Crypto();

        foreach (MemberInfo member in GetAllFields(voter.GetType()))
        {
            try
            {
                object? value = GetValue(member, voter);
                if (Contains(GlobalVariables.VRS_ENCRYPTED_FIELDS, member))
                {
                    if (value is not null && value.ToString()!.Length > 0)
                    {
                        string text = value.ToString()!;
                        string transformed = isCipher
                            ? crypto.RijndaelEncrypt(text, secureKey, vectorKey, keySize)
                            : crypto.RijndaelDecrypt(text, secureKey, vectorKey, keySize);
                        SetValue(member, voter, transformed);
                    }
                    else if (!isCipher)
                    {
                        SetValue(member, voter, string.Empty);
                    }
                    continue;
                }

                if (Contains(GlobalVariables.BLOB_FIELDS, member) && value is byte[] bytes)
                {
                    byte[] transformed = isCipher
                        ? crypto.RijndaelEncrypt(bytes, secureKey, vectorKey)
                        : crypto.RijndaelDecrypt(bytes, secureKey, vectorKey);
                    SetValue(member, voter, transformed);
                }
            }
            catch
            {
                // Preserve the Java implementation's per-field error isolation.
            }
        }
        return voter;
    }

    public static T CipherFields<T>(T voter, bool isCipher, string secureKey, string vectorKey,
        int keySize, IReadOnlyCollection<string> fieldsToTransform)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(voter);
        var crypto = new Crypto();

        foreach (MemberInfo member in GetAllFields(voter.GetType()))
        {
            if (!Contains(fieldsToTransform, member))
                continue;

            try
            {
                object? value = GetValue(member, voter);
                if (value is not null && value.ToString()!.Length > 0)
                {
                    string transformed = isCipher
                        ? crypto.RijndaelEncrypt(value.ToString()!, secureKey, vectorKey, keySize)
                        : crypto.RijndaelDecrypt(value.ToString()!, secureKey, vectorKey, keySize);
                    SetValue(member, voter, transformed);
                }
                else if (!isCipher)
                {
                    SetValue(member, voter, string.Empty);
                }
            }
            catch
            {
                // Preserve the Java implementation's per-field error isolation.
            }
        }
        return voter;
    }

    public static T CipherFieldsSV<T>(T voter, bool isCipher, string secureKey, string vectorKey, int keySize)
        where T : class
    {
        return CipherFields(voter, isCipher, secureKey, vectorKey, keySize,
            GlobalVariables.NEW_ENCRYPTED_FIELDS);
    }

    public static T CipherFieldsSKS<T>(T sksvoter, bool isCipher, string secureKey, string vectorKey,
        int keySize, IReadOnlyCollection<string> fieldsToTransform)
        where T : class
    {
        return CipherFields(sksvoter, isCipher, secureKey, vectorKey, keySize, fieldsToTransform);
    }

    public static IList<T> CipherOldSKS<T>(IList<T> voters, bool isCipher, string secureKey, string vectorKey)
        where T : class
    {
        foreach (T voter in voters)
        {
            var crypto = new Crypto();
            foreach (MemberInfo member in GetAllFields(voter.GetType()))
            {
                try
                {
                    object? value = GetValue(member, voter);
                    if (Contains(GlobalVariables.SK_ENCRYPTED_FIELDS_FOR_DECRYPTION, member)
                        && value is not null && value.ToString()!.Length > 0)
                    {
                        string text = value.ToString()!;
                        if (!isCipher)
                        {
                            if (MemberName(member).Equals("civilstatus", StringComparison.OrdinalIgnoreCase))
                            {
                                text = text switch
                                {
                                    "C33iplsO8x7rM4UggwiGnQ==" => "S",
                                    "fAXoXZfyC6Qsu9rMTlF3Ng==" => "L",
                                    "gXRuuPNF22ijUYJYF4oHi2Q==" => "W",
                                    "IHG1QXl33/VpUBdQtJlDrg==" => "M",
                                    "IYxdPCsRPnzHf3W8N+jNMw==" => "S",
                                    "koplq4kdh553FuCxSSkCWA==" => "M",
                                    _ => crypto.RijndaelDecryptOld(text, secureKey, vectorKey)
                                };
                                SetValue(member, voter, text);
                            }
                            else
                            {
                                SetValue(member, voter,
                                    crypto.RijndaelDecryptOld(text, secureKey, vectorKey));
                            }
                        }
                        else
                        {
                            SetValue(member, voter,
                                crypto.RijndaelEncryptOld(text, secureKey, vectorKey));
                        }
                    }
                    else if (value is byte[] bytes &&
                             (MemberName(member).Equals("p1", StringComparison.OrdinalIgnoreCase) ||
                              MemberName(member).Equals("s1", StringComparison.OrdinalIgnoreCase)))
                    {
                        SetValue(member, voter, isCipher
                            ? crypto.RijndaelEncryptOld(bytes, secureKey, vectorKey)
                            : crypto.RijndaelDecryptOld(bytes, secureKey, vectorKey));
                    }
                }
                catch
                {
                    // Preserve the Java implementation's per-field error isolation.
                }
            }
        }
        return voters;
    }

    public static string RijndaelCipher(string? cipherText, string secureKey, string iv,
        int keySize, bool isCipher)
    {
        if (cipherText is null)
            return string.Empty;

        var crypto = new Crypto();
        return isCipher
            ? crypto.RijndaelEncrypt(cipherText, secureKey, iv, keySize)
            : crypto.RijndaelDecrypt(cipherText, secureKey, iv, keySize);
    }

    public static string AESdecrypt(string cipherText, string secureKey, string iv)
    {
        try
        {
            return new Crypto().AESdecrypt(cipherText, secureKey, iv);
        }
        catch
        {
            return string.Empty;
        }
    }

    public interface IGemaltoDecryptor
    {
        string DecryptMaster(string value, string fallback);
        byte[] DecryptMaster(byte[] value);
    }

    public static T DecryptGemaltoApplicant<T>(T gemApplicant,
        IGemaltoDecryptor securityManager)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(gemApplicant);
        ArgumentNullException.ThrowIfNull(securityManager);

        foreach (MemberInfo member in GetAllFields(gemApplicant.GetType()))
        {
            try
            {
                object? value = GetValue(member, gemApplicant);
                if (value is null)
                    continue;

                if (Contains(GlobalVariables.NEW_ENCRYPTED_FIELDS, member) &&
                    value.ToString()!.Length > 0)
                {
                    SetValue(member, gemApplicant,
                        securityManager.DecryptMaster(value.ToString()!, "FAILED TO DECRYPT"));
                }
                else if (Contains(GlobalVariables.FULL_BLOB_FIELDS, member) &&
                         value is byte[] bytes)
                {
                    SetValue(member, gemApplicant, securityManager.DecryptMaster(bytes));
                }
            }
            catch
            {
                // Preserve the Java implementation's per-field error isolation.
            }
        }
        return gemApplicant;
    }
}
