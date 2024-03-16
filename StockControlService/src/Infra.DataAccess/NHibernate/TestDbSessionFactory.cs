using System.Reflection;
using Common.DataAccess;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

public class TestDbSessionFactory
{
    private readonly ILogger<TestDbSessionFactory> _logger;
    private static ISessionFactory _sessionFactory;

    public static TestDbSessionFactory Factory(ILogger<TestDbSessionFactory> logger)
        => new TestDbSessionFactory(logger);

    public TestDbSessionFactory(
        ILogger<TestDbSessionFactory> logger)
    {
        _logger = logger;
    }

    public static ISession OpenSession()
    {
        return _sessionFactory.OpenSession();
    }

    public ISessionFactory CreateSessionFactory(string connectionString)
    {
        _logger.LogInformation("Connecting to the database {DatabaseName}...", GetDatabaseName(connectionString));

        ValidateConnectionString(connectionString);

        var configuration = Fluently.Configure()
            .Database(PostgreSQLConfiguration
                .PostgreSQL82.ConnectionString(connectionString)
            )
            .Mappings(m => m.FluentMappings
                .AddFromAssembly(Assembly.GetExecutingAssembly()))
            .ExposeConfiguration(BuildSchema);

        try
        {
            _sessionFactory = configuration.BuildSessionFactory();
            _logger.LogInformation("Database {DatabaseName} has been successfully connected",
                GetDatabaseName(connectionString));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "The connection with the database {DatabaseName} failed",
                GetDatabaseName(connectionString));

            throw new DataAccessLayerException(e.Message);
        }

        return _sessionFactory;
    }

    public static void RunSqlScript(
        ILogger<TestDbSessionFactory> logger,
        string connectionString,
        string sqlStatements)
    {
        if (_sessionFactory == null)
        {
            var factory = new TestDbSessionFactory(logger);
            _sessionFactory = factory.CreateSessionFactory(connectionString);
        }

        using var session = _sessionFactory.OpenSession();
        session.CreateSQLQuery(sqlStatements)
            .ExecuteUpdate();
    }

    private static void BuildSchema(Configuration cfg)
    {
        var schemaUpdate = new SchemaUpdate(cfg);
        schemaUpdate.Execute(false, true);
    }

    private static void ValidateConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new DataAccessLayerException($"Invalid connection string {connectionString}.");

        var dbStartIndex = connectionString.IndexOf("Database=", StringComparison.Ordinal) + 9;
        var dbLength = connectionString.IndexOf(";", dbStartIndex, StringComparison.Ordinal) - dbStartIndex;
        var databaseName = connectionString.Substring(dbStartIndex, dbLength);

        if (databaseName.IndexOf("test", StringComparison.Ordinal) < 0)
            throw new DataAccessLayerException(
                $"Invalid Connection String: {connectionString}. For test purposes, the word 'test' must be on database name");
    }

    private static string GetDatabaseName(string connectionString)
    {
        var serverStartIndex = connectionString.IndexOf("Server=", StringComparison.Ordinal) + 7;
        var serverLength = connectionString.IndexOf(";", serverStartIndex, StringComparison.Ordinal) - serverStartIndex;

        var dbStartIndex = connectionString.IndexOf("Database=", StringComparison.Ordinal) + 9;
        var dbLength = connectionString.IndexOf(";", dbStartIndex, StringComparison.Ordinal) - dbStartIndex;

        var databaseName = connectionString.Substring(serverStartIndex, serverLength) +
                           "." + connectionString.Substring(dbStartIndex, dbLength);

        return databaseName;
    }
}