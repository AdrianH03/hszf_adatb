using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ABC123_HSZF_2024251.Persistence.MsSql
{
    public class TaxiDbContextFactory : IDesignTimeDbContextFactory<TaxiDbContext>
    {
        public TaxiDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TaxiDbContext>();
            optionsBuilder.UseSqlite("Data Source=TaxiDatabase.db");

            return new TaxiDbContext(optionsBuilder.Options);
        }
    }
}
