using CarsDealersManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsDealersManagement.Infrastructure.Configurations
{
    public class ApplicationUserTokenConfiguration : IEntityTypeConfiguration<ApplicationUserToken>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
        {
            // Composite primary key consisting of the UserId, LoginProvider and Name
            builder.HasOne(u => u.User).WithMany(u => u.Tokens).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Restrict);

            // Limit the size of the composite key columns due to common DB restrictions
            builder.Property(t => t.LoginProvider).HasMaxLength(2000);
            builder.Property(t => t.Name).HasMaxLength(2000);

            // Maps to the AspNetUserTokens table
            builder.ToTable("AspNetUserTokens");
        }
    }
}
