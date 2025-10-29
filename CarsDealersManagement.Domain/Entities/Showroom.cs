using System.ComponentModel.DataAnnotations;

namespace CarsDealersManagement.Domain.Entities
{
    public class Showroom : BaseEntity
    {
        public Showroom() { }

        public Showroom(string name, string location, int capacity)
        {
            Name = name;
            Location = location;
            Capacity = capacity;
        }

        [MaxLength(200)]
        public string Name { get; set; } = default!;

        [MaxLength(300)]
        public string Location { get; set; } = default!;

        public int Capacity { get; set; }

        public int DealerId { get; set; } = default!;

        public static Showroom Create(string name, string location, int capacity)
        {
            return new Showroom(name, location, capacity);
        }

        public void Update(string name, string location, int capacity)
        {
            Name = name;
            Location = location;
            Capacity = capacity;
        }

        public virtual Dealer Dealer { get; set; } = default!;

        public virtual ICollection<ContactPerson> ContactPersons { get; set; } = new List<ContactPerson>();
    }
}
