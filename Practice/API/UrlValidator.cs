using System.Text.RegularExpressions;

namespace Practice
{
    /// <summary>
    /// Клас, який надає функціональність перевірки правильності формату URL.
    /// </summary>
    internal static class UrlValidator
    {
        /// <summary>
        /// Перевіряє рядок на правильний формат посилання.
        /// </summary>
        /// <param name="url">Рядок з посиланням для перевірки.</param>
        /// <returns>Чи відповідає рядок формату посилання.</returns>
        public static bool ValidateURL(string url)
        {
            // Використовує регулярний вираз для перевірки формату посилання
            return Regex.IsMatch(url, @"^http(s)?:\/\/[\w\-]+(\.[\w\-]+)*(\/[\w\-]*)*$");
        }
    }
}
