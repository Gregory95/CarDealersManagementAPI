using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagingPackage;

namespace CarsDealersManagement.Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Authorize]
    public class DealersController(IDealersService _service) : ControllerBase
    {
        [HttpPost("search")]
        public async Task<ActionResult<PagingToolkit<DealersDto>>> SearchDealersAsync([FromBody] PagingParameters message)
        {
            var result = await _service.GetDealersPagingAsync(message, CancellationToken.None);
            return Ok(result);
        }
    }
}
