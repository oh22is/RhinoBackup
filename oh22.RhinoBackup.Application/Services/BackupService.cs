using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using oh22.RhinoBackup.Application.Enums;
using oh22.RhinoBackup.Application.Helpers;
using oh22.RhinoBackup.Application.Interfaces;
using oh22.RhinoBackup.Core.Interfaces.OutputAdapter;
using oh22.RhinoBackup.Export.Interfaces;
using oh22.RhinoBackup.Import.Interfaces;

namespace oh22.RhinoBackup.Application
{
  public class BackupService : BackgroundService
  {
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<BackupService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly IAppSettings _settings;

    /// <summary>
    /// The service, from which the import/ export will be started.
    /// </summary>
    /// <param name="applicationLifetime"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="logger"></param>
    /// <param name="settings"></param>
    public BackupService(
      IHostApplicationLifetime applicationLifetime,
      IServiceProvider serviceProvider,
      ILogger<BackupService> logger,
      IAppSettings settings
    )
    {
      _applicationLifetime = applicationLifetime;
      _logger = logger;
      _serviceProvider = serviceProvider;
      _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      await Task.Yield();

      _settings.HandleValidation();

      try
      {
        var adapter = OutputAdapterHelper.GetOutputAdapter(_settings);

        if (_settings.Export)
        {
          _logger.LogInformation("Starting {Mode} with {Adapter}", "export", adapter);
          await StartExportAsync(adapter);
        }
        else if (_settings.Import)
        {
          _logger.LogInformation("Starting {Mode} with {Adapter}", "import", adapter);
          await StartImportAsync(adapter);
        }
        Environment.ExitCode = (int)ExitCodes.Success;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error while Executing.");
        Environment.ExitCode = (int)ExitCodes.Exception;
      }
      _applicationLifetime.StopApplication();
    }

    private async Task StartImportAsync(IOutputReader reader)
    {
      var import = _serviceProvider.GetService<IImport>();
      var data = await reader.ReadAsync();

      if (import == null)
      {
        _logger.LogWarning("Could not Initiate Importer. Check Configuration.");
        return;
      }

      await import.ExecuteAsync(data);
    }

    private async Task StartExportAsync(IOutputWriter writer)
    {
      var export = _serviceProvider.GetService<IExport>();

      if (export == null)
      {
        _logger.LogWarning("Could not Initiate Exporter. Check Configuration.");
        return;
      }

      var data = await export.ExecuteAsync();

      await writer.WriteAsync(data);
    }
  }
}