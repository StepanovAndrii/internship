using System.Text.RegularExpressions;

namespace Practice
{
    internal static class UrlValidator
    {
        public static bool ValidateURL(string url)
        {
            return Regex.IsMatch(url, @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
        }
    }
}
