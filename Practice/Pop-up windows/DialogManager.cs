using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Practice
{
    internal static class DialogManager
    {
        /// <summary>
        /// Виводить повідомлення і зупиняє роботу додатку.
        /// </summary>
        /// <param name="message">Повідомлення яке потрібно вивести.</param>
        /// <param name="callerMemberName">Необов'язковий параметр. Передає назву метода який визвав помилку.</param>
        /// <param name="callerFilePath">Необов'язковий параметр. Передає шлях до файлу з розширенням який визвав помилку.</param>
        /// <param name="callerLineNumber">Необов'язковий параметр. Передає рядок на якому визвалась помилка.</param>
        public static void ExitWithError(string message, 
                                        [CallerMemberName] string callerMemberName = "",
                                        [CallerFilePath] string callerFilePath = "",
                                        [CallerLineNumber] int callerLineNumber = 0)
        {
            DisplayError(message, callerMemberName, callerFilePath, callerLineNumber);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Виводить повідомлення про помилку.
        /// </summary>
        /// <param name="message">Повідомлення яке потрібно вивести.</param>
        /// <param name="callerMemberName">Необов'язковий параметр. Передає назву метода який визвав помилку.</param>
        /// <param name="callerFilePath">Необов'язковий параметр. Передає шлях до файлу з розширенням який визвав помилку.</param>
        /// <param name="callerLineNumber">Необов'язковий параметр. Передає рядок на якому визвалась помилка.</param>
        public static void DisplayError(string message, // Повідомлення яке виведеться з помилкою
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

        /// <summary>
        /// Виводить питальне діалогове вікно для підтвердження.
        /// </summary>
        /// <param name="message">Питальне повідомлення для виводу</param>
        /// <param name="callerFilePath">Передає шлях до файлу з розширенням який визвав помилку.</param>
        /// <returns>Булеве значення (згоду чи незгоду користувача).</returns>
        public static bool DisplayQuestion(string message, [CallerFilePath] string callerFilePath = "")
        {
            string fileName = Path.GetFileName(callerFilePath); // Отримується файл з його розширенням з якого визвали помилку

            MessageBoxResult agreed = MessageBox.Show(message,
                                      fileName,
                                      MessageBoxButton.YesNo, // Кнопка "ОК" для підтверження і "Cancel" для відмови
                                      MessageBoxImage.Question); // Вид повідомлення - питання (підтвердження)
            
            return agreed == MessageBoxResult.Yes; // Повертає булеве значення "Чи людина відповіла "Так"?"
        }

        /// <summary>
        /// Виводить інформацію в окремому діалоговому вікні.
        /// </summary>
        /// <param name="message">Повідомлення яке треба вивести.</param>
        public static void DisplayInfo(string message)
        {
            MessageBox.Show(message,
                            "", // Пустий заголовок
                            MessageBoxButton.OK, // Кнопка "ОК" для закриття вікна
                            MessageBoxImage.Information); // Вид повідомлення - інформація
        }
    }
}
