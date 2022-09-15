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

      await ExecuteScriptsAsync("Drop", dropScripts, rollbackOnFail);
      await ExecuteScriptsAsync("Create", createScripts, rollbackOnFail);
      await ExecuteScriptsAsync("Alter", alterScripts, rollbackOnFail);
    }

    private async Task ExecuteScriptsAsync(string typeName, IEnumerable<string> scripts, bool rollbackOnFail = false)
    {
      _logger.LogInformation("Starting {ScriptType} Scripts", typeName);
      var queryStrings = scripts.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
      await _databaseHelper.ExecuteAsync(queryStrings, rollbackOnFail);
      _logger.LogInformation("Finished {ScriptCount} {ScriptType} Scripts", queryStrings.Count, typeName);
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
