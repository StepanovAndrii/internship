using System.IO;
using System.Reflection;
using System.Windows;

namespace Practice
{
    internal static class ErrorHandler
    {
        /// <summary>
        /// Виводить повідомлення про помилку та зупиняє роботу додатка.
        /// </summary>
        /// <param name="errorMessage">Повідомлення, яке розписує проблему.</param>
        public static void DisplayAndShutdownOnError(string errorMessage)
        {
            DisplayError(errorMessage);
            ShutdownApp();
        }

        /// <summary>
        /// Виводить повідомлення про помилку.
        /// </summary>
        /// <param name="message">Повідомлення, яке розписує проблему.</param>
        public static void DisplayError(string errorMessage)
        {
            string applicationName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
            string applicationExtension = Path.GetExtension(Assembly.GetEntryAssembly().Location);

            MessageBox.Show($"{errorMessage}. Для виходу з додатку натисніть кнопку \"OK\".", // Повідомлення про помилку
                            $"{applicationName}{applicationExtension} - Application Error", // Заголовок повідомлення
                            MessageBoxButton.OK, // Кнопка "ОК" для закриття повідомлення
                            MessageBoxImage.Stop); // Червоний знак зупинки, який вказує на критичну помилку або проблему, що призводить до зупинки
        }

        /// <summary>
        /// Закінчує роботу та закриває додаток.
        /// </summary>
        public static void ShutdownApp() 
        {
            Application.Current.Shutdown();
        }
    }
}