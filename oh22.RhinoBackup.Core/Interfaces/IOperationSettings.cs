namespace oh22.RhinoBackup.Core.Interfaces
{
  public interface IOperationSettings
  {
    public bool? DoExternalDataSources { get; set; }
    public bool? DoExternalFileFormats { get; set; }
    public bool? DoExternalTables { get; set; }
    public bool? DoFunctions { get; set; }
    public bool? DoSchemas { get; set; }
    public bool? DoStoredProcedures { get; set; }
    public bool? DoViews { get; set; }
    public bool? AreSystemObjects { get; set; }
    public IEnumerable<string> ExcludedSchemas { get; set; }
  }
}