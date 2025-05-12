using CryptoPriceTracker.Application.Dto;
using CryptoPriceTracker.Application.Interfaces;
using CryptoPriceTracker.Domain.Entities;
using CryptoPriceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CryptoPriceTracker.Application.Services;

/// <summary>
/// Service for managing and updating cryptocurrency prices.
/// </summary>
public class CryptoPriceService : ICryptoPriceService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly HttpClient _httpClient;

    private readonly string _currency = "usd";

    private readonly CoinGeckoSettings _coinGeckoSetting;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoPriceService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing and managing data.</param>
    /// <param name="httpClient">The HTTP client for making API requests.</param>
    public CryptoPriceService(ApplicationDbContext dbContext, HttpClient httpClient, IOptions<CoinGeckoSettings> coinGeckoSettings)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _coinGeckoSetting = coinGeckoSettings.Value;
    }

    /// <summary>
    /// Updates the prices of cryptocurrencies by fetching data from an external API
    /// and storing the latest prices in the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdatePricesAsync()
    {
        var cryptoAssets = await _dbContext.CryptoAssets.ToListAsync();
        string cryptoAssetsString = string.Join(",", cryptoAssets.Select(a => a.Name.ToLower()));

        var response = await _httpClient.GetAsync($"{_coinGeckoSetting.EndPoints.SimplePrice}?ids={cryptoAssetsString}&vs_currencies={_currency}");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var prices = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(json);

            bool pendingChanges = false;
            foreach (var asset in cryptoAssets)
            {
                var today = DateTime.UtcNow.Date;

                var lastPrice = await _dbContext.CryptoPriceHistories
                    .Where(p => p.CryptoAssetId == asset.Id)
                    .OrderByDescending(p => p.Date)
                    .FirstOrDefaultAsync();

                if (lastPrice != null && lastPrice.Date == today)
                {
                    continue;
                }

                var newPrice = prices.GetValueOrDefault(asset.ExternalId)?[_currency] ?? 0;
                if (newPrice > 0)
                {
                    var priceHistory = new CryptoPriceHistory
                    {
                        CryptoAssetId = asset.Id,
                        Price = newPrice,
                        Date = today
                    };
                    pendingChanges = true;
                    _dbContext.CryptoPriceHistories.Add(priceHistory);
                }
            }

            if (pendingChanges)
                _dbContext.SaveChanges();
        }
    }

    /// <summary>
    /// Retrieves a list of cryptocurrencies with their details and price information.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="CryptoCurrencyDto"/>.</returns>
    public async Task<List<CryptoCurrencyDto>> GetCryptoCurrenciesAsync()
    {
        var cryptoAssets = await _dbContext.CryptoAssets.ToListAsync();
        List<CryptoCurrencyDto> cryptoCurrencies = new List<CryptoCurrencyDto>();

        foreach (var asset in cryptoAssets)
        {
            var lastPrice = _dbContext.CryptoPriceHistories
                .Where(p => p.CryptoAssetId == asset.Id)
                .OrderByDescending(p => p.Date)
                .Take(2);

            if (lastPrice is not null)
            {
                var currentPrice = lastPrice.FirstOrDefault();
                var previousPrice = lastPrice.Skip(1).FirstOrDefault();

                var changePercentage =
                    previousPrice != null && currentPrice != null ? ((currentPrice.Price / previousPrice.Price) - 1) : 0;

                cryptoCurrencies.Add(new CryptoCurrencyDto
                {
                    Name = asset.Name,
                    Symbol = asset.Symbol,
                    CurrentPrice = currentPrice?.Price ?? 0m,
                    PriceChangePercentage24h = changePercentage,
                    LastUpdated = currentPrice?.Date,
                    Trend = changePercentage switch
                    {
                        > 0 => 1,
                        < 0 => -1,
                        _ => 0
                    },
                    Currency = _currency,
                    Icon = await GetIconUrl(asset.Name.ToLower())
                });
            }
        }

        return cryptoCurrencies;
    }

    /// <summary>
    /// Retrieves the icon URL for a given cryptocurrency asset.
    /// </summary>
    /// <param name="assetName">The name of the cryptocurrency asset.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the icon URL as a string.</returns>
    private async Task<string> GetIconUrl(string assetName)
    {
        var response = await _httpClient.GetAsync($"{_coinGeckoSetting.EndPoints.CoinsInfo}/{assetName}");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var coinDto = JsonConvert.DeserializeObject<CoinDto>(json);
            if (coinDto is not null)
                return coinDto.Image.Thumb;
        }
        return string.Empty;
    }
}