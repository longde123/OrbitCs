using System.Text;

namespace Orbit.Util.Misc;

public static class RngUtils
{
    private static readonly char[] AllowedChars =
    {
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    public static string RandomString(int numChars = 16, Random random = null)
    {
        if (numChars <= 0)
        {
            throw new ArgumentException("numCharacters must be > 0");
        }

        if (random == null)
        {
            random = new Random();
        }

        var targetString = new StringBuilder(numChars);

        var allowedCharsCount = AllowedChars.Length;
        for (var i = 0; i < numChars; i++)
        {
            targetString.Append(AllowedChars[random.Next(allowedCharsCount)]);
        }

        return targetString.ToString();
    }
}