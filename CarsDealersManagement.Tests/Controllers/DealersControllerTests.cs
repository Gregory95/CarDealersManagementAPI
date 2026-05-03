using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using CarsDealersManagement.Microservice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Tests.Controllers;

public class DealersControllerTests
{
    private readonly Mock<IDealersService> _service;
    private readonly DealersController _controller;

    public DealersControllerTests()
    {
        _service = new Mock<IDealersService>();
        _controller = new DealersController(_service.Object);
    }

    [Fact]
    public async Task SearchDealersAsync_ReturnsOkResult()
    {
        var pagingRequest = new PagingRequest { PageNumber = 1, PageSize = 10 };
        _service.Setup(x => x.GetDealersPagingAsync(pagingRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagingWrap<Dealer>)null!);

        var result = await _controller.SearchDealersAsync(pagingRequest, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        _service.Verify(x => x.GetDealersPagingAsync(pagingRequest, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddDealerAsync_ReturnsNoContent()
    {
        var dto = new DealersDto { FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "123" };
        _service.Setup(x => x.CreateDealerAsync(dto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.AddDealerAsync(dto, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        _service.Verify(x => x.CreateDealerAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditDealerAsync_ReturnsOkWithUpdatedDto()
    {
        var dto = new DealersDto { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", PhoneNumber = "123" };
        _service.Setup(x => x.EditDealerAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.EditDealerAsync(dto, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(dto);
        _service.Verify(x => x.EditDealerAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDealerByIdAsync_ReturnsOkWithDto()
    {
        var dto = new DealersDto { Id = 1, FirstName = "John", LastName = "Doe" };
        _service.Setup(x => x.GetDealerByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.GetDealerByIdAsync(1, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task DeleteDealerAsync_ReturnsOkWithId()
    {
        _service.Setup(x => x.DeleteDealerAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeleteDealerAsync(1, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(1);
        _service.Verify(x => x.DeleteDealerAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
