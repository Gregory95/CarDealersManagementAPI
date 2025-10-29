using CarsDealersManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CarsDealersManagement.Infrastructure.Configurations
{
    public class ContactPersonConfiguration : IEntityTypeConfiguration<ContactPerson>
    {
        public void Configure(EntityTypeBuilder<ContactPerson> builder)
        {
            builder.ToTable("ContactPersons");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName)
                .IsRequired();

            builder.Property(c => c.LastName)
                .IsRequired();

            builder.Property(c => c.PhoneNumber)
                .IsRequired();

            builder.HasIndex(c => new { c.Id, c.FirstName, c.LastName })
                .IsUnique();

            builder.HasIndex(c => new { c.Id, c.PhoneNumber })
                .IsUnique();

            builder.HasOne(c => c.Showroom)
                .WithMany(d => d.ContactPersons)
                .HasForeignKey(c => c.ShowroomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}