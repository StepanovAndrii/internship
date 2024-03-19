using System;
using System.Windows;

namespace Practice.API
{
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
                return double.Parse(doubleNumber.Trim().Replace('.', ','));
            }
            catch (FormatException)
            {
                DialogManager.Notify("Неправильний формат отриманих даних.", MessageBoxImage.Error, MessageBoxButton.OK);
                return double.NaN;
            }
            catch (Exception)
            {
                DialogManager.Notify("Невідома помилка.", MessageBoxImage.Error, MessageBoxButton.OK);
                return double.NaN;
            }
        }
    }
}
