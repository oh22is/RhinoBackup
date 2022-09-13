using Newtonsoft.Json;

namespace oh22.RhinoBackup.Core.Models
{
  /// <summary>
  /// A node for structuring dependencies.
  /// </summary>
  /// <typeparam name="T">The type of the value, which the node contains.</typeparam>
  public class Node<T>
  {
    public IList<Node<T>> References { get; set; } = new List<Node<T>>();
    public T? Value { get; set; }

    /// <summary>
    /// A node for structuring dependencies.
    /// </summary>
    [JsonConstructor]
    public Node()
    {
    }

    /// <summary>
    /// A node for structuring dependencies.
    /// </summary>
    /// <param name="value">The value which the node contains.</param>
    public Node(T value) : this()
    {
      Value = value;
    }
  }
}
