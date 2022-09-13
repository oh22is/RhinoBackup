namespace oh22.RhinoBackup.Application.Interfaces
{
  public interface IAppSettings
  {
    public bool Export { get; set; }
    public bool Import { get; set; }
    public string FilePath { get; set; }
    public string DirectoryPath { get; set; }
    public void HandleValidation();
  }
}