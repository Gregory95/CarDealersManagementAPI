using CarsDealersManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsDealersManagement.Infrastructure.Configurations
{
    public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.HasOne(u => u.Role).WithMany(u => u.UserRoles).HasForeignKey(u => u.RoleId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(u => u.User).WithMany(u => u.UserRoles).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
