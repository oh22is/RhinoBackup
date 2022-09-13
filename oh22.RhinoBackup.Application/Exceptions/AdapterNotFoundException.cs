using System.Runtime.Serialization;

namespace oh22.RhinoBackup.Application.Exceptions
{
  [Serializable]
  internal class AdapterNotFoundException : Exception
  {
    public AdapterNotFoundException()
    {
    }

    public AdapterNotFoundException(string? message) : base(message)
    {
    }

    public AdapterNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected AdapterNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}