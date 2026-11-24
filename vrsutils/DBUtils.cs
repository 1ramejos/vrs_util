#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace vrsutils;

public enum ConfigFile
{
    Applicant,
    Voter,
    Doctable,
    Ref,
    Config,
    Log,
    Reports,
    Applicant_Test,
    Voter_Test,
    Sk,
    Gemalto,
    LocalSetting,
    LocalReport,
    Applicant_Rap,
    Db_Sks,
    Config_Premig
}

public interface IDbManager : IDisposable
{
    void OpenSession();
    void CloseSession();
    void BeginTransaction();
    int CommitTransaction();
    void RollBack();

    void Save(object record);
    void SaveOrUpdate(object record);
    void Update(object record);
    void Delete(object record);

    object? GetObject(Type entityType, object id);
    object? GetObjectHql(string query);
    object? GetObjectHql(string query, string parameterName, string parameterValue);
    object? ExecuteScalarSql(string query);
    object? ExecuteScalarSql(string query, string parameterName, string parameterValue);
    object? ExecuteScalarSql(string query, IDictionary<string, object?> parameters);
    object? ExecuteScalarSql(
        string query,
        IReadOnlyCollection<string> parameterNames,
        IReadOnlyCollection<object?> parameterValues);
    object? ExecuteScalarSql(
        string query,
        IReadOnlyCollection<string> parameterNames,
        IReadOnlyCollection<object?> parameterValues,
        string collectionParameterName,
        IEnumerable collectionParameterValues);

    IList GetListSql(string query);
    IList GetListSql(string query, Type entityType);
    IList GetListSql(string query, string parameterName, string parameterValue);
    IList GetListSql(string parameterName, string parameterValue, string query, Type entityType);
    IList GetListSql(
        string query,
        IReadOnlyCollection<string> parameterNames,
        IReadOnlyCollection<object?> parameterValues);
    IList GetListSql(
        string query,
        IReadOnlyCollection<string> parameterNames,
        IReadOnlyCollection<object?> parameterValues,
        Type entityType);
    IList GetListSql(string query, string parameterName, IEnumerable parameterValues, Type entityType);
    IList GetListSql(
        string query,
        IReadOnlyCollection<string> parameterNames,
        IReadOnlyCollection<object?> parameterValues,
        string collectionParameterName,
        IEnumerable collectionParameterValues);
    IList GetListSql(
        string query,
        IReadOnlyCollection<string> parameterNames,
        IReadOnlyCollection<object?> parameterValues,
        string collectionParameterName,
        IEnumerable collectionParameterValues,
        Type entityType);

    IList GetListHql(string query);
    IList GetListHql(string query, string parameterName, string parameterValue);
    IList GetListHql(
        string query,
        IReadOnlyCollection<string> parameterNames,
        IReadOnlyCollection<object?> parameterValues);

    IList GetListAlias(string query);
    IList GetListAlias(
        string query,
        IReadOnlyCollection<string> parameterNames,
        IReadOnlyCollection<object?> parameterValues);
    IList GetListAlias(string query, string parameterName, string parameterValue);

    int ExecuteUpdate(string query);
    int ExecuteUpdate(string query, string parameterName, string parameterValue);
    int ExecuteUpdate(
        string query,
        IReadOnlyCollection<string> parameterNames,
        IReadOnlyCollection<object?> parameterValues);
    int ExecuteUpdate(string query, string parameterName, IEnumerable parameterValues);
}

public interface IDbManagerFactory
{
    IDbManager Create(ConfigFile configFile);
}

public static class DBUtils
{
    private static IDbManagerFactory? s_factory;

