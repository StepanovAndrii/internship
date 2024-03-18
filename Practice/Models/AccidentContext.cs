using System;
using System.Data;
using System.Data.Entity;

namespace Practice.Models
{
    internal class AccidentContext : DbContext
    {
        public AccidentContext() : base(nameOrConnectionString: "DbConnection") {
            try
            {
                Database.Connection.Open(); 
                var connection = Database.Connection.State;
                if (connection != ConnectionState.Open)
                {
                    Practice.DialogManager.ExitWithError("Помилка підключення до бази даних. Стан підключення: " + connection);
                }
            }
            catch (Exception exception)
            {
                Practice.DialogManager.ExitWithError("Непередбачена помилка: " + exception.Message);
            }
        }

        public DbSet<Accident> Accidents { get; set; }
    }
}