using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.OutputAdapter.File
{
  internal class FileWriter
  {
    private readonly string _filePath;

    internal FileWriter(string filePath)
    {
      _filePath = filePath;
    }

    internal async Task WriteAsync(RhinoExportData exportData)
    {
      string json = exportData.SerializeJson();
      await WriteFile(json);
    }

    private async Task WriteFile(string json)
    {
      await System.IO.File.WriteAllTextAsync(_filePath, json);
    }
  }
}