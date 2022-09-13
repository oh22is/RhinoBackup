using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using oh22.RhinoBackup.Application.Interfaces;
using oh22.RhinoBackup.Application.Models;
using oh22.RhinoBackup.Core.Interfaces;
using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup
{
  public static class IServiceCollectionExtension
  {
    public static IServiceCollection AddOptions(this IServiceCollection self, string[] args)
    {
      var config = self.BuildServiceProvider()
        .GetRequiredService<IConfiguration>();

      return self.Configure<OperationSettings>(config.GetSection("options"))
        .Configure<MsSqlSettings>(config.GetSection("sql"))
        .AddSingleton<IMsSqlSettings>(service => service.GetRequiredService<IOptions<MsSqlSettings>>().Value)
        .AddSingleton<IOperationSettings>(service => service.GetRequiredService<IOptions<OperationSettings>>().Value)
        .AddSingleton<IAppSettings>(_ =>
        {
          var parser = new Parser(config =>
          {
            config.CaseSensitive = false;
            config.IgnoreUnknownArguments = true;
            config.AutoHelp = true;
          });

          return parser.ParseArguments<AppSettings>(args).Value ?? new AppSettings();
        }
      );
    }
  }
}