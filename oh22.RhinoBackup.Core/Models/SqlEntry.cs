using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using oh22.RhinoBackup.Core.Interfaces;

namespace oh22.RhinoBackup.Core.Models
{
  /// <summary>
  /// A sql export entry which contains important information for the backup.
  /// </summary>
  [JsonObject(IsReference = true)]
  public class SqlEntry
  {
    public string Type { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CreateScript { get; set; } = string.Empty;
    public string DropScript { get; set; } = string.Empty;
    public string AlterScript { get; set; } = string.Empty;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public Resource? Resource { get; set; }
    public bool UseExternalDataSource => Resource
      ?.OpenRowSets
      ?.Any(o => !string.IsNullOrWhiteSpace(o.DataSource)) ?? false;

    public SqlEntry() { }

    /// <summary>
    /// Creates a sql export entry which contains important information for the backup.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <param name="schema">The schema it belongs to, if any.</param>
    /// <param name="type">The type of the entity.</param>
    /// <param name="createScript">The relative filepath to the create script.</param>
    /// <param name="dropScript">The relative filepath to the drop script.</param>
    /// <param name="alterScript">The relative filepath to the alter script, if any exist.</param>
    [JsonConstructor]
    public SqlEntry(string name, string schema, string type, string createScript, string dropScript, string alterScript)
    {
      Name = name;
      Schema = schema;
      Type = type;
      CreateScript = createScript;
      DropScript = dropScript;
      AlterScript = alterScript;
    }

    /// <summary>
    /// Gets a sql export entry from a smo object.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="name">The name of the entity.</param>
    /// <param name="schema">The schema it belongs to, if any.</param>
    /// <param name="createScript">The relative filepath to the create script.</param>
    /// <param name="dropScript">The relative filepath to the alter script.</param>
    /// <param name="alterScript">The relative filepath to the create script, if any exist.</param>
    public static SqlEntry FromSmoObject<T>(string name, string schema = "", string createScript = "", string dropScript = "", string alterScript = "") where T : SqlSmoObject
    {
      string typeName = typeof(T).Name;
      return new SqlEntry(name, schema, typeName, createScript, dropScript, alterScript);
    }

    public override string ToString()
    {
      return string.IsNullOrWhiteSpace(Schema) ? Name : $"{Schema}.{Name}";
    }

    public override bool Equals(object? obj)
    {
      return obj is not null and SqlEntry entry
        ? Equals(entry.Schema, entry.Name)
        : obj is not null and IDependency dep && Equals(dep.SchemaName, dep.ViewName);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Schema, Name);
    }

    /// <summary>
    /// Determines whether a schema and name string have the same value as this, ordinal ignoring case.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="name"></param>
    /// <returns>True/False depending on wether the values are equal.</returns>
    public bool Equals(string schema, string name)
    {
      return Equals(schema, name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether a schema and name string have the same value as this.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="name"></param>
    /// <param name="stringComparison"></param>
    /// <returns>True/False depending on wether the values are equal.</returns>
    public bool Equals(string schema, string name, StringComparison stringComparison)
    {
      return Schema.Equals(schema, stringComparison) && Name.Equals(name, stringComparison);
    }
  }
}
