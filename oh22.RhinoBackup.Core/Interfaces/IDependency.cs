namespace oh22.RhinoBackup.Core.Interfaces
{
  public interface IDependency
  {
    public string SchemaName { get; set; }
    public string ViewName { get; set; }
    public string ReferenceSchemaName { get; set; }
    public string ReferenceEntityName { get; set; }
    public string EntityType { get; set; }
  }
}
