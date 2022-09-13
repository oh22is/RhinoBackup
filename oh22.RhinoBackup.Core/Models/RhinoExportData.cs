using Newtonsoft.Json;
using oh22.RhinoBackup.Core.Interfaces;
using oh22.RhinoBackup.Core.POCOs;

namespace oh22.RhinoBackup.Core.Models
{
  public class RhinoExportData
  {
    public IEnumerable<SqlEntry> Credentials { get; set; } = new List<SqlEntry>();
    public IEnumerable<DataSource> DataSources { get; set; } = new List<DataSource>();
    public IEnumerable<OpenRowSet> OpenRowSets { get; set; } = new List<OpenRowSet>();
    public IEnumerable<Node<SqlEntry>> Entries { get; set; } = new List<Node<SqlEntry>>();
    public IEnumerable<IDependency> Dependencies { get; set; } = new List<Dependency>();

    /// <summary>
    /// Serializes Object into json String
    /// </summary>
    /// <returns>Returns json string</returns>
    public string SerializeJson()
    {
      var serializerSettings = new JsonSerializerSettings()
      {
        PreserveReferencesHandling = PreserveReferencesHandling.Objects
      };

      return JsonConvert.SerializeObject(this, Formatting.Indented, serializerSettings);
    }

    /// <summary>
    /// Deserializes the String into Rhino Object
    /// </summary>
    /// <param name="json"></param>
    /// <returns>Rhino Object</returns>
    public static RhinoExportData DeserializeJson(string json)
    {
      var serializerSettings = new JsonSerializerSettings
      {
        PreserveReferencesHandling = PreserveReferencesHandling.Objects
      };

      return JsonConvert.DeserializeObject<RhinoExportData>(json, serializerSettings) ?? new RhinoExportData();
    }
  }
}