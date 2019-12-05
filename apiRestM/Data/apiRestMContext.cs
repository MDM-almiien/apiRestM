using Microsoft.EntityFrameworkCore;

namespace apiRestM.Data
{
    public class ApiRestMContext : DbContext
    {
        public ApiRestMContext(DbContextOptions<ApiRestMContext> options) : base(options)
        {

        }

        public DbSet<apiRestM.Models.Loans> Loan { get; set; }
    }
}
