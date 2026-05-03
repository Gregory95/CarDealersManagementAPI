using AutoMapper;
using CarsDealersManagement.Application;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using CarsDealersManagement.Tests.Helpers;
using FluentAssertions;

namespace CarsDealersManagement.Tests.Application;

public class MappingProfilesTests
{
    private readonly IMapper _mapper = MapperHelper.Create();

    [Fact]
    public void Configuration_IsValid()
    {
        var action = () => MapperHelper.Create();

        action.Should().NotThrow();
    }

    [Fact]
    public void Dealer_MapsTo_DealersDto_WithCorrectValues()
    {
        var dealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");
        dealer.Id = 1;

        var dto = _mapper.Map<DealersDto>(dealer);

        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
        dto.Email.Should().Be("john@example.com");
        dto.PhoneNumber.Should().Be("1234567890");
        dto.Id.Should().Be(1);
    }

    [Fact]
    public void DealersDto_MapsTo_Dealer_WithCorrectValues()
    {
        var dto = new DealersDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890"
        };

        var dealer = _mapper.Map<Dealer>(dto);

        dealer.FirstName.Should().Be("John");
        dealer.LastName.Should().Be("Doe");
        dealer.Email.Should().Be("john@example.com");
        dealer.PhoneNumber.Should().Be("1234567890");
    }

    [Fact]
    public void Showroom_MapsTo_ShowroomDto_WithCorrectValues()
    {
        var showroom = Showroom.Create("Main Showroom", "Athens", 50);
        showroom.Id = 2;
        showroom.DealerId = 1;

        var dto = _mapper.Map<ShowroomDto>(showroom);

        dto.Name.Should().Be("Main Showroom");
        dto.Location.Should().Be("Athens");
        dto.Capacity.Should().Be(50);
        dto.DealerId.Should().Be(1);
        dto.Id.Should().Be(2);
    }

    [Fact]
    public void ShowroomDto_MapsTo_Showroom_WithCorrectValues()
    {
        var dto = new ShowroomDto
        {
            Id = 2,
            Name = "Main Showroom",
            Location = "Athens",
            Capacity = 50,
            DealerId = 1
        };

        var showroom = _mapper.Map<Showroom>(dto);

        showroom.Name.Should().Be("Main Showroom");
        showroom.Location.Should().Be("Athens");
        showroom.Capacity.Should().Be(50);
        showroom.DealerId.Should().Be(1);
    }

    [Fact]
    public void ContactPerson_MapsTo_ContactPersonDto_WithCorrectValues()
    {
        var contact = ContactPerson.Create("Alice", "Brown", "alice@example.com", "5551234567");
        contact.Id = 3;
        contact.ShowroomId = 2;

        var dto = _mapper.Map<ContactPersonDto>(contact);

        dto.FirstName.Should().Be("Alice");
        dto.LastName.Should().Be("Brown");
        dto.Email.Should().Be("alice@example.com");
        dto.PhoneNumber.Should().Be("5551234567");
        dto.ShowroomId.Should().Be(2);
        dto.Id.Should().Be(3);
    }

    [Fact]
    public void ContactPersonDto_MapsTo_ContactPerson_WithCorrectValues()
    {
        var dto = new ContactPersonDto
        {
            Id = 3,
            FirstName = "Alice",
            LastName = "Brown",
            Email = "alice@example.com",
            PhoneNumber = "5551234567",
            ShowroomId = 2
        };

        var contact = _mapper.Map<ContactPerson>(dto);

        contact.FirstName.Should().Be("Alice");
        contact.LastName.Should().Be("Brown");
        contact.Email.Should().Be("alice@example.com");
        contact.PhoneNumber.Should().Be("5551234567");
        contact.ShowroomId.Should().Be(2);
    }

    [Fact]
    public void ContactPerson_WithNullEmail_MapsEmailAsNull()
    {
        var contact = ContactPerson.Create("Alice", "Brown", null, "5551234567");

        var dto = _mapper.Map<ContactPersonDto>(contact);

        dto.Email.Should().BeNull();
    }
}
