using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.Text;

namespace CarsDealersManagement.Infrastructure;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Database"));
            options.UseOpenIddict();
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(5);

            options.Password.RequireDigit = true;
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/\\";
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddOpenIddict()
                // Register the OpenIddict core components.
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default entities.
                .AddCore(options =>
                {
                    options
                        .UseEntityFrameworkCore()
                        .UseDbContext<AppDbContext>();
                })
                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    options.SetTokenEndpointUris("/token");
                    options.AllowClientCredentialsFlow();
                    options.AllowPasswordFlow();
                    options.AcceptAnonymousClients();
                    options.SetAccessTokenLifetime(TimeSpan.FromHours(6));
                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();
                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    options
                        .UseAspNetCore()
                        .EnableTokenEndpointPassthrough()
                        .DisableTransportSecurityRequirement();
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

        services.AddCors(
            options => options.AddPolicy("AllowCors",
            builder =>
            {
                builder.SetIsOriginAllowed(origin => true)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
            })
        );

        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        });

        // Adding Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        // Adding Jwt Bearer
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = configuration["JWT:ValidAudience"],
                ValidIssuer = configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
            };
        });

        services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

        return services;
    }
}