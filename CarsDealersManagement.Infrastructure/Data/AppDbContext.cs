using CarsDealersManagement.Domain.Base;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarsDealersManagement.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<ContactPerson> ContactPersons { get; set; }
        public DbSet<Showroom> Showrooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationRoleConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserLoginConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserTokenConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationRoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new DealerConfiguration());
            modelBuilder.ApplyConfiguration(new ShowroomConfiguration());
            modelBuilder.ApplyConfiguration(new ContactPersonConfiguration());
        }

        public async Task<int> SaveChangesLightAsync(CancellationToken ct = default)
        {
            return await base.SaveChangesAsync(ct);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            AuditInfo();
            return await base.SaveChangesAsync(ct);
        }

        private void AuditInfo()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>().AsEnumerable())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}