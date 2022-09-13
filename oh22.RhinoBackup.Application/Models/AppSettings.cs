using CommandLine;
using oh22.RhinoBackup.Application.Exceptions;
using oh22.RhinoBackup.Application.Interfaces;

namespace oh22.RhinoBackup.Application.Models
{
  public class AppSettings : IAppSettings
  {
    [Option("export", Default = false, HelpText = "Exports Database")]
    public bool Export { get; set; }

    [Option("import", Default = false, HelpText = "Imports Rhino Backup File")]
    public bool Import { get; set; }

    [Option("AsFile", HelpText = "File Path for saving and loading data.")]
    public string FilePath { get; set; } = "";

    [Option("AsDirectory", HelpText = "Directory Path for saving and loading data.")]
    public string DirectoryPath { get; set; } = "export";

    /// <summary>
    /// Handles the validation and throws an exception if invalid.
    /// </summary>
    /// <exception cref="AppSettingsValidationException"></exception>
    public void HandleValidation()
    {
      var exceptionMessages = new List<string>();

      if (!(Export ^ Import))
      {
        exceptionMessages.Add("Either import or export must be selected.");
      }

      bool hasFilePath = !string.IsNullOrWhiteSpace(FilePath);
      bool hasDirectoryPath = !string.IsNullOrWhiteSpace(DirectoryPath);

      if ((hasFilePath && DirectoryPath != "export") || (!hasFilePath && !hasDirectoryPath))
      {
        exceptionMessages.Add("A single output adapter must be set.");
      }

      if (exceptionMessages.Any())
      {
        throw new AppSettingsValidationException(exceptionMessages);
      }
    }
  }
}