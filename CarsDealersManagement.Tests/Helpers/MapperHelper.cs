using AutoMapper;
using CarsDealersManagement.Application;
using Microsoft.Extensions.DependencyInjection;

namespace CarsDealersManagement.Tests.Helpers;

public static class MapperHelper
{
    public static IMapper Create()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfiles).Assembly));
        return services.BuildServiceProvider().GetRequiredService<IMapper>();
    }
}
