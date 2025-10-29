using CarsDealersManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CarsDealersManagement.Infrastructure.Configurations
{
    public class DealerConfiguration : IEntityTypeConfiguration<Dealer>
    {
        public void Configure(EntityTypeBuilder<Dealer> builder)
        {
            builder.ToTable("Dealers");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName)
                .IsRequired();

            builder.Property(c => c.LastName)
                .IsRequired();

            builder.Property(c => c.Email)
                .IsRequired();

            builder.HasIndex(c => new { c.Id, c.FirstName, c.LastName })
                .IsUnique();
        }
    }
}