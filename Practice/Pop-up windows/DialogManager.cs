using System.Windows;
using System.Runtime.CompilerServices;

namespace Practice
{
    /// <summary>
    /// Клас, що надає методи для відображення повідомлень.
    /// </summary>
    internal static class DialogManager
    {
        /// <summary>
        /// Відображає повідомлення з використанням стандартного вікна.
        /// </summary>
        /// <param name="message">Текст повідомлення.</param>
        /// <param name="messageType">Тип повідомлення.</param>
        /// <param name="messageButtons">Кнопки в повідомленні.</param>
        /// <param name="callerFilePath">Шлях до файлу, який викликав метод (за замовчуванням).</param>
        /// <param name="callerLineNumber">Номер рядка, який викликав метод (за замовчуванням).</param>
        public static bool Notify(
            string message,
            MessageBoxImage messageType,
            MessageBoxButton messageButtons,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0
        )
        {
            MessageBoxResult answer = MessageBox.Show(
                $"{callerFilePath} ({callerLineNumber})\n\n{message}",
                messageType.ToString(),
                messageButtons,
                messageType
            );

            return answer == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Відображає повідомлення про помилку та завершує виконання програми.
        /// </summary>
        /// <param name="message">Текст повідомлення про помилку.</param>
        /// <param name="messageType">Тип повідомлення про помилку.</param>
        /// <param name="messageButtons">Кнопки в повідомленні.</param>
        /// <param name="callerFilePath">Шлях до файлу, який викликав метод (за замовчуванням).</param>
        /// <param name="callerLineNumber">Номер рядка, який викликав метод (за замовчуванням).</param>
        public static void NotifyExit(
            string message,
            MessageBoxImage messageType,
            MessageBoxButton messageButtons,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0
        )
        {
            Notify(message, messageType, messageButtons, callerFilePath, callerLineNumber);
            Application.Current.Shutdown();
        }
    }
}

