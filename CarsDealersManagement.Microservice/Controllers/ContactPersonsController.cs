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
        public async Task<ActionResult<PagingWrap<ContactPerson>>> SearchContactPersonsAsync([FromBody] PagingRequest message)
        {
            var result = await _service.GetContactPersonsPagingAsync(message, CancellationToken.None);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddContactPersonAsync([FromBody] ContactPersonDto message)
        {
            await _service.CreateContactPersonAsync(message, CancellationToken.None);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<ContactPersonDto>> EditContactPersonAsync([FromBody] ContactPersonDto message)
        {
            var result = await _service.EditContactPersonAsync(message, CancellationToken.None);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactPersonDto>> GetContactPersonByIdAsync(int id)
        {
            var result = await _service.GetContactPersonByIdAsync(id, CancellationToken.None);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ContactPersonDto>> DeleteContactPersonAsync(int id)
        {
            await _service.DeleteContactPersonAsync(id, CancellationToken.None);
            return Ok(id);
        }
    }
}
