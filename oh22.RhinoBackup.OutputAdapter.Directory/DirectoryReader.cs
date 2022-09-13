using Newtonsoft.Json;
using oh22.RhinoBackup.Core.Models;
using oh22.RhinoBackup.OutputAdapter.Directory.Helpers;

namespace oh22.RhinoBackup.OutputAdapter.Directory
{
  internal class DirectoryReader
  {
    private readonly OutputDirectoryPathHelper _outputDirectoryPathHelper;

    internal DirectoryReader(string outputPath)
    {
      _outputDirectoryPathHelper = new OutputDirectoryPathHelper(outputPath);
    }

    internal async Task<RhinoExportData> ReadAsync()
    {
      var nodes = await ReadEntryNodesAsync();

      foreach (var node in nodes)
      {
        var entry = node.Value;

        if (entry == null)
        {
          continue;
        }

        if (!string.IsNullOrEmpty(entry.CreateScript))
        {
          string script = await ReadScriptFromRelativePathAsync(entry.CreateScript);
          entry.CreateScript = script;
        }
        if (!string.IsNullOrEmpty(entry.DropScript))
        {
          string script = await ReadScriptFromRelativePathAsync(entry.DropScript);
          entry.DropScript = script;
        }
        if (!string.IsNullOrEmpty(entry.AlterScript))
        {
          string script = await ReadScriptFromRelativePathAsync(entry.AlterScript);
          entry.AlterScript = script;
        }
      }

      var data = new RhinoExportData
      {
        Entries = nodes
      };

      return data;
    }

    private async Task<string> ReadScriptFromRelativePathAsync(string relativePath)
    {
      string outputPath = _outputDirectoryPathHelper.GetPath(relativePath);
      return await File.ReadAllTextAsync(outputPath);
    }

    private async Task<IEnumerable<Node<SqlEntry>>> ReadEntryNodesAsync()
    {
      string filePath = _outputDirectoryPathHelper.GetPath("order.json");
      string json = await File.ReadAllTextAsync(filePath);
      return JsonConvert.DeserializeObject<IEnumerable<Node<SqlEntry>>>(json) ?? new List<Node<SqlEntry>>();
    }
  }
}