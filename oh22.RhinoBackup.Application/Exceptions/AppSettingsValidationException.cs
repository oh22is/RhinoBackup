namespace oh22.RhinoBackup.Application.Exceptions
{
  public class AppSettingsValidationException : Exception
  {
    /// <summary>
    /// An exception which should be thrown when some app settings are invalid.
    /// </summary>
    /// <param name="messages"></param>
    public AppSettingsValidationException(List<string> messages) : base(string.Join(Environment.NewLine, messages))
    {
    }
  }
}
