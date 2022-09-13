using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.Import.Interfaces
{
  public interface IImport
  {
    public Task ExecuteAsync(RhinoExportData order);
  }
}
