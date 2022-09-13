using Newtonsoft.Json;

namespace oh22.RhinoBackup.Core.Models
{
  public class OpenRowSet
  {
    public string Name { get; set; }
    public string Bulk { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? DataSource { get; set; }

    [JsonConstructor]
    public OpenRowSet(string name, string bulk, string? dataSource = null)
    {
      Name = name;
      Bulk = bulk;
      DataSource = dataSource;
    }
  }
}
