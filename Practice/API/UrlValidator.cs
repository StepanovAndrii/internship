using System.Text.RegularExpressions;

namespace Practice
{
    internal static class UrlValidator
    {
        /// <summary>
        /// Перевіряє рядок на правильний формат посилання.
        /// </summary>
        /// <param name="url">Рядок з посиланням для перевірки.</param>
        /// <returns>Булеве значення на відповідь "чи дотримано стандартів форматування".</returns>
        public static bool ValidateURL(string url)
        {
            return Regex.IsMatch(url, @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");
        }
    }
}
