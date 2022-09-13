using Newtonsoft.Json;
namespace oh22.RhinoBackup.Core.Models
{
  public class Resource
  {
    public string Name { get; set; }
    public IEnumerable<OpenRowSet> OpenRowSets { get; set; }
    public IEnumerable<DataSource> DataSources { get; set; } = new List<DataSource>();

    [JsonConstructor]
    public Resource(string name)
    {
      Name = name;
      OpenRowSets = new List<OpenRowSet>();
    }

    public Resource(string name, IEnumerable<OpenRowSet> openRowSets)
    {
      Name = name;
      OpenRowSets = openRowSets;
    }
  }
}
