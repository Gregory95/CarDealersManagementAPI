namespace CarsDealersManagement.Domain.Models
{
    public class DealersDto : BaseDto
    {        
        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;

        public virtual ICollection<Showroom> Showrooms { get; set; } = new List<Showroom>();
    }
}
