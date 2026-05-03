using AutoMapper;
using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Application.Services;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CarsDealersManagement.Tests.Application.Services;

public class ApplicationUserServiceTests
{
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<IEmailSender> _emailSender;
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
    private readonly ApplicationUserService _service;

    public ApplicationUserServiceTests()
    {
        _mapper = new Mock<IMapper>();
        _configuration = new Mock<IConfiguration>();
        _emailSender = new Mock<IEmailSender>();

        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManager = new Mock<SignInManager<ApplicationUser>>(
            _userManager.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

        _configuration.Setup(x => x["JWT:Secret"]).Returns("SuperSecretTestKey_AtLeast32CharsLong!!");
        _configuration.Setup(x => x["JWT:ValidIssuer"]).Returns("https://test.issuer.com");
        _configuration.Setup(x => x["JWT:ValidAudience"]).Returns("https://test.audience.com");
        _configuration.Setup(x => x["ConnectionStrings:Uri"]).Returns("https://localhost:5001");

        _service = new ApplicationUserService(
            _mapper.Object,
            _configuration.Object,
            _emailSender.Object,
            _userManager.Object,
            _signInManager.Object);
    }

    private static ApplicationUser BuildUser(string userName = "testuser", string email = "test@example.com", int accessFailedCount = 0)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userName,
            Email = email,
            FirstName = "Test",
            LastName = "User",
            AccessFailedCount = accessFailedCount,
            PasswordHash = "hashed_password"
        };
    }

    // ── GetAccessTokenAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task GetAccessTokenAsync_WhenUserNotFound_ThrowsUnauthorizedAccessException()
    {
        _userManager.Setup(x => x.FindByNameAsync("unknown"))
            .ReturnsAsync((ApplicationUser?)null);

        var request = new LoginRequestDto { UserName = "unknown", Password = "pass" };

        var act = async () => await _service.GetAccessTokenAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*does not exist*");
    }

    [Fact]
    public async Task GetAccessTokenAsync_WhenPasswordIsWrong_ThrowsUnauthorizedAccessException()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.CheckPasswordAsync(user, "wrongpass")).ReturnsAsync(false);
        _userManager.Setup(x => x.AccessFailedAsync(user)).ReturnsAsync(IdentityResult.Success);

        var request = new LoginRequestDto { UserName = "testuser", Password = "wrongpass" };

        var act = async () => await _service.GetAccessTokenAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*not valid*");
    }

    [Fact]
    public async Task GetAccessTokenAsync_WhenPasswordIsWrong_CallsAccessFailedAsync()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.CheckPasswordAsync(user, "wrongpass")).ReturnsAsync(false);
        _userManager.Setup(x => x.AccessFailedAsync(user)).ReturnsAsync(IdentityResult.Success);

        var request = new LoginRequestDto { UserName = "testuser", Password = "wrongpass" };

        try { await _service.GetAccessTokenAsync(request); } catch { }

        _userManager.Verify(x => x.AccessFailedAsync(user), Times.Once);
    }

    [Fact]
    public async Task GetAccessTokenAsync_WhenCredentialsValid_ReturnsTokenWithAccessToken()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.CheckPasswordAsync(user, "correctpass")).ReturnsAsync(true);
        _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

        var request = new LoginRequestDto { UserName = "testuser", Password = "correctpass" };

        var result = await _service.GetAccessTokenAsync(request);

        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.ExpiresIn.Should().BeGreaterThan(0);
    }

    // ── LoginUserAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginUserAsync_WhenUserNotFound_ThrowsUnauthorizedAccessException()
    {
        _userManager.Setup(x => x.FindByNameAsync("unknown"))
            .ReturnsAsync((ApplicationUser?)null);

        var request = new LoginRequestDto { UserName = "unknown", Password = "pass" };

        var act = async () => await _service.LoginUserAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*does not exist*");
    }

    [Fact]
    public async Task LoginUserAsync_WhenUserIsLockedOut_ThrowsUnauthorizedAccessException()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(true);

        var request = new LoginRequestDto { UserName = "testuser", Password = "pass" };

        var act = async () => await _service.LoginUserAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*locked*");
    }

    [Fact]
    public async Task LoginUserAsync_WhenEmailNotConfirmed_ThrowsUnauthorizedAccessException()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _signInManager.Setup(x => x.CanSignInAsync(user)).ReturnsAsync(false);

        var request = new LoginRequestDto { UserName = "testuser", Password = "pass" };

        var act = async () => await _service.LoginUserAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*confirmation email*");
    }

    [Fact]
    public async Task LoginUserAsync_WhenPasswordIsWrong_ThrowsUnauthorizedAccessException()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _signInManager.Setup(x => x.CanSignInAsync(user)).ReturnsAsync(true);
        _userManager.Setup(x => x.CheckPasswordAsync(user, "wrongpass")).ReturnsAsync(false);
        _userManager.Setup(x => x.AccessFailedAsync(user)).ReturnsAsync(IdentityResult.Success);

        var request = new LoginRequestDto { UserName = "testuser", Password = "wrongpass" };

        var act = async () => await _service.LoginUserAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*not correct*");
    }

    [Fact]
    public async Task LoginUserAsync_WhenFourthFailedPasswordAttempt_EnablesLockout()
    {
        var user = BuildUser(accessFailedCount: 4);
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _signInManager.Setup(x => x.CanSignInAsync(user)).ReturnsAsync(true);
        _userManager.Setup(x => x.CheckPasswordAsync(user, "wrongpass")).ReturnsAsync(false);
        _userManager.Setup(x => x.AccessFailedAsync(user)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.SetLockoutEnabledAsync(user, true)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset?>())).ReturnsAsync(IdentityResult.Success);

        var request = new LoginRequestDto { UserName = "testuser", Password = "wrongpass" };

        try { await _service.LoginUserAsync(request); } catch { }

        _userManager.Verify(x => x.SetLockoutEnabledAsync(user, true), Times.Once);
        _userManager.Verify(x => x.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset?>()), Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_WhenRequiresTwoFactor_ThrowsUnauthorizedAccessException()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _signInManager.Setup(x => x.CanSignInAsync(user)).ReturnsAsync(true);
        _userManager.Setup(x => x.CheckPasswordAsync(user, "correctpass")).ReturnsAsync(true);
        _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());
        _signInManager.Setup(x => x.PasswordSignInAsync("testuser", "correctpass", true, false))
            .ReturnsAsync(SignInResult.TwoFactorRequired);

        var request = new LoginRequestDto { UserName = "testuser", Password = "correctpass" };

        var act = async () => await _service.LoginUserAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*authenticator*");
    }

    [Fact]
    public async Task LoginUserAsync_WhenCredentialsValid_ReturnsLoginResponseWithToken()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _signInManager.Setup(x => x.CanSignInAsync(user)).ReturnsAsync(true);
        _userManager.Setup(x => x.CheckPasswordAsync(user, "correctpass")).ReturnsAsync(true);
        _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
        _signInManager.Setup(x => x.PasswordSignInAsync("testuser", "correctpass", true, false))
            .ReturnsAsync(SignInResult.Success);
        _userManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var request = new LoginRequestDto { UserName = "testuser", Password = "correctpass" };

        var result = await _service.LoginUserAsync(request);

        result.Should().NotBeNull();
        result.UserName.Should().Be("testuser");
        result.Token.Should().NotBeNull();
        result.Token.AccessToken.Should().NotBeNullOrEmpty();
        _userManager.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task LoginUserAsync_WhenCredentialsValid_SetsLastLoginDate()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _signInManager.Setup(x => x.CanSignInAsync(user)).ReturnsAsync(true);
        _userManager.Setup(x => x.CheckPasswordAsync(user, "correctpass")).ReturnsAsync(true);
        _userManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string>());
        _signInManager.Setup(x => x.PasswordSignInAsync("testuser", "correctpass", true, false))
            .ReturnsAsync(SignInResult.Success);
        _userManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var before = DateTime.UtcNow.AddSeconds(-1);
        await _service.LoginUserAsync(new LoginRequestDto { UserName = "testuser", Password = "correctpass" });
        var after = DateTime.UtcNow.AddSeconds(1);

        user.LastLoginDate.Should().NotBeNull();
        user.LastLoginDate.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    // ── RegisterNewUserAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task RegisterNewUserAsync_WhenEmailAlreadyExists_ThrowsException()
    {
        var existingUser = BuildUser();
        _userManager.Setup(x => x.FindByEmailAsync("existing@example.com")).ReturnsAsync(existingUser);
        _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

        var request = new RegisterUserRequestDto
        {
            UserName = "newuser",
            Email = "existing@example.com",
            Password = "Pass@123",
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "123"
        };

        var act = async () => await _service.RegisterNewUserAsync(request);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task RegisterNewUserAsync_WhenUsernameAlreadyExists_ThrowsException()
    {
        var existingUser = BuildUser();
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        _userManager.Setup(x => x.FindByNameAsync("existinguser")).ReturnsAsync(existingUser);

        var request = new RegisterUserRequestDto
        {
            UserName = "existinguser",
            Email = "new@example.com",
            Password = "Pass@123",
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "123"
        };

        var act = async () => await _service.RegisterNewUserAsync(request);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*already exists*");
    }

    // BUG: The condition `if (newUserResult.Succeeded)` is inverted.
    // This test documents the EXPECTED behavior (success path returns the DTO).
    // It will FAIL until the bug is fixed: change `if (newUserResult.Succeeded)`
    // to `if (!newUserResult.Succeeded)` in RegisterNewUserAsync.
    [Fact]
    public async Task RegisterNewUserAsync_WhenUserCreationSucceeds_ReturnsResponseDto()
    {
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _emailSender.Setup(x => x.SendEmailAsync(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _mapper.Setup(x => x.Map<RegisterUserResponseDto>(It.IsAny<ApplicationUser>()))
            .Returns(new RegisterUserResponseDto { Username = "newuser", Email = "new@example.com" });

        var request = new RegisterUserRequestDto
        {
            UserName = "newuser",
            Email = "new@example.com",
            Password = "Pass@123",
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "123"
        };

        var result = await _service.RegisterNewUserAsync(request);

        result.Should().NotBeNull();
        result.Username.Should().Be("newuser");
    }

    [Fact]
    public async Task RegisterNewUserAsync_WhenUserCreationFails_ThrowsException()
    {
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        var request = new RegisterUserRequestDto
        {
            UserName = "newuser",
            Email = "new@example.com",
            Password = "weak",
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "123"
        };

        var act = async () => await _service.RegisterNewUserAsync(request);

        // The correct behavior is that a failed creation should throw.
        // Currently the code DOES throw when Succeeded=false (because the inverted condition misses this path).
        // After the bug fix, this test verifies that failures are properly reported.
        await act.Should().ThrowAsync<Exception>();
    }

    // ── DeleteUserAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteUserAsync_WhenUserNotFound_ThrowsException()
    {
        _userManager.Setup(x => x.FindByNameAsync("unknown"))
            .ReturnsAsync((ApplicationUser?)null);

        var act = async () => await _service.DeleteUserAsync("unknown");

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*does not exist*");
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserFound_CallsDeleteAndReturnsUserId()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        var result = await _service.DeleteUserAsync("testuser");

        result.Should().Be(user.Id);
        _userManager.Verify(x => x.DeleteAsync(user), Times.Once);
    }

    // ── UnlockUserAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task UnlockUserAsync_WhenUserNotFound_ThrowsException()
    {
        _userManager.Setup(x => x.FindByNameAsync("unknown"))
            .ReturnsAsync((ApplicationUser?)null);

        var act = async () => await _service.UnlockUserAsync("unknown");

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*does not exist*");
    }

    [Fact]
    public async Task UnlockUserAsync_WhenUserIsNotLocked_ThrowsException()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);

        var act = async () => await _service.UnlockUserAsync("testuser");

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*not locked*");
    }

    [Fact]
    public async Task UnlockUserAsync_WhenUserIsLocked_DisablesLockoutAndReturnsTrue()
    {
        var user = BuildUser();
        _userManager.Setup(x => x.FindByNameAsync("testuser")).ReturnsAsync(user);
        _userManager.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(true);
        _userManager.Setup(x => x.SetLockoutEnabledAsync(user, false)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(x => x.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset?>())).ReturnsAsync(IdentityResult.Success);

        var result = await _service.UnlockUserAsync("testuser");

        result.Should().BeTrue();
        _userManager.Verify(x => x.SetLockoutEnabledAsync(user, false), Times.Once);
        _userManager.Verify(x => x.SetLockoutEndDateAsync(user, It.IsAny<DateTimeOffset?>()), Times.Once);
    }
}
