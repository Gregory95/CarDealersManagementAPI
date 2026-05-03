using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Microservice.Controllers;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Tests.Controllers;

public class ShowroomsControllerTests
{
    private readonly Mock<IShowroomsService> _service;
    private readonly ShowroomsController _controller;

    public ShowroomsControllerTests()
    {
        _service = new Mock<IShowroomsService>();
        _controller = new ShowroomsController(_service.Object);
    }

    [Fact]
    public async Task SearchShowroomsAsync_ReturnsOkResult()
    {
        var pagingRequest = new PagingRequest { PageNumber = 1, PageSize = 10 };
        _service.Setup(x => x.GetShowroomsPagingAsync(pagingRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagingWrap<Showroom>)null!);

        var result = await _controller.SearchShowroomsAsync(pagingRequest, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        _service.Verify(x => x.GetShowroomsPagingAsync(pagingRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddShowroomAsync_ReturnsNoContent()
    {
        var dto = new ShowroomDto { Name = "Showroom A", Location = "Athens", Capacity = 30, DealerId = 1 };
        _service.Setup(x => x.CreateShowroomAsync(dto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.AddShowroomAsync(dto, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        _service.Verify(x => x.CreateShowroomAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditShowroomAsync_ReturnsOkWithUpdatedDto()
    {
        var dto = new ShowroomDto { Id = 1, Name = "Updated Showroom", Location = "Piraeus", Capacity = 40 };
        _service.Setup(x => x.EditShowroomAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.EditShowroomAsync(dto, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetShowroomByIdAsync_ReturnsOkWithDto()
    {
        var dto = new ShowroomDto { Id = 1, Name = "Main Showroom", Location = "Athens", Capacity = 50 };
        _service.Setup(x => x.GetShowroomByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.GetShowroomByIdAsync(1, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task DeleteShowroomAsync_ReturnsOkWithId()
    {
        _service.Setup(x => x.DeleteShowroomAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeleteShowroomAsync(1, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(1);
        _service.Verify(x => x.DeleteShowroomAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
