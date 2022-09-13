using oh22.RhinoBackup.Core.Helpers;

namespace oh22.RhinoBackup.Application.Test
{
  public class PetaPocoHelperTest
  {

    [Fact]
    public async Task TestPetaPocoStringEscape()
    {
      string input = "@Test";

      string escaped = PetaPocoHelper.Escape(input);

      string expected = "@@Test";

      Assert.Equal(expected, escaped);
    }
  }
}
