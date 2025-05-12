using CryptoPriceTracker.Application.Dto;
using CryptoPriceTracker.Application.Interfaces;
using CryptoPriceTracker.Application.Services;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace CryptoPriceTracker.Api.Extensions;

public static class ModulesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICryptoPriceService, CryptoPriceService>();

        services.Configure<CoinGeckoSetting>(configuration.GetSection("CoinGeckoSetting"));

        services.AddHttpClient<ICryptoPriceService, CryptoPriceService>(client =>
        {
            var coinGeckoSetting = configuration.GetSection("CoinGeckoSetting").Get<CoinGeckoSetting>();

            client.BaseAddress = new Uri(coinGeckoSetting.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("x-cg-demo-api-key", coinGeckoSetting.ApiKey);
        });

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "CryptoPriceTracker.Api", Version = "v1.0" });

            var xmlFiles = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory), "*.xml");
            foreach (var filePath in xmlFiles)
            {
                options.IncludeXmlComments(filePath);
            }
        });

        return services;
    }
}