    public static void Configure(IDbManagerFactory factory)
    {
        s_factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    private static IDbManager CreateManager(ConfigFile configFile)
    {
        return (s_factory ?? throw new InvalidOperationException(
            "No database manager factory is configured. Call DBUtils.Configure(...) before using DBUtils."))
            .Create(configFile);
    }

    private static IReadOnlyCollection<string> Names(IEnumerable? names)
    {
        return names?.Cast<object>().Select(value => value.ToString()!)
            .ToArray() ?? Array.Empty<string>();
    }

    private static IReadOnlyCollection<object?> Values(IEnumerable? values)
    {
        return values?.Cast<object?>().ToArray() ?? Array.Empty<object?>();
    }

    public static IList GetList(
        string query,
        IList parameterNames,
        IList parameterValues,
        Type entityType,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(query, Names(parameterNames), Values(parameterValues), entityType);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(string query, IList parameterNames, IList parameterValues, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(query, Names(parameterNames), Values(parameterValues));
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(
        string query,
        string[] parameterNames,
        string[] parameterValues,
        Type entityType,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(query, parameterNames, parameterValues, entityType);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(
        string query,
        IList parameterNames,
        IList parameterValues,
        string collectionParameterName,
        IEnumerable collectionParameterValues,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(
                query,
                Names(parameterNames),
                Values(parameterValues),
                collectionParameterName,
                collectionParameterValues);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(
        string query,
        string[] parameterNames,
        string[] parameterValues,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(query, parameterNames, parameterValues);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(
        string query,
        string parameterName,
        string parameterValue,
        Type entityType,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(parameterName, parameterValue, query, entityType);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(
        string query,
        string parameterName,
        string parameterValue,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(query, parameterName, parameterValue);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(
        string query,
        string[] parameterNames,
        string[] parameterValues,
        string collectionParameterName,
        IEnumerable collectionParameterValues,
        Type entityType,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(
                query,
                parameterNames,
                parameterValues,
                collectionParameterName,
                collectionParameterValues,
                entityType);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetListHql(string query, string parameterName, string parameterValue, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListHql(query, parameterName, parameterValue);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetListHql(string query, IList parameterNames, IList parameterValues, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListHql(query, Names(parameterNames), Values(parameterValues));
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetListHql(string query, string[] parameterNames, string[] parameterValues, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListHql(query, parameterNames, parameterValues);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetListHql(string query, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListHql(query);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static object? GetObject(Type entityType, object id, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetObject(entityType, id);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static object? GetObject(
        string query,
        string[] parameterNames,
        string[] parameterValues,
        string collectionParameterName,
        IEnumerable collectionParameterValues,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteScalarSql(
                query,
                parameterNames,
                parameterValues,
                collectionParameterName,
                collectionParameterValues);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static object? GetObjectHql(string query, string parameterName, string parameterValue, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetObjectHql(query, parameterName, parameterValue);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static object? GetObjectHql(string query, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetObjectHql(query);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static object? GetObject(string query, IList parameterNames, IList parameterValues, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteScalarSql(query, Names(parameterNames), Values(parameterValues));
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static object? GetObject(string query, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteScalarSql(query);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static object? GetObject(string query, string parameterName, string parameterValue, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteScalarSql(query, parameterName, parameterValue);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static object? GetObject(
        string query,
        string[] parameterNames,
        string[] parameterValues,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteScalarSql(query, parameterNames, parameterValues);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(Type entityType, string query, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(query, entityType);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(Type entityType, string query, string parameterName, string parameterValue, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(parameterName, parameterValue, query, entityType);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(
        Type entityType,
        string query,
        string[] parameterNames,
        string[] parameterValues,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(query, parameterNames, parameterValues, entityType);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(
        Type entityType,
        string query,
        string parameterName,
        IEnumerable parameterValues,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(query, parameterName, parameterValues, entityType);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetList(string query, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListSql(query);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IEnumerator GetIterator(string query, ConfigFile configFile)
    {
        return GetList(query, configFile).GetEnumerator();
    }

    public static int Save(object record, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.BeginTransaction();
        try
        {
            manager.Save(record);
            return manager.CommitTransaction();
        }
        catch
        {
            manager.RollBack();
            throw;
        }
    }

    public static int SaveOrUpdateEntity(object record, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.BeginTransaction();
        try
        {
            manager.SaveOrUpdate(record);
            return manager.CommitTransaction();
        }
        catch
        {
            manager.RollBack();
            throw;
        }
    }

    public static long Save(string query, IList parameterNames, IList parameterValues, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteUpdate(query, Names(parameterNames), Values(parameterValues));
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static long Save(string query, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteUpdate(query);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static int ExecuteUpdate(string query, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteUpdate(query);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static int ExecuteUpdate(string query, IList parameterNames, IList parameterValues, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.BeginTransaction();
        try
        {
            return manager.ExecuteUpdate(query, Names(parameterNames), Values(parameterValues));
        }
        finally
        {
            manager.CommitTransaction();
        }
    }

    public static int ExecuteUpdate(string query, string[] parameterNames, string[] parameterValues, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteUpdate(query, parameterNames, parameterValues);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static long ExecuteUpdate(string query, string parameterName, string parameterValue, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteUpdate(query, parameterName, parameterValue);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static long ExecuteUpdate(string query, string parameterName, IEnumerable parameterValues, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.ExecuteUpdate(query, parameterName, parameterValues);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static long Delete(string query, ConfigFile configFile) => ExecuteUpdate(query, configFile);

    public static int Delete(string query, IList parameterNames, IList parameterValues, ConfigFile configFile)
        => ExecuteUpdate(query, parameterNames, parameterValues, configFile);

    public static long Delete(string query, string[] parameterNames, string[] parameterValues, ConfigFile configFile)
        => ExecuteUpdate(query, parameterNames, parameterValues, configFile);

    public static long Delete(string query, string parameterName, string parameterValue, ConfigFile configFile)
        => ExecuteUpdate(query, parameterName, parameterValue, configFile);

    public static long Delete(string query, string parameterName, IEnumerable parameterValues, ConfigFile configFile)
        => ExecuteUpdate(query, parameterName, parameterValues, configFile);

    public static int Delete(object record, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.BeginTransaction();
        try
        {
            manager.Delete(record);
            return manager.CommitTransaction();
        }
        catch
        {
            manager.RollBack();
            throw;
        }
    }

    public static bool Update(object record, ConfigFile configFile)
        => Update(new object[] { record }, configFile);

    public static bool Update(IEnumerable<object> records, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        try
        {
            manager.BeginTransaction();
            foreach (object? record in records)
            {
                if (record is not null)
                    manager.Update(record);
            }
            manager.CommitTransaction();
            return true;
        }
        catch (Exception)
        {
            manager.RollBack();
            return false;
        }
    }

    public static bool SaveOrUpdate(object record, ConfigFile configFile)
        => SaveOrUpdate(new object[] { record }, configFile);

    public static bool SaveOrUpdate(IEnumerable<object> records, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        try
        {
            manager.BeginTransaction();
            foreach (object? record in records)
            {
                if (record is not null)
                    manager.SaveOrUpdate(record);
            }
            manager.CommitTransaction();
            return true;
        }
        catch (Exception)
        {
            manager.RollBack();
            return false;
        }
    }

    public static IList GetListAlias(string query, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListAlias(query);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetListAlias(
        string query,
        IList parameterNames,
        IList parameterValues,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListAlias(query, Names(parameterNames), Values(parameterValues));
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static IList GetListAlias(
        string query,
        string parameterName,
        string parameterValue,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            return manager.GetListAlias(query, parameterName, parameterValue);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static int UpdateEntity(object record, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.BeginTransaction();
        try
        {
            manager.Update(record);
            manager.CommitTransaction();
            return 1;
        }
        catch
        {
            manager.RollBack();
            throw;
        }
    }

    public static void ExecuteNonQuery(string sql, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.OpenSession();
        try
        {
            manager.ExecuteUpdate(sql);
        }
        finally
        {
            manager.CloseSession();
        }
    }

    public static void ExecuteNonQuery(string query)
        => ExecuteNonQuery(query, ConfigFile.Applicant_Test);

    public static void ExecuteQuery(
        string query,
        string parameterName,
        string parameterValue,
        ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.BeginTransaction();
        try
        {
            manager.ExecuteUpdate(query, parameterName, parameterValue);
            manager.CommitTransaction();
        }
        catch
        {
            manager.RollBack();
            throw;
        }
    }

    public static void ExecuteQuery(string query, ConfigFile configFile)
    {
        using IDbManager manager = CreateManager(configFile);
        manager.BeginTransaction();
        try
        {
            manager.ExecuteUpdate(query);
            manager.CommitTransaction();
        }
        catch
        {
            manager.RollBack();
            throw;
        }
    }
}
