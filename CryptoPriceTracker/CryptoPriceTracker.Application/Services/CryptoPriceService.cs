using CryptoPriceTracker.Application.Interfaces;
using CryptoPriceTracker.Domain.Entities;
using CryptoPriceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CryptoPriceTracker.Application.Services;

/// <summary>
/// Service for managing and updating cryptocurrency prices.
/// </summary>
public class CryptoPriceService : ICryptoPriceService
{
    private readonly ApplicationDbContext _dbContext;

    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoPriceService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing and managing data.</param>
    /// <param name="httpClient">The HTTP client for making API requests.</param>
    public CryptoPriceService(ApplicationDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Updates the prices of cryptocurrencies by fetching data from an external API
    /// and storing the latest prices in the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdatePricesAsync()
    {
        // Retrieve the list of cryptocurrency assets from the database.
        var cryptoAssets = await _dbContext.CryptoAssets.ToListAsync();

        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("x-cg-demo-api-key", "CG-ycBGXkiM7uK7QfVqCsoCwy9n");

        // Fetch the latest prices from the external API.
        var response = await _httpClient.GetAsync("https://ap2i.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum&vs_currencies=usd");

        if (response.IsSuccessStatusCode)
        {
            // Parse the JSON response into a dictionary of prices.
            var json = await response.Content.ReadAsStringAsync();
            var prices = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(json);

            bool pendingChanges = false;
            foreach (var asset in cryptoAssets)
            {
                // Get the current date in UTC.
                var today = DateTime.UtcNow.Date;

                // Retrieve the most recent price history for the asset.
                var lastPrice = await _dbContext.CryptoPriceHistories
                    .Where(p => p.CryptoAssetId == asset.Id)
                    .OrderByDescending(p => p.Date)
                    .FirstOrDefaultAsync();

                // Skip if a price for today already exists.
                if (lastPrice != null && lastPrice.Date == today)
                {
                    continue;
                }

                // Get the new price for the asset from the API response.
                var newPrice = prices.GetValueOrDefault(asset.ExternalId)?["usd"] ?? 0;
                if (newPrice > 0)
                {
                    // Create a new price history record and add it to the database.
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
}