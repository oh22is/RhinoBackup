using Microsoft.Extensions.Logging;
using oh22.RhinoBackup.Core.Helpers;
using oh22.RhinoBackup.Core.Interfaces;
using oh22.RhinoBackup.Core.POCOs;
using PetaPoco;
using PetaPoco.Providers;

namespace oh22.RhinoBackup.Core.Services
{
  public class DatabaseService : IDatabaseService
  {
    private readonly ILogger<DatabaseService> _logger;

    private readonly IDatabase _db;
    private readonly ISqlConnectionService _sqlService;

    public DatabaseService(ILogger<DatabaseService> logger, ISqlConnectionService sqlService)
    {
      _logger = logger;
      _sqlService = sqlService;
      _db = DatabaseConfiguration.Build()
        .UsingConnection(_sqlService.GetSqlConnection())
        .UsingProvider<SqlServerMsDataDatabaseProvider>()
        .Create();

      _db.Connection.Open();
    }

    /// <summary>
    /// Executes sql query and returns TObject.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    /// <param name="sql"></param>
    public async Task<IEnumerable<TObject>> QueryAsync<TObject>(string sql)
    {
      return _db
        .Fetch<TObject>(sql)
        .ToList();
    }

    /// <summary>
    /// Executes a list of sql commands.
    /// Rolls back if rollbackOnFail is set true.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="rollbackOnFail"></param>
    public async Task ExecuteAsync(IEnumerable<string> sql, bool rollbackOnFail = true)
    {
      await _db.BeginTransactionAsync();

      _logger.LogDebug("Executing Query Strings");

      try
      {
        int index = 0;
        foreach (string query in sql)
        {
          try
          {
            _logger.LogDebug("Executing {Index} Query String", index);
            string escapedQuery = PetaPocoHelper.Escape(query);
            _ = await _db.ExecuteAsync(escapedQuery);
            _logger.LogDebug("Executed {Index} Query String", index);
            index++;
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, "SQL-Execution failed on Script {Index}:\n{QueryString}", index, query);
            throw;
          }
        }

        _db.CompleteTransaction();

        _logger.LogDebug("Query Strings executed");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Query Strings execution failed");

        if (rollbackOnFail)
        {
          _logger.LogInformation("Performing rollback");

          try
          {
            _db.AbortTransaction();

            _logger.LogInformation("Rollback finished");
          }
          catch (Exception rollbackEx)
          {
            _logger.LogError(rollbackEx, "Rollback failed");

            throw;
          }
        }
        else
        {
          _logger.LogDebug("Continuing");
        }

        throw;
      }
    }

    /// <summary>
    /// Returns dependencies of database.
    /// </summary>
    public async Task<IEnumerable<Dependency>> GetDependenciesAsync()
    {
      string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      string filePath = Path.Combine(baseDirectory, "Resources", "dependencies.sql");
      string sqlQuery = await File.ReadAllTextAsync(filePath);

      return await QueryAsync<Dependency>(sqlQuery);
    }
  }
}
