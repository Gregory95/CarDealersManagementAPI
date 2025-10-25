using CarsDealersManagement.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace CarsDealersManagement.Domain.Entities
{
    public class Dealer : BaseEntity
    {
        public Dealer() { }

        public Dealer(string firstName, string lastName, string email, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        [MaxLength(50)]
        public string FirstName { get; set; } = default!;

        [MaxLength(50)]
        public string LastName { get; set; } = default!;

        [MaxLength(100)]
        public string Email { get; set; } = default!;

        [MaxLength(25)]
        public string PhoneNumber { get; set; } = default!;

        public static Dealer Create(string firstName, string lastName, string email, string phoneNumber)
        {
            return new Dealer(firstName, lastName, email, phoneNumber);
        }

        public void Update(string firstName, string lastName, string email, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
