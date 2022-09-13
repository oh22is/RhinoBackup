using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.Core.Interfaces.OutputAdapter
{
  public interface IOutputWriter
  {
    public Task WriteAsync(RhinoExportData exportData);
  }
}
