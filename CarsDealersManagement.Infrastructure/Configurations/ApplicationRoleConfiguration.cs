using CarsDealersManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CarsDealersManagement.Infrastructure.Configurations
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            // Indexes for "normalized" name to allow efficient lookups
           builder.HasIndex(u => u.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

            // Maps to the AspNetRoles table
           builder.ToTable("AspNetRoles");

            // A concurrency token for use with the optimistic concurrency checking
           builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            // Limit the size of columns to use efficient database types
           builder.Property(u => u.Description).HasMaxLength(1000);

            // The relationships between User and other entity types
            // Note that these relationships are configured with no navigation properties

           builder.HasMany(e => e.UserRoles)
             .WithOne(e => e.Role)
             .HasForeignKey(e => e.RoleId)
             .IsRequired();

           builder.HasMany(e => e.RoleClaims)
             .WithOne(e => e.Role)
             .HasForeignKey(e => e.RoleId)
             .IsRequired();

            // Each User can have many UserTokens
           builder.HasMany<ApplicationRoleClaim>().WithOne().HasForeignKey(ut => ut.RoleId).IsRequired();

            // Each User can have many entries in the UserRole join table
           builder.HasMany<ApplicationUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
        }
    }
}
