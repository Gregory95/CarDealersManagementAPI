using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Models;
using CarsDealersManagement.Microservice.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CarsDealersManagement.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IApplicationUserService> _service;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _service = new Mock<IApplicationUserService>();
        _controller = new UserController(_service.Object);
    }

    [Fact]
    public async Task GetAccessTokenAsync_ReturnsOkWithToken()
    {
        var request = new LoginRequestDto { UserName = "testuser", Password = "pass" };
        var token = new TokenDto { AccessToken = "jwt_token_here", ExpiresIn = 9999999999 };

        _service.Setup(x => x.GetAccessTokenAsync(request)).ReturnsAsync(token);

        var result = await _controller.GetAccessTokenAsync(request);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(token);
        _service.Verify(x => x.GetAccessTokenAsync(request), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ReturnsOkWithLoginResponse()
    {
        var request = new LoginRequestDto { UserName = "testuser", Password = "pass" };
        var loginResponse = new LoginResponseDto
        {
            UserName = "testuser",
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            IsExternal = false,
            Token = new TokenDto { AccessToken = "jwt_token", ExpiresIn = 9999999999 }
        };

        _service.Setup(x => x.LoginUserAsync(request)).ReturnsAsync(loginResponse);

        var result = await _controller.LoginAsync(request);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(loginResponse);
        _service.Verify(x => x.LoginUserAsync(request), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_ReturnsOkWithRegisterResponse()
    {
        var request = new RegisterUserRequestDto
        {
            UserName = "newuser",
            Email = "new@example.com",
            Password = "Pass@123",
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "123"
        };
        var response = new RegisterUserResponseDto { Username = "newuser", Email = "new@example.com" };

        _service.Setup(x => x.RegisterNewUserAsync(request)).ReturnsAsync(response);

        var result = await _controller.RegisterUserAsync(request);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(response);
        _service.Verify(x => x.RegisterNewUserAsync(request), Times.Once);
    }

    [Fact]
    public async Task UnlockUserAsync_ReturnsOkWithTrueResult()
    {
        var dto = new UserNameDto { UserName = "lockeduser" };
        _service.Setup(x => x.UnlockUserAsync("lockeduser")).ReturnsAsync(true);

        var result = await _controller.UnlockUserAsync(dto);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(true);
        _service.Verify(x => x.UnlockUserAsync("lockeduser"), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsOkWithUserId()
    {
        _service.Setup(x => x.DeleteUserAsync("testuser")).ReturnsAsync("user-guid-123");

        var result = await _controller.DeleteUserAsync("testuser");

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be("user-guid-123");
        _service.Verify(x => x.DeleteUserAsync("testuser"), Times.Once);
    }
}
