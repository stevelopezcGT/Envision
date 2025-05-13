namespace CryptoPriceTracker.ApplicationTest;

/// <summary>
/// Contains unit tests for the <see cref="CryptoPriceService"/> class.
/// </summary>
public class CryptoPriceServiceTest
{
    /// <summary>
    /// Tests that <see cref="CryptoPriceService.UpdatePricesAsync"/> adds a new price history entry
    /// when there is no price history for today.
    /// </summary>
    [Test]
    public async Task UpdatePricesAsync_AddsNewPriceHistory_WhenNoHistoryForToday()
    {
        // Arrange
        var asset = CryptoAssetEntity;
        var assets = new List<CryptoAsset> { asset };
        var histories = new List<CryptoPriceHistory>();
        var dbContext = Helpers.DBContextHelper.GetDbContextWithData(assets, histories);

        var priceDict = new Dictionary<string, Dictionary<string, decimal>>
        {
            { "bitcoin", new Dictionary<string, decimal> { { "usd", 50000m } } }
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(priceDict))
        };
        var httpClient = Helpers.HttpClientHelper.GetMockHttpClient(response);
        var service = new CryptoPriceService(dbContext, httpClient, Helpers.SettingsHelper.GetSettings());

        // Act
        await service.UpdatePricesAsync();

