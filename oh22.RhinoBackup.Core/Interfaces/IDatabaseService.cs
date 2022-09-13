using oh22.RhinoBackup.Core.POCOs;

namespace oh22.RhinoBackup.Core.Interfaces
{
  public interface IDatabaseService
  {
    public Task<IEnumerable<TObject>> QueryAsync<TObject>(string sql);
    public Task ExecuteAsync(IEnumerable<string> sql, bool rollbackOnFail = true);
    public Task<IEnumerable<Dependency>> GetDependenciesAsync();
  }
}
