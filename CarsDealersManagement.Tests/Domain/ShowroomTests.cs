using CarsDealersManagement.Domain.Entities;
using FluentAssertions;

namespace CarsDealersManagement.Tests.Domain;

public class ShowroomTests
{
    [Fact]
    public void Create_WithValidParameters_SetsAllProperties()
    {
        var showroom = Showroom.Create("Main Showroom", "Athens, Greece", 50);

        showroom.Name.Should().Be("Main Showroom");
        showroom.Location.Should().Be("Athens, Greece");
        showroom.Capacity.Should().Be(50);
    }

    [Fact]
    public void Create_ReturnsNewShowroomInstance()
    {
        var showroom = Showroom.Create("Main Showroom", "Athens, Greece", 50);

        showroom.Should().NotBeNull();
        showroom.Should().BeOfType<Showroom>();
    }

    [Fact]
    public void Create_InitializesContactPersonsAsEmptyCollection()
    {
        var showroom = Showroom.Create("Main Showroom", "Athens, Greece", 50);

        showroom.ContactPersons.Should().NotBeNull();
        showroom.ContactPersons.Should().BeEmpty();
    }

    [Fact]
    public void Update_ChangesAllProperties()
    {
        var showroom = Showroom.Create("Main Showroom", "Athens, Greece", 50);

        showroom.Update("North Showroom", "Thessaloniki, Greece", 100);

        showroom.Name.Should().Be("North Showroom");
        showroom.Location.Should().Be("Thessaloniki, Greece");
        showroom.Capacity.Should().Be(100);
    }

    [Fact]
    public void Update_DoesNotCreateNewInstance()
    {
        var showroom = Showroom.Create("Main Showroom", "Athens, Greece", 50);
        var originalRef = showroom;

        showroom.Update("North Showroom", "Thessaloniki, Greece", 100);

        showroom.Should().BeSameAs(originalRef);
    }

    [Fact]
    public void DefaultConstructor_CreatesInstanceWithoutException()
    {
        var action = () => new Showroom();

        action.Should().NotThrow();
    }
}
