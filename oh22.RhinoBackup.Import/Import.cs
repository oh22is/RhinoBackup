using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Smo;
using oh22.RhinoBackup.Core.Models;
using oh22.RhinoBackup.Core.Interfaces;
using oh22.RhinoBackup.Import.Interfaces;

namespace oh22.RhinoBackup.Import
{
  public class Import : IImport
  {
    private readonly IDatabaseService _databaseHelper;
    private readonly ILogger<Import> _logger;
    private readonly IOperationSettings _settings;
    private readonly HashSet<int> _visited = new HashSet<int>();

    public Import(ILogger<Import> logger, IDatabaseService databaseHelper, IOperationSettings settings)
    {
      _logger = logger;
      _databaseHelper = databaseHelper;
      _settings = settings;
    }

    public async Task ExecuteAsync(RhinoExportData order)
    {
      if (!order.Entries.Any())
      {
        _logger.LogInformation("No files to import found");
        return;
      }

      var entries = order.Entries.Where(node => !IsExcludedSchema(node));

      using (_logger.BeginScope("Import"))
      {
        _logger.LogInformation("Import started");
        try
        {
          await ImportAsync(entries, true);
          _logger.LogInformation("Import succeeded");
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Import failed");
          throw;
        }
      }
    }

    private async Task ImportAsync(IEnumerable<Node<SqlEntry>> nodes, bool rollbackOnFail = false)
    {
      var orderedEntries = nodes.SelectMany(SolveOrder).ToList();

      var createScripts = orderedEntries.Where(s => !string.IsNullOrWhiteSpace(s.CreateScript))
        .Select(e => e.CreateScript);

      var dropScripts = orderedEntries.Where(s => !string.IsNullOrWhiteSpace(s.DropScript))
        .Select(e => e.DropScript)
        .Reverse();

      var alterScripts = orderedEntries.Where(s => !string.IsNullOrWhiteSpace(s.AlterScript))
        .Select(e => e.AlterScript);

      await ExecuteScriptsAsync(dropScripts, rollbackOnFail);
      await ExecuteScriptsAsync(createScripts, rollbackOnFail);
      await ExecuteScriptsAsync(alterScripts, rollbackOnFail);
    }

    private async Task ExecuteScriptsAsync(IEnumerable<string> paths, bool rollbackOnFail = false)
    {
      var queryStrings = paths.Where(s => !string.IsNullOrWhiteSpace(s));

      await _databaseHelper.ExecuteAsync(queryStrings, rollbackOnFail);
    }

    /// <summary>
    /// Creates a ordered enumerable of sql import entries.
    /// </summary>
    /// <param name="node"></param>
    private List<SqlEntry> SolveOrder(Node<SqlEntry> node)
    {
      var result = new List<SqlEntry>();
      if (node.Value == null || _visited.Contains(node.Value.GetHashCode()))
      {
        return result;
      }

      foreach (var referenceEntries in node.References.Select(SolveOrder))
      {
        result.AddRange(referenceEntries);
      }

      result.Add(node.Value);
      _ = _visited.Add(node.Value.GetHashCode());

      return result;
    }

    private bool IsExcludedSchema(Node<SqlEntry> node)
    {
      var entry = node.Value;
      if (entry == null) { return true; }
      bool isSchemaEntry = entry.Type == nameof(Schema);
      bool isExcludedEntry = _settings.ExcludedSchemas.Contains(entry.Name);
      bool isExcludedSchema = _settings.ExcludedSchemas.Any(s => s.Equals(entry.Schema, StringComparison.OrdinalIgnoreCase));

      return (isSchemaEntry && isExcludedEntry) || isExcludedSchema;
    }
  }
}
