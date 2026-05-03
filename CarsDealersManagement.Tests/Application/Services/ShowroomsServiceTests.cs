using AutoMapper;
using CarsDealersManagement.Application.Services;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using CarsDealersManagement.Domain.Repositories;
using FluentAssertions;
using Moq;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Tests.Application.Services;

public class ShowroomsServiceTests
{
    private readonly Mock<IShowroomsRepository> _repo;
    private readonly Mock<IMapper> _mapper;
    private readonly ShowroomsService _service;

    public ShowroomsServiceTests()
    {
        _repo = new Mock<IShowroomsRepository>();
        _mapper = new Mock<IMapper>();
        _service = new ShowroomsService(_repo.Object, _mapper.Object);
    }

    // ── GetDetailsAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetDetailsAsync_WhenNoShowroomsExist_ReturnsEmptyCollection()
    {
        _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Showroom>());
        _mapper.Setup(x => x.Map<IEnumerable<ShowroomDto>>(It.IsAny<IEnumerable<Showroom>>()))
            .Returns(new List<ShowroomDto>());

        var result = await _service.GetDetailsAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDetailsAsync_WhenShowroomsExist_ReturnsMappedDtos()
    {
        var showrooms = new List<Showroom>
        {
            Showroom.Create("Showroom A", "Athens", 30),
            Showroom.Create("Showroom B", "Thessaloniki", 60)
        };
        var expectedDtos = new List<ShowroomDto>
        {
            new() { Name = "Showroom A", Location = "Athens", Capacity = 30 },
            new() { Name = "Showroom B", Location = "Thessaloniki", Capacity = 60 }
        };

        _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(showrooms);
        _mapper.Setup(x => x.Map<IEnumerable<ShowroomDto>>(showrooms))
            .Returns(expectedDtos);

        var result = await _service.GetDetailsAsync(CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedDtos);
    }

    // ── CreateShowroomAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateShowroomAsync_MapsDto_ThenCallsAddAsync()
    {
        var dto = new ShowroomDto { Name = "New Showroom", Location = "Athens", Capacity = 40, DealerId = 1 };
        var entity = Showroom.Create("New Showroom", "Athens", 40);

        _mapper.Setup(x => x.Map<Showroom>(dto)).Returns(entity);
        _repo.Setup(x => x.AddAsync(entity, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _service.CreateShowroomAsync(dto, CancellationToken.None);

        _mapper.Verify(x => x.Map<Showroom>(dto), Times.Once);
        _repo.Verify(x => x.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── EditShowroomAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task EditShowroomAsync_WhenShowroomNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new ShowroomDto { Id = 99, Name = "Ghost", Location = "Nowhere", Capacity = 0 };

        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Showroom?)null);

        var act = async () => await _service.EditShowroomAsync(dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Showroom not found");
    }

    [Fact]
    public async Task EditShowroomAsync_WhenShowroomFound_UpdatesEntityAndReturnsMappedDto()
    {
        var existing = Showroom.Create("Old Name", "Old Location", 10);
        existing.Id = 1;
        var dto = new ShowroomDto { Id = 1, Name = "New Name", Location = "New Location", Capacity = 50 };
        var expectedDto = new ShowroomDto { Id = 1, Name = "New Name", Location = "New Location", Capacity = 50 };

        _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _repo.Setup(x => x.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapper.Setup(x => x.Map<ShowroomDto>(existing))
            .Returns(expectedDto);

        var result = await _service.EditShowroomAsync(dto, CancellationToken.None);

        existing.Name.Should().Be("New Name");
        existing.Location.Should().Be("New Location");
        existing.Capacity.Should().Be(50);
        result.Should().BeEquivalentTo(expectedDto);
        _repo.Verify(x => x.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── GetShowroomByIdAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetShowroomByIdAsync_WhenShowroomNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Showroom?)null);

        var act = async () => await _service.GetShowroomByIdAsync(99, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Showroom not found");
    }

    [Fact]
    public async Task GetShowroomByIdAsync_WhenShowroomFound_ReturnsMappedDto()
    {
        var showroom = Showroom.Create("Main Showroom", "Athens", 50);
        showroom.Id = 1;
        var expectedDto = new ShowroomDto { Id = 1, Name = "Main Showroom", Location = "Athens", Capacity = 50 };

        _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(showroom);
        _mapper.Setup(x => x.Map<ShowroomDto>(showroom))
            .Returns(expectedDto);

        var result = await _service.GetShowroomByIdAsync(1, CancellationToken.None);

        result.Should().BeEquivalentTo(expectedDto);
    }

    // ── DeleteShowroomAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteShowroomAsync_WhenShowroomNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Showroom?)null);

        var act = async () => await _service.DeleteShowroomAsync(99, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Showroom not found");
    }

    [Fact]
    public async Task DeleteShowroomAsync_WhenShowroomFound_CallsDeleteAsync()
    {
        var showroom = Showroom.Create("Main Showroom", "Athens", 50);
        showroom.Id = 1;

        _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(showroom);
        _repo.Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.DeleteShowroomAsync(1, CancellationToken.None);

        _repo.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── GetShowroomsPagingAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetShowroomsPagingAsync_CallsGetAllAndGetPagedAsync_OnRepository()
    {
        var pagingRequest = new PagingRequest { PageNumber = 2, PageSize = 5 };
        var queryable = new List<Showroom>().AsQueryable();

        _repo.Setup(x => x.GetAll()).Returns(queryable);
        _repo.Setup(x => x.GetPagedAsync(queryable, 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagingWrap<Showroom>)null!);

        await _service.GetShowroomsPagingAsync(pagingRequest, CancellationToken.None);

        _repo.Verify(x => x.GetAll(), Times.Once);
        _repo.Verify(x => x.GetPagedAsync(queryable, 2, 5, It.IsAny<CancellationToken>()), Times.Once);
    }
}
