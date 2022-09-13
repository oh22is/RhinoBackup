using Microsoft.Extensions.DependencyInjection;
using oh22.RhinoBackup.Export.Interfaces;

namespace oh22.RhinoBackup
{
  public static class IServiceCollectionExtension
  {
    public static IServiceCollection AddExport(this IServiceCollection self)
    {
      return self.AddTransient<IExport, Export.Export>();
    }
  }
}