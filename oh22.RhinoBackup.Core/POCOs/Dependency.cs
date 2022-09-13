using oh22.RhinoBackup.Core.Interfaces;
using PetaPoco;

namespace oh22.RhinoBackup.Core.POCOs
{
  public class Dependency : IDependency
  {
    [Column("schema_name")]
    public string SchemaName { get; set; } = string.Empty;
    [Column("view_name")]
    public string ViewName { get; set; } = string.Empty;
    [Column("referenced_schema_name")]
    public string ReferenceSchemaName { get; set; } = string.Empty;
    [Column("referenced_entity_name")]
    public string ReferenceEntityName { get; set; } = string.Empty;
    [Column("entity_type")]
    public string EntityType { get; set; } = string.Empty;

    public override int GetHashCode()
    {
      return HashCode.Combine(EntityType, SchemaName, ViewName, ReferenceSchemaName, ReferenceEntityName);
    }
  }
}
