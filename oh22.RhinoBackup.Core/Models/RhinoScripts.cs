using Microsoft.SqlServer.Management.Smo;
using oh22.RhinoBackup.Core.Extensions;

namespace oh22.RhinoBackup.Core.Models
{
  public class RhinoScripts
  {
    public string Name { get; set; } = string.Empty;
    public string Schema { get; set; } = string.Empty;
    public string AlterScript { get; set; } = string.Empty;
    public string CreateScript { get; set; } = string.Empty;
    public string DropScript { get; set; } = string.Empty;
    public string QualifiedName => string.IsNullOrWhiteSpace(Schema)
        ? Name
        : $"{Schema}.{Name}";

    public static RhinoScripts CreateFromScriptable<T>(T scriptable) where T : NamedSmoObject, IScriptable
    {
      return new RhinoScripts
      {
        Name = scriptable.Name,
        Schema = scriptable is ScriptSchemaObjectBase schemaObject
            ? schemaObject.Schema
            : string.Empty,
        AlterScript = scriptable.GetAlter(),
        CreateScript = scriptable.GetCreate(),
        DropScript = scriptable.GetDrop()
      };
    }
  }
}