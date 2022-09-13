using System.Text.RegularExpressions;

namespace oh22.RhinoBackup.Core.Helpers
{
  public static class PetaPocoHelper
  {
    /// <summary>
    /// Escapes the input query for petapoco.
    /// </summary>
    /// <param name="query"></param>
    public static string Escape(string query)
    {
      const string pattern = "(@+)";
      const string replacement = "@$1";

      return Regex.Replace(query, pattern, replacement);
    }
  }
}
