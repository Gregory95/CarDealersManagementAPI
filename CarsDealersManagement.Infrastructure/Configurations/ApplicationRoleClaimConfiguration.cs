using CarsDealersManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsDealersManagement.Infrastructure.Configurations
{
    public class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
        {
            // Primary key
            builder.HasOne(u => u.Role).WithMany(u => u.RoleClaims).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Restrict);
            // Maps to the AspNetRoleClaims table
            builder.ToTable("AspNetRoleClaims");
        }
    }
}
