using System;
using System.Windows;

namespace Practice.API
{
    /// <summary>
    /// Клас, який надає функціональність конвертації double значень з формату JSON у формат для C#.
    /// </summary>
    internal static class JsonDoubleConverter
    {
        /// <summary>
        /// Конвертує рядок, який представляє double з формату JSON у double для C#.
        /// </summary>
        /// <param name="doubleNumber">Рядок, що містить double у форматі JSON.</param>
        /// <returns>Double значення, конвертоване з JSON у формат для C#.</returns>
        public static double Convert(string doubleNumber)
        {
            try
            {
                // Конвертуємо рядок у double, враховуючи можливий використання крапки або коми як роздільника дробової частини
                return double.Parse(doubleNumber.Trim().Replace('.', ','));
            }
            catch (FormatException)
            {
                // Повідомляємо про помилку у випадку неправильного формату рядка
                DialogManager.Notify("Неправильний формат отриманих даних.", MessageBoxImage.Error, MessageBoxButton.OK);
                return double.NaN; // Повертаємо NaN (Not a Number) у разі помилки
            }
            catch (Exception)
            {
                // Повідомляємо про невідому помилку у випадку будь-якої іншої помилки
                DialogManager.Notify("Невідома помилка.", MessageBoxImage.Error, MessageBoxButton.OK);
                return double.NaN; // Повертаємо NaN у разі невідомої помилки
            }
        }
    }
}
