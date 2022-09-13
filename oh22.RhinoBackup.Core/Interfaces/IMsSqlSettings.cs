namespace oh22.RhinoBackup.Core.Interfaces
{
  public interface IMsSqlSettings
  {
    public string? SqlConnection { get; set; }
    public string? SqlEndpoint { get; set; }
    public string? Database { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

    public bool IsValid();
  }
}