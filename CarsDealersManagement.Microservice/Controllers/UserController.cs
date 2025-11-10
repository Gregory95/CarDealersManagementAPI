using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Models;
using Microsoft.AspNetCore.Mvc;


namespace CarsDealersManagement.Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class UserController(IApplicationUserService _service) : ControllerBase
    {
        [HttpPost("access-token")]
        public async Task<ActionResult<TokenDto>> GetAccessTokenAsync([FromBody] LoginRequestDto message)
        {
            var result = await _service.GetAccessTokenAsync(message);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> LoginAsync([FromBody] LoginRequestDto message)
        {
            var result = await _service.LoginUserAsync(message);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserResponseDto>> RegisterUserAsync([FromBody] RegisterUserRequestDto message)
        {
            var result = await _service.RegisterNewUserAsync(message);
            return Ok(result);
        }

        [HttpPost("unlock")]
        public async Task<ActionResult<bool>> UnlockUserAsync([FromBody] UserNameDto message)
        {
            var result = await _service.UnlockUserAsync(message.UserName);
            return Ok(result);
        }

        [HttpDelete("delete{userName}")]
        public async Task<ActionResult<string>> DeleteUserAsync(string userName)
        {
            var result = await _service.DeleteUserAsync(userName);
            return Ok(result);
        }
    }
}
