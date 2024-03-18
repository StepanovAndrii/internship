using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Practice.API
{
    internal static class JsonDoubleConverter
    {
        /// <summary>
        /// Змінює json double в double під C#.
        /// </summary>
        /// <remarks>
        /// Double в json форматі як розділяючий знак використовує символ ",". В C# це не мало б змісту, бо замість цього використовується символ ".".
        /// Для цього і використовується цей метод, щоб замінити символ "," на символ ".".
        /// </remarks>
        /// <param name="doubleNumber">Рядок який містить в собі тип double витягнутий з json об'єкта.</param>
        /// <returns>Перероблений double під C#.</returns>
        public static double Convert(string doubleNumber)
        {
            try
            {
                return double.Parse(doubleNumber.Trim().Replace('.', ','));
            }
            catch (FormatException)
            {
                DialogManager.DisplayError("Неправильний формат отриманих даних.");
                return double.NaN;
            }
            catch (Exception exception)
            {
                DialogManager.DisplayError($"Невідома помилка: {exception.Message}");
                return double.NaN;
            }
        }
    }
}
