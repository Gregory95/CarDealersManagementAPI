using AutoMapper;
using CarsDealersManagement.Application.Services;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using CarsDealersManagement.Domain.Repositories;
using FluentAssertions;
using Moq;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Tests.Application.Services;

public class ContactPersonsServiceTests
{
    private readonly Mock<IContactPersonsRepository> _repo;
    private readonly Mock<IMapper> _mapper;
    private readonly ContactPersonsService _service;

    public ContactPersonsServiceTests()
    {
        _repo = new Mock<IContactPersonsRepository>();
        _mapper = new Mock<IMapper>();
        _service = new ContactPersonsService(_repo.Object, _mapper.Object);
    }

    // ── GetDetailsAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetDetailsAsync_WhenNoContactPersonsExist_ReturnsEmptyCollection()
    {
        _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ContactPerson>());
        _mapper.Setup(x => x.Map<IEnumerable<ContactPersonDto>>(It.IsAny<IEnumerable<ContactPerson>>()))
            .Returns(new List<ContactPersonDto>());

        var result = await _service.GetDetailsAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDetailsAsync_WhenContactPersonsExist_ReturnsMappedDtos()
    {
        var contacts = new List<ContactPerson>
        {
            ContactPerson.Create("Alice", "Brown", "alice@example.com", "111"),
            ContactPerson.Create("Bob", "Green", "bob@example.com", "222")
        };
        var expectedDtos = new List<ContactPersonDto>
        {
            new() { FirstName = "Alice", LastName = "Brown" },
            new() { FirstName = "Bob", LastName = "Green" }
        };

        _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(contacts);
        _mapper.Setup(x => x.Map<IEnumerable<ContactPersonDto>>(contacts))
            .Returns(expectedDtos);

        var result = await _service.GetDetailsAsync(CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedDtos);
    }

    // ── CreateContactPersonAsync ───────────────────────────────────────────────

    [Fact]
    public async Task CreateContactPersonAsync_MapsDto_ThenCallsAddAsync()
    {
        var dto = new ContactPersonDto { FirstName = "Alice", LastName = "Brown", Email = "alice@example.com", PhoneNumber = "111", ShowroomId = 1 };
        var entity = ContactPerson.Create("Alice", "Brown", "alice@example.com", "111");

        _mapper.Setup(x => x.Map<ContactPerson>(dto)).Returns(entity);
        _repo.Setup(x => x.AddAsync(entity, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _service.CreateContactPersonAsync(dto, CancellationToken.None);

        _mapper.Verify(x => x.Map<ContactPerson>(dto), Times.Once);
        _repo.Verify(x => x.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── EditContactPersonAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task EditContactPersonAsync_WhenContactPersonNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new ContactPersonDto { Id = 99, FirstName = "Ghost", LastName = "Person", PhoneNumber = "000" };

        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContactPerson?)null);

        var act = async () => await _service.EditContactPersonAsync(dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Contact person not found");
    }

    [Fact]
    public async Task EditContactPersonAsync_WhenContactPersonFound_UpdatesEntityAndReturnsMappedDto()
    {
        var existing = ContactPerson.Create("Alice", "Brown", "alice@example.com", "111");
        existing.Id = 1;
        var dto = new ContactPersonDto { Id = 1, FirstName = "Bob", LastName = "Green", Email = "bob@example.com", PhoneNumber = "222" };
        var expectedDto = new ContactPersonDto { Id = 1, FirstName = "Bob", LastName = "Green", Email = "bob@example.com", PhoneNumber = "222" };

        _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _repo.Setup(x => x.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapper.Setup(x => x.Map<ContactPersonDto>(existing))
            .Returns(expectedDto);

        var result = await _service.EditContactPersonAsync(dto, CancellationToken.None);

        existing.FirstName.Should().Be("Bob");
        existing.LastName.Should().Be("Green");
        existing.Email.Should().Be("bob@example.com");
        existing.PhoneNumber.Should().Be("222");
        result.Should().BeEquivalentTo(expectedDto);
        _repo.Verify(x => x.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── GetContactPersonByIdAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetContactPersonByIdAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContactPerson?)null);

        var act = async () => await _service.GetContactPersonByIdAsync(99, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Contact person not found");
    }

    [Fact]
    public async Task GetContactPersonByIdAsync_WhenFound_ReturnsMappedDto()
    {
        var contact = ContactPerson.Create("Alice", "Brown", "alice@example.com", "111");
        contact.Id = 1;
        var expectedDto = new ContactPersonDto { Id = 1, FirstName = "Alice", LastName = "Brown" };

        _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);
        _mapper.Setup(x => x.Map<ContactPersonDto>(contact))
            .Returns(expectedDto);

        var result = await _service.GetContactPersonByIdAsync(1, CancellationToken.None);

        result.Should().BeEquivalentTo(expectedDto);
    }

    // ── DeleteContactPersonAsync ───────────────────────────────────────────────

    [Fact]
    public async Task DeleteContactPersonAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContactPerson?)null);

        var act = async () => await _service.DeleteContactPersonAsync(99, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Contact person not found");
    }

    [Fact]
    public async Task DeleteContactPersonAsync_WhenFound_CallsDeleteAsync()
    {
        var contact = ContactPerson.Create("Alice", "Brown", "alice@example.com", "111");
        contact.Id = 1;

        _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);
        _repo.Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.DeleteContactPersonAsync(1, CancellationToken.None);

        _repo.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── GetContactPersonsPagingAsync ───────────────────────────────────────────

    [Fact]
    public async Task GetContactPersonsPagingAsync_CallsGetAllAndGetPagedAsync_OnRepository()
    {
        var pagingRequest = new PagingRequest { PageNumber = 1, PageSize = 20 };
        var queryable = new List<ContactPerson>().AsQueryable();

        _repo.Setup(x => x.GetAll()).Returns(queryable);
        _repo.Setup(x => x.GetPagedAsync(queryable, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagingWrap<ContactPerson>)null!);

        await _service.GetContactPersonsPagingAsync(pagingRequest, CancellationToken.None);

        _repo.Verify(x => x.GetAll(), Times.Once);
        _repo.Verify(x => x.GetPagedAsync(queryable, 1, 20, It.IsAny<CancellationToken>()), Times.Once);
    }
}
