using System.ComponentModel.DataAnnotations;

namespace CarsDealersManagement.Domain.Models
{
    public class LoginRequestDto
    {
        public required string UserName { get; init; } = default!;
        public required string Password { get; init; } = default!;
    }
}
