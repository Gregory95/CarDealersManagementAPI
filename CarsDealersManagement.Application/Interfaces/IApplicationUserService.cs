using CarsDealersManagement.Domain.Base;
using CarsDealersManagement.Domain.Models;

namespace CarsDealersManagement.Application.Interfaces
{
    public interface IApplicationUserService
    {
        Task<TokenDto> GetAccessTokenAsync(LoginRequestDto message);

        Task<LoginResponseDto> LoginUserAsync(LoginRequestDto message);

        Task<RegisterUserResponseDto> RegisterNewUserAsync(RegisterUserRequestDto message);

        Task<bool> UnlockUserAsync(string userName);

        Task<string> DeleteUserAsync(string userName);
    }
}
