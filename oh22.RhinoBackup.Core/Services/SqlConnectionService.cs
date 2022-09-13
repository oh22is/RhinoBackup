using Microsoft.Data.SqlClient;
using oh22.RhinoBackup.Core.Interfaces;

namespace oh22.RhinoBackup.Core.Services
{
  public class SqlConnectionService : ISqlConnectionService
  {
    private readonly IMsSqlSettings _settings;
    private readonly SqlConnectionStringBuilder _builder;
    public SqlConnectionService(IMsSqlSettings settings)
    {
      if (!settings.IsValid())
      {
        throw new ArgumentException("MS SQL Connection Settings are not valid.", nameof(settings));
      }
      _settings = settings;
      _builder = GetBuilder();
    }

    private SqlConnectionStringBuilder GetBuilder()
    {
      var builder = string.IsNullOrWhiteSpace(_settings.SqlConnection)
        ? GetBuilderFromProperties()
        : GetBuilderFromConnectionString();

      builder.ApplicationName = "oh22.RhinoBackup";

      return builder;
    }

    private SqlConnectionStringBuilder GetBuilderFromConnectionString()
    {
      return new SqlConnectionStringBuilder(_settings.SqlConnection);
    }

    private SqlConnectionStringBuilder GetBuilderFromProperties()
    {
      return new SqlConnectionStringBuilder()
      {
        DataSource = _settings.SqlEndpoint,
        Authentication = SqlAuthenticationMethod.SqlPassword,
        UserID = _settings.Username,
        Password = _settings.Password,
        InitialCatalog = _settings.Database,
        MultipleActiveResultSets = true
      };
    }

    public SqlConnection GetSqlConnection()
    {
      return new SqlConnection(_builder.ConnectionString);
    }
  }
}