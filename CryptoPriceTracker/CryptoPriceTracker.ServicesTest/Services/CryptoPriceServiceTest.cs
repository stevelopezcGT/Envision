using CryptoPriceTracker.Application.Services;
using FluentAssertions;
using Newtonsoft.Json;

namespace CryptoPriceTracker.ApplicationTest;

public class CryptoPriceServiceTest
{
    [SetUp]
    public void Setup()
    {
        // This method is called before each test in the class.
        // You can use it to set up any common test data or configurations.
    }

    [Test]
    public async Task UpdatePricesAsync_ShouldAddNewPriceHistory_WhenApiReturnsValidData()
    {
        // Arrange
        var dbContext = Helpers.DBContextHelper.GetDbContextWithData();
        var priceResponse = JsonConvert.SerializeObject(new Dictionary<string, Dictionary<string, decimal>>
        {
            { "bitcoin", new Dictionary<string, decimal> { { "usd", 60000 } } }
        });
        var httpClient = Helpers.HttpClientHelper.GetMockHttpClient("simple/price", priceResponse);
        var service = new CryptoPriceService(dbContext, httpClient, Helpers.SettingsHelper.GetCoinGeckoSettings());

        // Act
        await service.UpdatePricesAsync();

        // Assert
        var today = DateTime.UtcNow.Date;
        var currentPrice = 60000m;
        var priceHistory = dbContext.CryptoPriceHistories
            .FirstOrDefault(p => p.CryptoAssetId == 1 && p.Date == today);
        Assert.NotNull(priceHistory);
        Assert.That(currentPrice, Is.EqualTo(priceHistory.Price));
    }

    [Test]
    public async Task GetCryptoCurrenciesAsync_ShouldReturnListWithCorrectData()
    {
        // Arrange
        var dbContext = Helpers.DBContextHelper.GetDbContextWithData();
        var coinInfoResponse = JsonConvert.SerializeObject(new
        {
            image = new { thumb = "https://icon.url/btc.png" }
        });
        var httpClient = Helpers.HttpClientHelper.GetMockHttpClient("coins/bitcoin", coinInfoResponse);
        var service = new CryptoPriceService(dbContext, httpClient, Helpers.SettingsHelper.GetCoinGeckoSettings());

        // Act
        var result = await service.GetCryptoCurrenciesAsync();
        var cryptoCurrencyDto = new
        {
            Name = "Bitcoin",
            Symbol = "BTC",
            CurrentPrice = 50000,
            Currency = "usd",
            Icon = "https://icon.url/btc.png"
        };

        // Assert
        var btc = result.First();
        btc.Should().BeEquivalentTo(cryptoCurrencyDto);
    }
}