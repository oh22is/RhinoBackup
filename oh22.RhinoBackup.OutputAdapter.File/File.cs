using oh22.RhinoBackup.Core.Interfaces.OutputAdapter;
using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.OutputAdapter.File
{
  public class File : IOutputAdapter
  {
    private readonly string _filePath;

    public File(string filePath)
    {
      _filePath = filePath;
    }

    public async Task<RhinoExportData> ReadAsync()
    {
      var reader = new FileReader(_filePath);
      return await reader.ReadAsync();
    }

    public async Task WriteAsync(RhinoExportData exportData)
    {
      var writer = new FileWriter(_filePath);
      await writer.WriteAsync(exportData);
    }

    public override string ToString()
    {
      return $"{nameof(Directory)} ({_filePath})";
    }
  }
}
