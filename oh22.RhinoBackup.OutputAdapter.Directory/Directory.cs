using oh22.RhinoBackup.Core.Interfaces.OutputAdapter;
using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.OutputAdapter.Directory
{
  public class Directory : IOutputAdapter
  {
    private readonly string _outputPath;

    public Directory(string outputPath)
    {
      _outputPath = outputPath;
    }

    public async Task<RhinoExportData> ReadAsync()
    {
      var reader = new DirectoryReader(_outputPath);
      return await reader.ReadAsync();
    }

    public async Task WriteAsync(RhinoExportData exportData)
    {
      var writer = new DirectoryWriter(_outputPath);
      await writer.WriteAsync(exportData);
    }

    public override string ToString()
    {
      return $"{nameof(Directory)} ({_outputPath})";
    }
  }
}
