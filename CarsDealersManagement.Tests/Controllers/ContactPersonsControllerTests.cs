using CarsContactPersonsManagement.Microservice.Controllers;
using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Tests.Controllers;

public class ContactPersonsControllerTests
{
    private readonly Mock<IContactPersonsService> _service;
    private readonly ContactPersonsController _controller;

    public ContactPersonsControllerTests()
    {
        _service = new Mock<IContactPersonsService>();
        _controller = new ContactPersonsController(_service.Object);
    }

    [Fact]
    public async Task SearchContactPersonsAsync_ReturnsOkResult()
    {
        var pagingRequest = new PagingRequest { PageNumber = 1, PageSize = 10 };
        _service.Setup(x => x.GetContactPersonsPagingAsync(pagingRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagingWrap<ContactPerson>)null!);

        var result = await _controller.SearchContactPersonsAsync(pagingRequest, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        _service.Verify(x => x.GetContactPersonsPagingAsync(pagingRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddContactPersonAsync_ReturnsNoContent()
    {
        var dto = new ContactPersonDto { FirstName = "Alice", LastName = "Brown", PhoneNumber = "111", ShowroomId = 1 };
        _service.Setup(x => x.CreateContactPersonAsync(dto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.AddContactPersonAsync(dto, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        _service.Verify(x => x.CreateContactPersonAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditContactPersonAsync_ReturnsOkWithUpdatedDto()
    {
        var dto = new ContactPersonDto { Id = 1, FirstName = "Bob", LastName = "Green", Email = "bob@example.com", PhoneNumber = "222" };
        _service.Setup(x => x.EditContactPersonAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.EditContactPersonAsync(dto, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetContactPersonByIdAsync_ReturnsOkWithDto()
    {
        var dto = new ContactPersonDto { Id = 1, FirstName = "Alice", LastName = "Brown" };
        _service.Setup(x => x.GetContactPersonByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.GetContactPersonByIdAsync(1, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task DeleteContactPersonAsync_ReturnsOkWithId()
    {
        _service.Setup(x => x.DeleteContactPersonAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeleteContactPersonAsync(1, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(1);
        _service.Verify(x => x.DeleteContactPersonAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
