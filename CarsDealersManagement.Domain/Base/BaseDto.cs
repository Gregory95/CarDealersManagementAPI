namespace CarsDealersManagement.Domain.Base
{
    public class BaseDto
    {
        public int? Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string? ModifiedBy { get; set; } = null;
        public string? CreatedBy { get; set; } = null;
    }
}
