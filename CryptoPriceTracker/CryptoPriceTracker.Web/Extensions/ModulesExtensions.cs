using CryptoPriceTracker.Application.Dto;

namespace CryptoPriceTracker.Web.Extensions;

public static class ModulesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SiteSettings>(configuration.GetSection("SiteSettings"));

        services.AddHttpClient();

        return services;
    }
}