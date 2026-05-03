using System.ComponentModel.DataAnnotations;

namespace CarsDealersManagement.Domain.Entities
{
    public sealed class ApprovalStatus
    {
        [Range(0, 2)]
        public byte ApprovalStatusId { get; init; } = default!;

        [MaxLength(50)]
        public string Name { get; set; } = default!;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
