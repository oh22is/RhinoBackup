namespace oh22.RhinoBackup.Core.Extensions
{
  public static class StringCollectionExtensions
  {
    /// <summary>
    /// Writes the enumerable to the filepath, separated by new lines.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="filePath"></param>
    public static async Task WriteFileAsync(this IEnumerable<string> list, string filePath)
    {
      await File.WriteAllTextAsync(filePath, string.Join(Environment.NewLine, list));
    }
  }
}
