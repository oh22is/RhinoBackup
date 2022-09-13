using oh22.RhinoBackup.Application.Exceptions;
using oh22.RhinoBackup.Application.Interfaces;
using oh22.RhinoBackup.Core.Interfaces.OutputAdapter;

namespace oh22.RhinoBackup.Application.Helpers
{
  internal static class OutputAdapterHelper
  {
    internal static IOutputAdapter GetOutputAdapter(IAppSettings appSettings)
    {
      if (!string.IsNullOrWhiteSpace(appSettings.FilePath))
      {
        return new OutputAdapter.File.File(appSettings.FilePath);
      }
      else if (!string.IsNullOrWhiteSpace(appSettings.DirectoryPath))
      {
        return new OutputAdapter.Directory.Directory(appSettings.DirectoryPath);
      }
      throw new AdapterNotFoundException("Execution statement did not indicate a valid output adapter.");
    }
  }
}
