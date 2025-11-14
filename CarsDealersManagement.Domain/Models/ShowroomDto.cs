namespace CarsDealersManagement.Domain.Models
{
    public class ShowroomDto : BaseDto
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public int Capacity { get; set; }

        public int DealerId { get; set; }
    }
}
