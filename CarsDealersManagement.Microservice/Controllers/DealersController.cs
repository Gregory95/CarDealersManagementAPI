using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.Query.EntityFramework;

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
        public async Task<ActionResult<PagingWrap<DealersDto>>> SearchDealersAsync([FromBody] PagingRequest message)
        {
            var result = await _service.GetDealersPagingAsync(message, CancellationToken.None);
            return Ok(result);
        }
    }
}
