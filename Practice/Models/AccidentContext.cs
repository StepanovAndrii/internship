using System;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Practice.Models
{
    internal class AccidentContext : DbContext
    {
        public AccidentContext() : base("DbConnection")
        {
            try
            {
                InitializeAsync();
            }
            catch (TimeoutException)
            {
                DialogManager.ExitWithError("Час очікування з'єднання з сервером вийшов.");
            }
            catch (Exception)
            {
                DialogManager.ExitWithError("Під час виконання сталася невідома помилка.");
            }
        }

        private async void InitializeAsync()
        {
            try
            {
                await OpenConnectionAsync();
            }
            catch (Exception)
            {
                DialogManager.ExitWithError("Під час виконання сталася невідома помилка.");
            }
        }

        private async Task OpenConnectionAsync()
        {
            if (Database.Connection.State != ConnectionState.Open)
            {
                await Database.Connection.OpenAsync(); 
                if (Database.Connection.State != ConnectionState.Open)
                {
                    DialogManager.ExitWithError("Не вдалось з'єднатись з базою даних.");
                }
            }
        }

        public DbSet<Accident> Accidents { get; set; }
    }
}