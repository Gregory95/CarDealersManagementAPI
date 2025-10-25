using System.ComponentModel.DataAnnotations;

namespace CarsDealersManagement.Domain.Entities
{
    public class ContactPerson : BaseEntity
    {
        public ContactPerson() { }

        public ContactPerson(string firstName, string lastName, string email, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        [MaxLength(100)]
        public string FirstName { get; set; } = default!;

        [MaxLength(100)]
        public string LastName { get; set; } = default!;

        [MaxLength(100)]
        public string Email { get; set; } = default!;

        [MaxLength(25)]
        public string PhoneNumber { get; set; } = default!;

        public static ContactPerson Create(string firstName, string lastName, string email, string phoneNumber)
        {
            return new ContactPerson(firstName, lastName, email, phoneNumber);
        }

        public void Update(string firstName, string lastName, string email, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
