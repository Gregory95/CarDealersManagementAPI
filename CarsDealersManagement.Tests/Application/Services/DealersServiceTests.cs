using AutoMapper;
using CarsDealersManagement.Application.Services;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using CarsDealersManagement.Domain.Repositories;
using FluentAssertions;
using Moq;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Tests.Application.Services;

public class DealersServiceTests
{
    private readonly Mock<IDealersRepository> _repo;
    private readonly Mock<IMapper> _mapper;
    private readonly DealersService _service;

    public DealersServiceTests()
    {
        _repo = new Mock<IDealersRepository>();
        _mapper = new Mock<IMapper>();
        _service = new DealersService(_repo.Object, _mapper.Object);
    }

    // ── GetDetailsAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetDetailsAsync_WhenNoDealersExist_ReturnsEmptyCollection()
    {
        _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Dealer>());
        _mapper.Setup(x => x.Map<IEnumerable<DealersDto>>(It.IsAny<IEnumerable<Dealer>>()))
            .Returns(new List<DealersDto>());

        var result = await _service.GetDetailsAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDetailsAsync_WhenDealersExist_ReturnsMappedDtos()
    {
        var dealers = new List<Dealer>
        {
            Dealer.Create("John", "Doe", "john@example.com", "1234567890"),
            Dealer.Create("Jane", "Smith", "jane@example.com", "0987654321")
        };
        var expectedDtos = new List<DealersDto>
        {
            new() { FirstName = "John", LastName = "Doe" },
            new() { FirstName = "Jane", LastName = "Smith" }
        };

        _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dealers);
        _mapper.Setup(x => x.Map<IEnumerable<DealersDto>>(dealers))
            .Returns(expectedDtos);

        var result = await _service.GetDetailsAsync(CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task GetDetailsAsync_CallsGetAllAsync_OnRepository()
    {
        _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Dealer>());
        _mapper.Setup(x => x.Map<IEnumerable<DealersDto>>(It.IsAny<IEnumerable<Dealer>>()))
            .Returns(new List<DealersDto>());

        await _service.GetDetailsAsync(CancellationToken.None);

        _repo.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── CreateDealerAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task CreateDealerAsync_MapsDto_ThenCallsAddAsync()
    {
        var dto = new DealersDto { FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "1234567890" };
        var entity = Dealer.Create("John", "Doe", "john@example.com", "1234567890");

        _mapper.Setup(x => x.Map<Dealer>(dto)).Returns(entity);
        _repo.Setup(x => x.AddAsync(entity, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _service.CreateDealerAsync(dto, CancellationToken.None);

        _mapper.Verify(x => x.Map<Dealer>(dto), Times.Once);
        _repo.Verify(x => x.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── EditDealerAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task EditDealerAsync_WhenDealerNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new DealersDto { Id = 99, FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "1234567890" };

        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dealer?)null);

        var act = async () => await _service.EditDealerAsync(dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Dealer not found");
    }

    [Fact]
    public async Task EditDealerAsync_WhenDealerFound_UpdatesEntityAndReturnsMappedDto()
    {
        var existingDealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");
        existingDealer.Id = 1;
        var dto = new DealersDto { Id = 1, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", PhoneNumber = "0987654321" };
        var expectedDto = new DealersDto { Id = 1, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", PhoneNumber = "0987654321" };

        _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDealer);
        _repo.Setup(x => x.UpdateAsync(existingDealer, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapper.Setup(x => x.Map<DealersDto>(existingDealer))
            .Returns(expectedDto);

        var result = await _service.EditDealerAsync(dto, CancellationToken.None);

        existingDealer.FirstName.Should().Be("Jane");
        existingDealer.LastName.Should().Be("Smith");
        existingDealer.Email.Should().Be("jane@example.com");
        existingDealer.PhoneNumber.Should().Be("0987654321");
        result.Should().BeEquivalentTo(expectedDto);
        _repo.Verify(x => x.UpdateAsync(existingDealer, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditDealerAsync_WhenDealerNotFound_NeverCallsUpdateAsync()
    {
        var dto = new DealersDto { Id = 99, FirstName = "John", LastName = "Doe", Email = "j@e.com", PhoneNumber = "123" };

        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dealer?)null);

        try { await _service.EditDealerAsync(dto, CancellationToken.None); } catch { }

        _repo.Verify(x => x.UpdateAsync(It.IsAny<Dealer>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── GetDealerByIdAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetDealerByIdAsync_WhenDealerNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dealer?)null);

        var act = async () => await _service.GetDealerByIdAsync(99, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Dealer not found");
    }

    [Fact]
    public async Task GetDealerByIdAsync_WhenDealerFound_ReturnsMappedDto()
    {
        var dealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");
        dealer.Id = 1;
        var expectedDto = new DealersDto { Id = 1, FirstName = "John", LastName = "Doe" };

        _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dealer);
        _mapper.Setup(x => x.Map<DealersDto>(dealer))
            .Returns(expectedDto);

        var result = await _service.GetDealerByIdAsync(1, CancellationToken.None);

        result.Should().BeEquivalentTo(expectedDto);
    }

    // ── DeleteDealerAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteDealerAsync_WhenDealerNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dealer?)null);

        var act = async () => await _service.DeleteDealerAsync(99, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Dealer not found");
    }

    [Fact]
    public async Task DeleteDealerAsync_WhenDealerFound_CallsDeleteAsync()
    {
        var dealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");
        dealer.Id = 1;

        _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dealer);
        _repo.Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.DeleteDealerAsync(1, CancellationToken.None);

        _repo.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteDealerAsync_WhenDealerNotFound_NeverCallsDeleteAsync()
    {
        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dealer?)null);

        try { await _service.DeleteDealerAsync(99, CancellationToken.None); } catch { }

        _repo.Verify(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── GetDealersPagingAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task GetDealersPagingAsync_CallsGetAllAndGetPagedAsync_OnRepository()
    {
        var pagingRequest = new PagingRequest { PageNumber = 1, PageSize = 10 };
        var queryable = new List<Dealer>().AsQueryable();

        _repo.Setup(x => x.GetAll()).Returns(queryable);
        _repo.Setup(x => x.GetPagedAsync(queryable, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagingWrap<Dealer>)null!);

        await _service.GetDealersPagingAsync(pagingRequest, CancellationToken.None);

        _repo.Verify(x => x.GetAll(), Times.Once);
        _repo.Verify(x => x.GetPagedAsync(queryable, 1, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
