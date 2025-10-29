using CarsDealersManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsDealersManagement.Infrastructure.Configurations
{
    public class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogin>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
        {
            // Composite primary key consisting of the LoginProvider and the key to use
            // with that provider
            builder.HasOne(r => r.User).WithMany(e => e.Logins).OnDelete(DeleteBehavior.Restrict);

            // Limit the size of the composite key columns due to common DB restrictions
            builder.Property(l => l.LoginProvider).HasMaxLength(128);
            builder.Property(l => l.ProviderKey).HasMaxLength(128);

            // Maps to the AspNetUserLogins table
            builder.ToTable("AspNetUserLogins");
        }
    }
}
