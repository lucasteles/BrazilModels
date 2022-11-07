namespace BrazilModels.Tests.Utils;

public static class Util
{
    public static IEnumerable<string> RepeatingDigits(string format) =>
        Enumerable.Range(0, 10)
            .Select(n => format.Replace("#", n.ToString()));
}
