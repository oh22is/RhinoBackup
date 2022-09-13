using Newtonsoft.Json;
using oh22.RhinoBackup.Application.Test.Properties;
using oh22.RhinoBackup.Core.Helpers;

namespace oh22.RhinoBackup.Application.Test
{
  public class SqlStringHelperTest
  {
    private readonly string _localPath = AppDomain.CurrentDomain.BaseDirectory;

    [Fact]
    public async Task TestGetOpenRowSetAsync()
    {
      string sql = Resources.OpenRowSetsExampleSql;

      var resources = SqlStringHelper.GetOpenRowSets(sql, "OpenRowSetsExample");

      string sqlJson = JsonConvert.SerializeObject(resources);

      string json = Resources.OpenRowSetsExampleJson;

      Assert.Equal(json, sqlJson);
    }
  }
}
