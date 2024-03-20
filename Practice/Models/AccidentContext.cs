using System;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;

namespace Practice.Models
{
    /// <summary>
    /// Клас, який представляє контекст даних для доступу до таблиці з інформацією про нещасні випадки.
    /// </summary>
    internal class AccidentContext : DbContext
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу AccidentContext з використанням заданого рядка підключення.
        /// </summary>
        public AccidentContext() : base("DbConnection")
        {
            try
            {
                InitializeAsync();
            }
            catch (TimeoutException)
            {
                // Повідомляємо про помилку часу очікування з'єднання
                DialogManager.Notify("Час очікування з'єднання з сервером вийшов.", MessageBoxImage.Error, MessageBoxButton.OK);
            }
            catch (Exception)
            {
                // Повідомляємо про невідому помилку
                DialogManager.Notify("Під час виконання сталася невідома помилка.", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Ініціалізує контекст даних асинхронно.
        /// </summary>
        private async void InitializeAsync()
        {
            try
            {
                await OpenConnectionAsync();
            }
            catch (Exception exception)
            {
                // Повідомляємо про невідому помилку в ході ініціалізації
                DialogManager.Notify($"Під час виконання сталася невідома помилка.\n{exception}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Відкриває з'єднання з базою даних асинхронно.
        /// </summary>
        private async Task OpenConnectionAsync()
        {
            if (Database.Connection.State != ConnectionState.Open)
            {
                await Database.Connection.OpenAsync();
                if (Database.Connection.State != ConnectionState.Open)
                {
                    // Повідомляємо про невдале з'єднання з базою даних
                    DialogManager.Notify("Не вдалось з'єднатись з базою даних.", MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Властивість DbSet для доступу до таблиці з інформацією про нещасні випадки.
        /// </summary>
        public DbSet<Accident> Accidents { get; set; }
    }
}
