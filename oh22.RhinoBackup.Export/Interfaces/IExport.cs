using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.Export.Interfaces
{
  public interface IExport
  {
    public Task<RhinoExportData> ExecuteAsync();
  }
}
