namespace CarsDealersManagement.Domain.Models
{
    public class TokenDto
    {
        public string AccessToken { get; init; } = default!;
        public long ExpiresIn { get; init; } = default!;
    }
}
