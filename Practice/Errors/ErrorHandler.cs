using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Practice
{
    internal static class ErrorHandler
    {
        /// <summary>
        /// Виводить повідомлення про помилку.
        /// </summary>
        /// <param name="message">Повідомлення яке потрібно вивести.</param>
        public static void DisplayError(string message)
        {
            DisplayErrorInternal(message);
        }

        /// <summary>
        /// Виводить повідомлення і зупиняє роботу додатку.
        /// </summary>
        /// <param name="message">Повідомлення яке потрібно вивести.</param>
        public static void ExitWithError(string message)
        {
            DisplayErrorInternal(message);
            Application.Current.Shutdown();
        }

        private static void DisplayErrorInternal(string message, // Повідомлення яке виведеться з помилкою
                                        [CallerMemberName] string callerMemberName = "", // Назва методу з якого визвалась помилка
                                        [CallerFilePath] string callerFilePath = "", // Шлях до файлу з якого визвалась помилка
                                        [CallerLineNumber] int callerLineNumber = 0) // Номер рядку з якого визвалась помилка
        {
            string fileName = Path.GetFileName(callerFilePath); // Отримується файл з його розширенням з якого визвали помилку
            string errorMessage = $"Файл: {fileName} \n" +
                                  $"Метод: {callerMemberName} \n" +
                                  $"Рядок: {callerLineNumber} \n\n" +
                                  $"{message}";

            MessageBox.Show(errorMessage, 
                            "Повідомлення про помилку",
                            MessageBoxButton.OK, // Кнопка "OK" для закриття помилки
                            MessageBoxImage.Error); // Вид повідомлення - помилка
        }
    }
}
