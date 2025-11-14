namespace CarsDealersManagement.Domain.Models
{
    public class ContactPersonDto : BaseDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Email { get; set; }

        public string PhoneNumber { get; set; }

        public int ShowroomId { get; set; }

        public virtual Showroom Showroom { get; set; }
    }
}
