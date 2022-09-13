using System.Text.RegularExpressions;
using oh22.RhinoBackup.Core.Models;

namespace oh22.RhinoBackup.Core.Helpers
{
  public static class SqlStringHelper
  {
    private const RegexOptions _options = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline;

    /// <summary>
    /// Returns a List of OpenRowSet objects, fetched from input string.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="name"></param>
    public static IEnumerable<OpenRowSet> GetOpenRowSets(string input, string name)
    {
      var results = new List<OpenRowSet>();

      const string pattern = @"OPENROWSET\((?'Content'[^\)]*)\)";
      const string groupName = @"Content";

      foreach (string content in GetFirstMatches(input, pattern, groupName))
      {
        string bulk = string.Empty;
        string? dataSource = null;

        foreach (string arg in content.Split(","))
        {
          string? firstBulk = GetFirstBulk(arg);

          if (!string.IsNullOrWhiteSpace(firstBulk))
          {
            bulk = firstBulk;
          }

          string? firstDataSource = GetFirstDataSource(arg);

          if (!string.IsNullOrWhiteSpace(firstDataSource))
          {
            dataSource = firstDataSource;
          }
        }

        var openRowSet = new OpenRowSet(
          name,
          bulk,
          dataSource
        );

        results.Add(openRowSet);
      }

      return results;
    }

    /// <summary>
    /// Returns first Bulk-Path from OpenRowSet-Content.
    /// </summary>
    /// <param name="openRowSetContent"></param>
    public static string? GetFirstBulk(string openRowSetContent)
    {
      const string pattern = @"BULK\s*'(?'Bulk'[^']*)'";
      const string groupName = "Bulk";

      return GetFirstMatch(openRowSetContent, pattern, groupName);
    }

    /// <summary>
    /// Returns first DataSource-Name from OpenRowSet-Content.
    /// </summary>
    /// <param name="openRowSetContent"></param>
    public static string? GetFirstDataSource(string openRowSetContent)
    {
      const string pattern = @"DATA_SOURCE\s*=\s*'(?'DataSource'[^']*)'";
      const string groupName = "DataSource";

      return GetFirstMatch(openRowSetContent, groupName, pattern);
    }

    /// <summary>
    /// Returns a List of DataSource objects, fetched from input string.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="name"></param>
    public static IEnumerable<DataSource> GetDataSources(string input, string name)
    {
      var results = new List<DataSource>();

      const string pattern = @"DATA SOURCE[^(]*\((?'Content'[^)]*)\)";
      const string groupName = "Content";

      foreach (string content in GetFirstMatches(input, pattern, groupName))
      {
        string location = GetFirstLocation(content);

        var dataSource = new DataSource(name, location);

        results.Add(dataSource);
      }

      return results;
    }

    /// <summary>
    /// Returns first Location from DataSource-Content.
    /// </summary>
    /// <param name="dataSourceContent"></param>
    public static string GetFirstLocation(string dataSourceContent)
    {
      const string groupName = "Location";
      const string pattern = @"LOCATION\s*=[^']*'(?'Location'[^']*)'";

      return GetFirstMatch(dataSourceContent, pattern, groupName);
    }

    private static string GetFirstMatch(string input, string pattern, string groupName)
    {
      string[] args = input.Split(",");

      string result = string.Empty;

      foreach (string arg in args)
      {
        var matches = Regex.Matches(arg, pattern, _options);

        if (matches.Count > 0)
        {
          result = matches[0]
            .Groups.Values.First(g => g.Name == groupName)
            .Value;
          break;
        }
      }

      return result;
    }

    private static IEnumerable<string> GetFirstMatches(string input, string pattern, string groupName)
    {
      var result = new List<string>();

      var matchesOpenRowSet = Regex.Matches(input, pattern, _options);

      foreach (var matchOpenRowSet in matchesOpenRowSet.Where(f => f != null))
      {
        string content = matchOpenRowSet.Groups.Values.AsEnumerable()
          .First(g => g.Name == groupName)
          .Value;

        result.Add(content);
      }

      return result;
    }
  }
}
