using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CarsDealersManagement.Application
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfiles).Assembly));

            services.AddScoped<IApplicationUserService, ApplicationUserService>();
            services.AddScoped<IDealersService, DealersService>();
            services.AddScoped<IContactPersonsService, ContactPersonsService>();
            services.AddScoped<IEmailSender, EmailSender>();

            return services;
        }
    }
}
