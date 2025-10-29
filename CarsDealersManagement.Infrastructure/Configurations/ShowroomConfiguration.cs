using CarsDealersManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarsDealersManagement.Infrastructure.Configurations
{
    public class ShowroomConfiguration : IEntityTypeConfiguration<Showroom>
    {
        public void Configure(EntityTypeBuilder<Showroom> builder)
        {
            builder.ToTable("Showrooms");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired();

            builder.Property(c => c.Location)
                .IsRequired();

            builder.HasIndex(c => new { c.Id, c.Name, c.Location })
                .IsUnique();

            builder.HasOne(c => c.Dealer)
                .WithMany(d => d.Showrooms)
                .HasForeignKey(c => c.DealerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
