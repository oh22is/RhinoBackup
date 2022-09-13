using Microsoft.Extensions.DependencyInjection;
using oh22.RhinoBackup.Import.Interfaces;

namespace oh22.RhinoBackup
{
  public static class IServiceCollectionExtension
  {
    public static IServiceCollection AddImport(this IServiceCollection self)
    {
      return self.AddTransient<IImport, Import.Import>();
    }
  }
}