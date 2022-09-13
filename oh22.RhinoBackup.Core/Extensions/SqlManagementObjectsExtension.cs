using Microsoft.SqlServer.Management.Smo;

namespace oh22.RhinoBackup.Core.Extensions
{
  public static class SqlManagementObjectsExtension
  {
    /// <summary>
    /// Writes create script to path.
    /// </summary>
    /// <param name="scriptable"></param>
    /// <param name="path"></param>
    /// <returns>Returns true if a file was created. False if not.</returns>
    public static bool WriteCreate(this IScriptable scriptable, string path)
    {
      var scriptingOptions = new ScriptingOptions();

      return scriptable.WriteScript(path, scriptingOptions);
    }

    /// <summary>
    /// Get create script.
    /// </summary>
    /// <param name="scriptable"></param>
    /// <returns>Returns the Create Script.</returns>
    public static string GetCreate(this IScriptable scriptable)
    {
      var scriptingOptions = new ScriptingOptions();

      return scriptable.GetScript(scriptingOptions);
    }

    /// <summary>
    /// Writes drop script to path.
    /// </summary>
    /// <param name="scriptable"></param>
    /// <param name="path"></param>
    /// <returns>Returns true if a file was created. False if not.</returns>
    public static bool WriteDrop(this IScriptable scriptable, string path)
    {
      var scriptingOptions = new ScriptingOptions()
      {
        IncludeIfNotExists = true,
        ScriptDrops = true
      };

      return scriptable.WriteScript(path, scriptingOptions);
    }

    /// <summary>
    /// Gets drop script .
    /// </summary>
    /// <param name="scriptable"></param>
    /// <returns>Returns the Drop Script.</returns>
    public static string GetDrop(this IScriptable scriptable)
    {
      var scriptingOptions = new ScriptingOptions()
      {
        IncludeIfNotExists = true,
        ScriptDrops = true
      };

      return scriptable.GetScript(scriptingOptions);
    }

    /// <summary>
    /// Writes alter script to path.
    /// </summary>
    /// <param name="scriptable"></param>
    /// <param name="path"></param>
    /// <returns>Returns true if a file was created. False if not.</returns>
    public static bool WriteAlter(this IScriptable scriptable, string path)
    {
      var scriptingOptions = new ScriptingOptions()
      {
        ScriptForAlter = true
      };

      return scriptable.WriteScript(path, scriptingOptions);
    }

    /// <summary>
    /// Get alter script.
    /// </summary>
    /// <param name="scriptable"></param>
    /// <returns>Returns the AlterScript.</returns>
    public static string GetAlter(this IScriptable scriptable)
    {
      var scriptingOptions = new ScriptingOptions()
      {
        ScriptForAlter = true
      };

      return scriptable.GetScript(scriptingOptions);
    }

    public static string GetScript(this IScriptable scriptable, ScriptingOptions scriptingOptions)
    {
      scriptingOptions ??= new ScriptingOptions();

      var scripts = scriptable.Script(scriptingOptions);

      return scripts.Count == 0 ? string.Empty : scripts[^1] ?? string.Empty;
    }

    public static bool WriteScript(this IScriptable scriptable, string path, ScriptingOptions scriptingOptions)
    {
      string? script = scriptable.GetScript(scriptingOptions);
      if (string.IsNullOrWhiteSpace(script))
      {
        return false;
      }

      string? folderPath = Path.GetDirectoryName(path);
      if (folderPath != null)
      {
        _ = Directory.CreateDirectory(folderPath);
      }

      File.WriteAllText(path, script);

      return true;
    }
  }
}
