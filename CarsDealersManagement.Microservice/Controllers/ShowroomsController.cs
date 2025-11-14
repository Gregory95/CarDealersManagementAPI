using CarsShowroomsManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.Query.EntityFramework;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;

namespace CarsShowroomsManagement.Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Authorize]
    public class ShowroomsController(IShowroomsService _service) : ControllerBase
    {
        [HttpPost("search")]
        public async Task<ActionResult<PagingWrap<Showroom>>> SearchShowroomsAsync([FromBody] PagingRequest message)
        {
            var result = await _service.GetShowroomsPagingAsync(message, CancellationToken.None);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddShowroomAsync([FromBody] ShowroomDto message)
        {
            await _service.CreateShowroomAsync(message, CancellationToken.None);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<ShowroomDto>> EditShowroomAsync([FromBody] ShowroomDto message)
        {
            var result = await _service.EditShowroomAsync(message, CancellationToken.None);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShowroomDto>> GetShowroomByIdAsync(int id)
        {
            var result = await _service.GetShowroomByIdAsync(id, CancellationToken.None);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ShowroomDto>> DeleteShowroomAsync(int id)
        {
            await _service.DeleteShowroomAsync(id, CancellationToken.None);
            return Ok(id);
        }
    }
}
