using CarsDealersManagement.Domain.Entities;
using FluentAssertions;

namespace CarsDealersManagement.Tests.Domain;

public class DealerTests
{
    [Fact]
    public void Create_WithValidParameters_SetsAllProperties()
    {
        var dealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");

        dealer.FirstName.Should().Be("John");
        dealer.LastName.Should().Be("Doe");
        dealer.Email.Should().Be("john@example.com");
        dealer.PhoneNumber.Should().Be("1234567890");
    }

    [Fact]
    public void Create_ReturnsNewDealerInstance()
    {
        var dealer = Dealer.Create("Jane", "Smith", "jane@example.com", "0987654321");

        dealer.Should().NotBeNull();
        dealer.Should().BeOfType<Dealer>();
    }

    [Fact]
    public void Create_InitializesShowroomsAsEmptyCollection()
    {
        var dealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");

        dealer.Showrooms.Should().NotBeNull();
        dealer.Showrooms.Should().BeEmpty();
    }

    [Fact]
    public void Update_ChangesAllProperties()
    {
        var dealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");

        dealer.Update("Jane", "Smith", "jane@example.com", "0987654321");

        dealer.FirstName.Should().Be("Jane");
        dealer.LastName.Should().Be("Smith");
        dealer.Email.Should().Be("jane@example.com");
        dealer.PhoneNumber.Should().Be("0987654321");
    }

    [Fact]
    public void Update_DoesNotCreateNewInstance()
    {
        var dealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");
        var originalRef = dealer;

        dealer.Update("Jane", "Smith", "jane@example.com", "0987654321");

        dealer.Should().BeSameAs(originalRef);
    }

    [Fact]
    public void GetFullName_ReturnsConcatenatedFirstAndLastName()
    {
        var dealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");

        var fullName = dealer.GetFullName();

        fullName.Should().Be("John Doe");
    }

    [Fact]
    public void GetFullName_AfterUpdate_ReflectsNewName()
    {
        var dealer = Dealer.Create("John", "Doe", "john@example.com", "1234567890");
        dealer.Update("Jane", "Smith", "jane@example.com", "0987654321");

        var fullName = dealer.GetFullName();

        fullName.Should().Be("Jane Smith");
    }

    [Fact]
    public void DefaultConstructor_CreatesInstanceWithoutException()
    {
        var action = () => new Dealer();

        action.Should().NotThrow();
    }
}
