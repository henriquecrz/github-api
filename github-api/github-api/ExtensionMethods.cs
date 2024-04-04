using System.Text.RegularExpressions;

namespace github_api;

public static class ExtensionMethods
{
    public static string Replace(this string input, Regex regex, string newValue)
    {
        return regex.Replace(input, newValue);
    }
}
