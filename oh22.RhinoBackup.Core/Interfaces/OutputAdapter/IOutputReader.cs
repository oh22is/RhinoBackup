using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.Core.Interfaces.OutputAdapter
{
  public interface IOutputReader
  {
    public Task<RhinoExportData> ReadAsync();
  }
}