        // Assert
        var priceHistory = dbContext.CryptoPriceHistories.FirstOrDefault();
        Assert.Multiple(() =>
        {
            Assert.That(priceHistory, Is.Not.EqualTo(null));
            Assert.That(priceHistory.Id, Is.EqualTo(1));
            Assert.That(priceHistory.Price, Is.EqualTo(50000m));
            Assert.That(priceHistory.Date, Is.EqualTo(DateTime.UtcNow.Date));
        });
    }

    /// <summary>
    /// Tests that <see cref="CryptoPriceService.UpdatePricesAsync"/> does not add a new price history entry
    /// when an entry already exists for today.
    /// </summary>
    [Test]
    public async Task UpdatePricesAsync_DoesNotAddHistory_WhenAlreadyExistsForToday()
    {
        // Arrange
        var asset = CryptoAssetEntity;
        var today = DateTime.UtcNow.Date;
        var histories = new List<CryptoPriceHistory>
        {
            new CryptoPriceHistory { Id = 1, CryptoAssetId = 1, Date = today, Price = 40000m }
        };
        var dbContext = Helpers.DBContextHelper.GetDbContextWithData(new List<CryptoAsset> { asset }, histories);

        var priceDict = new Dictionary<string, Dictionary<string, decimal>>
        {
            { "bitcoin", new Dictionary<string, decimal> { { "usd", 50000m } } }
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(priceDict))
        };
        var httpClient = Helpers.HttpClientHelper.GetMockHttpClient(response);
        var service = new CryptoPriceService(dbContext, httpClient, Helpers.SettingsHelper.GetSettings());

        // Act
        await service.UpdatePricesAsync();

        // Assert
        // Should not add a new entry for today
        Assert.That(dbContext.CryptoPriceHistories.Count(h => h.CryptoAssetId == 1 && h.Date == today), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that <see cref="CryptoPriceService.UpdatePricesAsync"/> does nothing when the API call fails.
    /// </summary>
    [Test]
    public async Task UpdatePricesAsync_DoesNothing_WhenApiFails()
    {
        // Arrange
        var asset = CryptoAssetEntity;
        var dbContext = Helpers.DBContextHelper.GetDbContextWithData(new List<CryptoAsset> { asset }, new List<CryptoPriceHistory>());
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var httpClient = Helpers.HttpClientHelper.GetMockHttpClient(response);
        var service = new CryptoPriceService(dbContext, httpClient, Helpers.SettingsHelper.GetSettings());

        // Act
        await service.UpdatePricesAsync();

        // Assert
        Assert.That(dbContext.CryptoPriceHistories, Is.Empty);
    }

    /// <summary>
    /// Tests that <see cref="CryptoPriceService.GetCryptoCurrenciesAsync"/> returns correct data
    /// including price, trend, and price change percentage.
    /// </summary>
    [Test]
    public async Task GetCryptoCurrenciesAsync_ReturnsCorrectData()
    {
        // Arrange
        var asset = CryptoAssetEntity;
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var today = DateTime.UtcNow.Date;
        var histories = new List<CryptoPriceHistory>
        {
            new CryptoPriceHistory { Id = 1, CryptoAssetId = 1, Date = yesterday, Price = 40000m },
            new CryptoPriceHistory { Id = 2, CryptoAssetId = 1, Date = today, Price = 50000m }
        };
        var dbContext = Helpers.DBContextHelper.GetDbContextWithData(new List<CryptoAsset> { asset }, histories);

        var coinDto = new CoinDto
        {
            Image = new Image { Thumb = "https://icon.url/bitcoin.png" }
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(coinDto))
        };
        var httpClient = Helpers.HttpClientHelper.GetMockHttpClient(response);
        var service = new CryptoPriceService(dbContext, httpClient, Helpers.SettingsHelper.GetSettings());

        // Act
        var result = await service.GetCryptoCurrenciesAsync();

        // Assert
        Assert.That(result.Count == 1, Is.True);
        var dto = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(dto.Name, Is.EqualTo("Bitcoin"));
            Assert.That(dto.Symbol, Is.EqualTo("BTC"));
            Assert.That(dto.CurrentPrice, Is.EqualTo(50000m));
            Assert.That(dto.Currency, Is.EqualTo("usd"));
            Assert.That(dto.LastUpdated, Is.EqualTo(today));
            Assert.That(dto.Trend, Is.EqualTo(1));
            Assert.That(dto.Icon, Is.EqualTo("https://icon.url/bitcoin.png"));
            Assert.That(dto.PriceChangePercentage24h, Is.EqualTo(0.25m)); // (50000/40000)-1 = 0.25
        });
    }

    /// <summary>
    /// Tests that <see cref="CryptoPriceService.GetCryptoCurrenciesAsync"/> returns zero price change and neutral trend
    /// when there is no previous price history.
    /// </summary>
    [Test]
    public async Task GetCryptoCurrenciesAsync_ReturnsZeroChange_WhenNoPreviousPrice()
    {
        // Arrange
        var asset = CryptoAssetEntity;
        var today = DateTime.UtcNow.Date;
        var histories = new List<CryptoPriceHistory>
        {
            new CryptoPriceHistory { Id = 1, CryptoAssetId = 1, Date = today, Price = 50000m }
        };
        var dbContext = Helpers.DBContextHelper.GetDbContextWithData(new List<CryptoAsset> { asset }, histories);

        var coinDto = new CoinDto
        {
            Image = new Image { Thumb = "https://icon.url/bitcoin.png" }
        };
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(coinDto))
        };
        var httpClient = Helpers.HttpClientHelper.GetMockHttpClient(response);
        var service = new CryptoPriceService(dbContext, httpClient, Helpers.SettingsHelper.GetSettings());

        // Act
        var result = await service.GetCryptoCurrenciesAsync();

        // Assert
        Assert.That(result.Count == 1, Is.True);
        var dto = result.First();
        Assert.That(dto.PriceChangePercentage24h, Is.EqualTo(0m));
        Assert.That(dto.Trend, Is.EqualTo(0));
    }

    /// <summary>
    /// Gets a sample <see cref="CryptoAsset"/> entity for use in tests.
    /// </summary>
    private CryptoAsset CryptoAssetEntity => new CryptoAsset
    {
        Id = 1,
        Name = "Bitcoin",
        ExternalId = "bitcoin",
        Symbol = "BTC",
        IconUrl = "https://icon.url/bitcoin.png"
    };
}