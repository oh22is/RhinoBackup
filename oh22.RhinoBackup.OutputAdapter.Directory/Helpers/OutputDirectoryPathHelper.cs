namespace oh22.RhinoBackup.OutputAdapter.Directory.Helpers
{
  internal class OutputDirectoryPathHelper
  {
    private readonly string _outputPath;

    internal OutputDirectoryPathHelper(string outputPath)
    {
      _outputPath = outputPath;
    }
    internal string GetPath()
    {
      return GetPath(string.Empty);
    }

    internal string GetPath(string relativePath)
    {
      string outputPath = Path.Combine(_outputPath, relativePath);
      return Path.GetFullPath(outputPath);
    }
  }
}
