using System.Data.Entity;

namespace Practice.Models
{
    internal class AccidentContext : DbContext
    {
        public AccidentContext() : base("DbConnection")
        {
        
        }
        public DbSet<Accident> Accidents { get; set; }
    }
}
