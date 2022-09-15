using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using oh22.RhinoBackup.Core.Interfaces;
using oh22.RhinoBackup.Core.Services;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace oh22.RhinoBackup.Application
{
  public static class Program
  {
    public static async Task Main(string[] args)
    {
      using var host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration((config) => config
          .AddEnvironmentVariables("RHINO_")
          .AddCommandLine((s) => s.Args = args.Except(new[] { "--export", "--import" }, StringComparer.InvariantCultureIgnoreCase))
        )
        .UseSerilog((context, logging) => logging.ReadFrom.Configuration(context.Configuration))
        .ConfigureServices((service) =>
        {
          _ = service
              .AddOptions(args)
              .AddSingleton<IDatabaseService, DatabaseService>()
              .AddSingleton<ISqlConnectionService, SqlConnectionService>()
              .AddExport()
              .AddImport()
              .AddHostedService<BackupService>();
        })
        .Build();

      try
      {
        await host.RunAsync();
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }
  }
}
