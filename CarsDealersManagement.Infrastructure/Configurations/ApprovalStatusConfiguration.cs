using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CarsDealersManagement.Infrastructure.Configurations
{
    public class ApprovalStatusConfiguration : IEntityTypeConfiguration<ApprovalStatus>
    {
        public void Configure(EntityTypeBuilder<ApprovalStatus> builder)
        {
            builder.HasKey(x => x.ApprovalStatusId);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .HasMaxLength(200);

            // Seed data
            builder.HasData(
                new ApprovalStatus
                {
                    ApprovalStatusId = (byte)ApprovalStatusEnum.Pending,
                    Name = "Pending",
                    Description = "Awaiting approval"
                },
                new ApprovalStatus
                {
                    ApprovalStatusId = (byte)ApprovalStatusEnum.Approved,
                    Name = "Approved",
                    Description = "Has been approved"
                },
                new ApprovalStatus
                {
                    ApprovalStatusId = (byte)ApprovalStatusEnum.Rejected,
                    Name = "Rejected",
                    Description = "Has been rejected"
                }
            );
        }
    }
}
