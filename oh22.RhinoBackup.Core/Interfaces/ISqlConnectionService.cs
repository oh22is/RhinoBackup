using Microsoft.Data.SqlClient;

namespace oh22.RhinoBackup.Core.Interfaces
{
  public interface ISqlConnectionService
  {
    public SqlConnection GetSqlConnection();
  }
}