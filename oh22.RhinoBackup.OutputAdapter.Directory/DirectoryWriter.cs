using Newtonsoft.Json;
using oh22.RhinoBackup.Core.Models;
using oh22.RhinoBackup.OutputAdapter.Directory.Helpers;

namespace oh22.RhinoBackup.OutputAdapter.Directory
{
  internal class DirectoryWriter
  {
    private readonly OutputDirectoryPathHelper _outputDirectoryPathHelper;

    internal DirectoryWriter(string outputPath)
    {
      _ = System.IO.Directory.CreateDirectory(outputPath);

      _outputDirectoryPathHelper = new OutputDirectoryPathHelper(outputPath);
    }

    internal async Task WriteAsync(RhinoExportData exportData)
    {
      await WriteEntriesAsync(exportData.Entries);

      await WriteJsonAsync(_outputDirectoryPathHelper.GetPath(), "dataSources", exportData.DataSources);
      await WriteJsonAsync(_outputDirectoryPathHelper.GetPath(), "openRowSets", exportData.OpenRowSets);
      await WriteJsonAsync(_outputDirectoryPathHelper.GetPath(), "credentials", exportData.Credentials);
      await WriteJsonAsync(_outputDirectoryPathHelper.GetPath(), "order", exportData.Entries);
    }

    private async Task WriteGroupsAsync(string relativePath, IEnumerable<IGrouping<string, SqlEntry>> groups)
    {
      foreach (var group in groups)
      {
        string typeName = group.Key;

        string relativeGroupPath = Path.Combine(relativePath, typeName);
        string outputGroupPath = _outputDirectoryPathHelper.GetPath(relativeGroupPath);

        _ = System.IO.Directory.CreateDirectory(outputGroupPath);

        await WriteEntriesAsync(relativeGroupPath, group);
      }
    }

    private static async Task WriteJsonAsync(string path, string name, object content)
    {
      string json = JsonConvert.SerializeObject(content);
      string fileName = $"{name}.json";

      await WriteTextAsync(path, fileName, json);
    }

    private static async Task WriteTextAsync(string path, string name, string content)
    {
      string filePath = Path.Combine(path, name);

      await File.WriteAllTextAsync(filePath, content);
    }

    private async Task WriteEntriesAsync(IEnumerable<Node<SqlEntry>> sqlEntries)
    {
      var groups = sqlEntries
        .Where(e => e.Value != null)
        .Select(e => e.Value!)
        .GroupBy(e => e.Type)
        .ToList();

      if (groups.Count == 0)
      {
        return;
      }

      string outputSqlPath = _outputDirectoryPathHelper.GetPath("SQL");

      _ = System.IO.Directory.CreateDirectory(outputSqlPath);

      await WriteGroupsAsync(outputSqlPath, groups);
    }

    private async Task WriteEntriesAsync(string relativePath, IEnumerable<SqlEntry> entries)
    {
      foreach (var entry in entries)
      {
        string relativeDirectoryPath = string.IsNullOrWhiteSpace(entry.Schema) ? relativePath : Path.Combine(relativePath, entry.Schema);

        entry.CreateScript = await WriteScriptAsync(relativeDirectoryPath, "create", entry.Name, entry.CreateScript);
        entry.DropScript = await WriteScriptAsync(relativeDirectoryPath, "drop", entry.Name, entry.DropScript);
        entry.AlterScript = await WriteScriptAsync(relativeDirectoryPath, "alter", entry.Name, entry.AlterScript);
      }
    }

    private async Task<string> WriteScriptAsync(string relativePath, string subdirectory, string name, string script)
    {
      if (string.IsNullOrWhiteSpace(script))
      {
        return string.Empty;
      }

      string relativeFilePath = Path.Combine(relativePath, subdirectory, name);
      var file = new FileInfo(_outputDirectoryPathHelper.GetPath(relativeFilePath));
      file.Directory?.Create();
      using var writer = file.CreateText();
      await writer.WriteAsync(script);
      return relativeFilePath;
    }
  }
}