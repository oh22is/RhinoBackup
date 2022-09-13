using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using oh22.RhinoBackup.Core.Helpers;
using oh22.RhinoBackup.Core.Interfaces;
using oh22.RhinoBackup.Core.Models;
using oh22.RhinoBackup.Export.Extensions;
using oh22.RhinoBackup.Export.Helpers.Export;
using oh22.RhinoBackup.Export.Interfaces;

namespace oh22.RhinoBackup.Export
{
  public class Export : IExport
  {
    private readonly ILogger<Export> _logger;
    private readonly ILoggerFactory _loggerFactory;

    private readonly Database _db;
    private readonly IOperationSettings _settings;
    private readonly IDatabaseService _databaseService;

    private readonly IServiceProvider _serviceProvider;
    private readonly RhinoExportData _export = new RhinoExportData();

    /// <summary>
    /// Export class, which manages the backup export of the synapse sql server.
    /// </summary>
    /// <param name="loggerFactory">FActory for Creating Logger</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="sql">The app settings</param>
    /// <param name="databaseService">The database service</param>
    /// <param name="settings"></param>
    public Export(
      ILoggerFactory loggerFactory,
      IServiceProvider serviceProvider,
      ISqlConnectionService sql,
      IDatabaseService databaseService,
      IOperationSettings settings
    )
    {
      _logger = loggerFactory.CreateLogger<Export>();
      _loggerFactory = loggerFactory;
      _settings = settings;
      _databaseService = databaseService;

      _serviceProvider = serviceProvider;

      var sqlConnection = sql.GetSqlConnection();
      var serverConnection = new ServerConnection(sqlConnection);

      var server = new Server(serverConnection);
      _db = server.Databases[sqlConnection.Database];
    }

    public async Task<RhinoExportData> ExecuteAsync()
    {
      var sqlExportEntries = await ExportAsync();

      await HandleExportEntriesAsync(sqlExportEntries);
      await HandleExportDataSourcesAsync(sqlExportEntries);
      await HandleExportCredentialsAsync();
      await HandleDependenciesAsync(sqlExportEntries);
      HandleExportEntries(sqlExportEntries);

      return _export;
    }

    private void HandleExportEntries(IEnumerable<SqlEntry> sqlExportEntries)
    {
      _export.Entries = sqlExportEntries.CreateTree(_export.Dependencies);
    }

    private async Task HandleExportDataSourcesAsync(IEnumerable<SqlEntry> sqlExportEntries)
    {
      var dataSources = sqlExportEntries
        .Where(e => e.Type.Equals(nameof(ExternalDataSource), StringComparison.Ordinal))
        .SelectMany(e => SqlStringHelper.GetDataSources(e.CreateScript, e.Name));
      _export.DataSources = dataSources.ToList();
    }

    private async Task HandleExportCredentialsAsync()
    {
      _export.Credentials = await CreateInstance<DatabaseScopedCredential>()
        .ExecuteAsync("Credentials", _db.DatabaseScopedCredentials);
    }

    private async Task HandleDependenciesAsync(IEnumerable<SqlEntry> sqlExportEntries)
    {
      var entriesDependencies = sqlExportEntries
        .Where(entry => entry.UseExternalDataSource)
        .GetDependencies();
      var dependencies = new List<IDependency>(entriesDependencies);
      var dataBaseDependencies = await _databaseService.GetDependenciesAsync();

      dependencies.AddRange(dataBaseDependencies);
      _export.Dependencies = dependencies;
    }

    private async Task HandleExportEntriesAsync(IEnumerable<SqlEntry> sqlExportEntries)
    {
      var resources = sqlExportEntries
        .Where(entry => entry.Resource != null)
        .SelectMany(entry => entry.Resource?.OpenRowSets ?? Enumerable.Empty<OpenRowSet>());

      _export.OpenRowSets = resources.ToList();
    }

    private async Task<IEnumerable<SqlEntry>> ExportAsync()
    {
      var sqlExportEntries = new List<SqlEntry>();

      if (_settings.DoSchemas.GetValueOrDefault())
      {
        var entries = await CreateInstance<Schema>()
          .ExecuteAsync("Schemas", _db.Schemas);
        sqlExportEntries.AddRange(entries);
      }

      if (_settings.DoFunctions.GetValueOrDefault())
      {
        var entries = await CreateInstance<UserDefinedFunction>()
          .ExecuteAsync("User Defined Functions", _db.UserDefinedFunctions);
        sqlExportEntries.AddRange(entries);
      }

      if (_settings.DoExternalFileFormats.GetValueOrDefault())
      {
        var entries = await CreateInstance<ExternalFileFormat>()
          .ExecuteAsync("External File Formats", _db.ExternalFileFormats);
        sqlExportEntries.AddRange(entries);
      }

      if (_settings.DoStoredProcedures.GetValueOrDefault())
      {
        var entries = await CreateInstance<StoredProcedure>()
          .ExecuteAsync("Stored Procedures", _db.StoredProcedures);
        sqlExportEntries.AddRange(entries);
      }

      if (_settings.DoViews.GetValueOrDefault())
      {
        var entries = await CreateInstance<View>()
          .ExecuteAsync("Views ", _db.Views);
        sqlExportEntries.AddRange(entries);
      }

      if (_settings.DoExternalDataSources.GetValueOrDefault())
      {
        var entries = await CreateInstance<ExternalDataSource>()
          .ExecuteAsync("External Data Sources", _db.ExternalDataSources);
        sqlExportEntries.AddRange(entries);
      }

      if (_settings.DoExternalTables.GetValueOrDefault())
      {
        var entries = await CreateInstance<Table>()
          .ExecuteAsync("Table", _db.Tables);
        sqlExportEntries.AddRange(entries);
      }

      return sqlExportEntries;
    }

    private ExporterBase<T> CreateInstance<T>() where T : NamedSmoObject, IScriptable
    {
      var logger = _loggerFactory.CreateLogger<ExporterBase<T>>();
      return new ExporterBase<T>(_settings, logger);
    }
  }
}