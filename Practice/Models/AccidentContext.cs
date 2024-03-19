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
                DialogManager.Notify("Час очікування з'єднання з сервером вийшов.", MessageBoxImage.Error, MessageBoxButton.OK);
            }
            catch (Exception)
            {
                DialogManager.Notify("Під час виконання сталася невідома помилка.", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }
        private async void InitializeAsync()
        {
            try
            {
                await OpenConnectionAsync();
            }
            catch (Exception exception)
            {
                DialogManager.Notify($"Під час виконання сталася невідома помилка./n{exception}", MessageBoxImage.Error, MessageBoxButton.OK);
            }
        }
        private async Task OpenConnectionAsync()
        {
            if (Database.Connection.State != ConnectionState.Open)
            {
                await Database.Connection.OpenAsync();
                if (Database.Connection.State != ConnectionState.Open)
                {
                    DialogManager.Notify("Не вдалось з'єднатись з базою даних.", MessageBoxImage.Error, MessageBoxButton.OK);
                }
            }
        }
        public DbSet<Accident> Accidents { get; set; }
    }
}
