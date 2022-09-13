namespace oh22.RhinoBackup.Core.Models
{
  public class DataSource
  {
    public string Name { get; set; }
    public string Location { get; set; }

    public DataSource(string name, string location)
    {
      Name = name;
      Location = location;
    }
  }
}
