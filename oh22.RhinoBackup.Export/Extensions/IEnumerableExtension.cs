using oh22.RhinoBackup.Core.Models;
using oh22.RhinoBackup.Core.Interfaces;
using oh22.RhinoBackup.Core.POCOs;

namespace oh22.RhinoBackup.Export.Extensions
{
  public static class IEnumerableExtension
  {
    /// <summary>
    /// Creates a dependency tree from sql export entries and dependencies.
    /// </summary>
    /// <param name="entries"></param>
    /// <param name="dependencies"></param>
    public static IEnumerable<Node<SqlEntry>> CreateTree(this IEnumerable<SqlEntry> entries, IEnumerable<IDependency> dependencies)
    {
      var tree = entries.Select(e => new Node<SqlEntry>(e)).ToList();

      dependencies = dependencies.Distinct();

      foreach (var dependency in dependencies)
      {
        var view = tree
          .Find(
            node => node.Value?.Equals(dependency) == true
          );

        if (view == default)
        {
          continue;
        }

        var reference = tree
          .Find(
            node => node.Value?.Schema.Equals(dependency.ReferenceSchemaName, StringComparison.OrdinalIgnoreCase) == true
              && node.Value?.Name.Equals(dependency.ReferenceEntityName, StringComparison.OrdinalIgnoreCase) == true
          );

        if (view != default && reference != default)
        {
          view.References.Add(reference);
        }
      }

      return tree;
    }

    /// <summary>
    /// Gets the dependencies for sql export entries.
    /// </summary>
    /// <param name="entries"></param>
    public static IEnumerable<Dependency> GetDependencies(this IEnumerable<SqlEntry> entries)
    {
      var dependencies = new List<Dependency>();

      foreach (var entry in entries)
      {
        if (entry.Resource == null)
        {
          continue;
        }

        foreach (string? dataSource in entry.Resource.OpenRowSets.Select(set => set.DataSource))
        {
          if (string.IsNullOrWhiteSpace(dataSource))
          {
            continue;
          }

          var dependency = new Dependency()
          {
            EntityType = entry.Type,
            SchemaName = entry.Schema,
            ViewName = entry.Name,
            ReferenceSchemaName = "",
            ReferenceEntityName = dataSource
          };

          dependencies.Add(dependency);
        }
      }

      return dependencies.Distinct();
    }
  }
}