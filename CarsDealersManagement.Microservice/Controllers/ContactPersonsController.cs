using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.Query.EntityFramework;

namespace CarsContactPersonsManagement.Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Authorize]
    public class ContactPersonsController(IContactPersonsService _service) : ControllerBase
    {
        [HttpPost("search")]
        public async Task<ActionResult<PagingWrap<ContactPerson>>> SearchContactPersonsAsync([FromBody] PagingRequest message, CancellationToken ct)
        {
            var result = await _service.GetContactPersonsPagingAsync(message, ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddContactPersonAsync([FromBody] ContactPersonDto message, CancellationToken ct)
        {
            await _service.CreateContactPersonAsync(message, ct);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<ContactPersonDto>> EditContactPersonAsync([FromBody] ContactPersonDto message, CancellationToken ct)
        {
            var result = await _service.EditContactPersonAsync(message, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactPersonDto>> GetContactPersonByIdAsync(int id, CancellationToken ct)
        {
            var result = await _service.GetContactPersonByIdAsync(id, ct);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ContactPersonDto>> DeleteContactPersonAsync(int id, CancellationToken ct)
        {
            await _service.DeleteContactPersonAsync(id, ct);
            return Ok(id);
        }
    }
}
