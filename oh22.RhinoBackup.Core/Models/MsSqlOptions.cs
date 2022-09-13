using oh22.RhinoBackup.Core.Interfaces;

namespace oh22.RhinoBackup.Core.Models
{
  public class MsSqlSettings : IMsSqlSettings
  {
    public string? SqlConnection { get; set; }
    public string? SqlEndpoint { get; set; }
    public string? Database { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

    public bool IsValid()
    {
      bool isConnectionString = !string.IsNullOrWhiteSpace(SqlConnection);
      bool isParams = !string.IsNullOrWhiteSpace(SqlEndpoint)
        && !string.IsNullOrWhiteSpace(Database)
        && !string.IsNullOrWhiteSpace(Username)
        && !string.IsNullOrWhiteSpace(Password);

      return isParams || isConnectionString;
    }
  }
}