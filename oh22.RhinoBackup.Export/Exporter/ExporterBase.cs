using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Smo;
using oh22.RhinoBackup.Core.Helpers;
using oh22.RhinoBackup.Core.Interfaces;
using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.Export.Helpers.Export
{
  public class ExporterBase<T> where T : NamedSmoObject, IScriptable
  {
    protected IOperationSettings Settings { get; }
    protected ILogger<ExporterBase<T>> Logger { get; }

    /// <summary>
    /// An abstract class for exporting sql objects.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="logger">The logger.</param>
    public ExporterBase(IOperationSettings settings, ILogger<ExporterBase<T>> logger)
    {
      Settings = settings;
      Logger = logger;
    }

    protected IEnumerable<T> GetObjectBases(SmoCollectionBase collection)
    {
      return collection.OfType<T>();
    }

    private static bool NoneOrEqualSystemObject(T smoObject, bool? isSystemObject)
    {
      return smoObject switch
      {
        Schema schema => schema.IsSystemObject == isSystemObject,
        StoredProcedure storedProcedure => storedProcedure.IsSystemObject == isSystemObject,
        Table table => table.IsSystemObject == isSystemObject,
        UserDefinedFunction userDefinedFunction => userDefinedFunction.IsSystemObject == isSystemObject,
        View view => view.IsSystemObject == isSystemObject,
        _ => true,
      };
    }

    /// <summary>
    /// Starts the export.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="collection"></param>
    /// <returns>An enumerable of sql export entries.</returns>
    public async Task<IEnumerable<SqlEntry>> ExecuteAsync(string name, SmoCollectionBase collection)
    {
      using (Logger.BeginScope("{Name}", name))
      {
        Logger.LogInformation("Export {Name} started", name);
        try
        {
          var objectsBases = GetObjectBases(collection)
            .Where(smoObject => NoneOrEqualSystemObject(smoObject, Settings.AreSystemObjects)).ToList();

          var sqlExportEntries = await ExportNameObjectBaseAsync(objectsBases);

          LogSqlObjectCount(sqlExportEntries.Count);

          Logger.LogInformation("Export {Name} succeeded", name);

          return sqlExportEntries;
        }
        catch (Exception ex)
        {
          Logger.LogError(ex, "Export {Name} failed", name);
          throw;
        }
      }
    }

    private bool IsExcludedSchema(RhinoScripts scripts, bool isSchema)
    {
      string schema = scripts.Schema;
      if (isSchema)
      {
        schema = scripts.Name;
      }
      return Settings.ExcludedSchemas?.Contains(schema, StringComparer.OrdinalIgnoreCase) ?? false;
    }

    private async Task<IList<SqlEntry>> ExportNameObjectBaseAsync(IEnumerable<T> objectBases)
    {
      var sqlExportEntries = new List<SqlEntry>();

      foreach (var objectBase in objectBases)
      {
        var scripts = ExporterBase<T>.GetScripts(objectBase);

        if (IsExcludedSchema(scripts, objectBase is Schema))
        {
          continue;
        }

        string? schemaQualifiedName = scripts.QualifiedName;

        Logger.LogInformation("{Name}", schemaQualifiedName);

        try
        {
          var sqlExportEntry = SqlEntry.FromSmoObject<T>(
            scripts.Name,
            scripts.Schema,
            scripts.CreateScript,
            scripts.DropScript,
            scripts.AlterScript
          );

          if (!string.IsNullOrWhiteSpace(scripts.CreateScript))
          {
            var openRowSets = SqlStringHelper.GetOpenRowSets(scripts.CreateScript, scripts.QualifiedName);
            sqlExportEntry.Resource = new Resource(scripts.Name, openRowSets);
          }

          sqlExportEntries.Add(sqlExportEntry);
        }
        catch (Exception ex)
        {
          Logger.LogError(ex, "Error with {Name}", schemaQualifiedName);
        }
      }

      return sqlExportEntries;
    }

    private static RhinoScripts GetScripts(T scriptableObjectBase)
    {
      return RhinoScripts.CreateFromScriptable(scriptableObjectBase);
    }

    private void LogSqlObjectCount(int counter)
    {
      if (counter > 0)
      {
        return;
      }

      Logger.LogInformation("No SQL objects of this type were found");
      if (!Settings.AreSystemObjects.GetValueOrDefault())
      {
        Logger.LogInformation("Please check if system objects of this type should be exported");
      }
    }
  }
}