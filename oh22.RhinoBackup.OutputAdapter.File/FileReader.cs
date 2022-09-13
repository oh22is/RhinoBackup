using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.OutputAdapter.File
{
  internal class FileReader
  {
    private readonly string _filePath;

    internal FileReader(string filePath)
    {
      _filePath = filePath;
    }

    internal async Task<RhinoExportData> ReadAsync()
    {
      string json = await System.IO.File.ReadAllTextAsync(_filePath);
      return RhinoExportData.DeserializeJson(json);
    }
  }
}