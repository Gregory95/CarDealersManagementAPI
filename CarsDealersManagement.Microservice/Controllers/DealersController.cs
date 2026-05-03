using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Entities;
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
        public async Task<ActionResult<PagingWrap<Dealer>>> SearchDealersAsync([FromBody] PagingRequest message, CancellationToken ct)
        {
            var result = await _service.GetDealersPagingAsync(message, ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddDealerAsync([FromBody] DealersDto message, CancellationToken ct)
        {
            await _service.CreateDealerAsync(message, ct);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<DealersDto>> EditDealerAsync([FromBody] DealersDto message, CancellationToken ct)
        {
            var result = await _service.EditDealerAsync(message, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DealersDto>> GetDealerByIdAsync(int id, CancellationToken ct)
        {
            var result = await _service.GetDealerByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DealersDto>> DeleteDealerAsync(int id, CancellationToken ct)
        {
            await _service.DeleteDealerAsync(id, ct);
            return Ok(id);
        }
    }
}
