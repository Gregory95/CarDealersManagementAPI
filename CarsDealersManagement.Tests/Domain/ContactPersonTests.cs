using CarsDealersManagement.Domain.Entities;
using FluentAssertions;

namespace CarsDealersManagement.Tests.Domain;

public class ContactPersonTests
{
    [Fact]
    public void Create_WithValidParameters_SetsAllProperties()
    {
        var contact = ContactPerson.Create("Alice", "Brown", "alice@example.com", "5551234567");

        contact.FirstName.Should().Be("Alice");
        contact.LastName.Should().Be("Brown");
        contact.Email.Should().Be("alice@example.com");
        contact.PhoneNumber.Should().Be("5551234567");
    }

    [Fact]
    public void Create_WithNullEmail_SetsEmailToNull()
    {
        var contact = ContactPerson.Create("Alice", "Brown", null, "5551234567");

        contact.Email.Should().BeNull();
    }

    [Fact]
    public void Create_ReturnsNewContactPersonInstance()
    {
        var contact = ContactPerson.Create("Alice", "Brown", "alice@example.com", "5551234567");

        contact.Should().NotBeNull();
        contact.Should().BeOfType<ContactPerson>();
    }

    [Fact]
    public void Update_ChangesAllProperties()
    {
        var contact = ContactPerson.Create("Alice", "Brown", "alice@example.com", "5551234567");

        contact.Update("Bob", "Green", "bob@example.com", "5559876543");

        contact.FirstName.Should().Be("Bob");
        contact.LastName.Should().Be("Green");
        contact.Email.Should().Be("bob@example.com");
        contact.PhoneNumber.Should().Be("5559876543");
    }

    [Fact]
    public void Update_WithNullEmail_SetsEmailToNull()
    {
        var contact = ContactPerson.Create("Alice", "Brown", "alice@example.com", "5551234567");

        contact.Update("Alice", "Brown", null, "5551234567");

        contact.Email.Should().BeNull();
    }

    [Fact]
    public void Update_DoesNotCreateNewInstance()
    {
        var contact = ContactPerson.Create("Alice", "Brown", "alice@example.com", "5551234567");
        var originalRef = contact;

        contact.Update("Bob", "Green", "bob@example.com", "5559876543");

        contact.Should().BeSameAs(originalRef);
    }

    [Fact]
    public void DefaultConstructor_CreatesInstanceWithoutException()
    {
        var action = () => new ContactPerson();

        action.Should().NotThrow();
    }
}
