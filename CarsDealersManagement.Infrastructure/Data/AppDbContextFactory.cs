using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CarsDealersManagement.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer("Server=localhost\\SQLSERVER_LOCAL;Database=CarsDealersManagementDb;User Id=sa;Password=\"5w_vhAJ%3FNoM/n1;NxH\";TrustServerCertificate=True;");

            optionsBuilder.UseOpenIddict();

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
