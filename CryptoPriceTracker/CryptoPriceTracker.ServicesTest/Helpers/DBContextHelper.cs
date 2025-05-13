using CryptoPriceTracker.Domain.Entities;
using CryptoPriceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CryptoPriceTracker.ApplicationTest.Helpers;

public static class DBContextHelper
{
    public static ApplicationDbContext GetDbContextWithData()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())

            // No other changes are needed in the file.
            .Options;

        var dbContext = new ApplicationDbContext(options);

        dbContext.CryptoAssets.Add(new CryptoAsset
        {
            Id = 1,
            Name = "Bitcoin",
            Symbol = "BTC",
            ExternalId = "bitcoin",
            IconUrl = string.Empty
        });

        dbContext.CryptoPriceHistories.Add(new CryptoPriceHistory
        {
            Id = 1,
            CryptoAssetId = 1,
            Price = 50000,
            Date = DateTime.UtcNow.Date.AddDays(-1)
        });

        dbContext.SaveChanges();
        return dbContext;
    }
}