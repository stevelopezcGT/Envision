namespace CryptoPriceTracker.ApplicationTest.Helpers;

public static class DBContextHelper
{
    public static ApplicationDbContext GetDbContextWithData(List<CryptoAsset> assets, List<CryptoPriceHistory> histories)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var dbContext = new ApplicationDbContext(options);
        dbContext.CryptoAssets.AddRange(assets);
        dbContext.CryptoPriceHistories.AddRange(histories);
        dbContext.SaveChanges();
        return dbContext;
    }
}