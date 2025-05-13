namespace CryptoPriceTracker.ApplicationTest.Helpers;

public static class SettingsHelper
{
    public static IOptions<CoinGeckoSettings> GetCoinGeckoSettings()
    {
        return Options.Create(new CoinGeckoSettings
        {
            EndPoints = new EndPoint
            {
                SimplePrice = "https://api.coingecko.com/api/v3/simple/price",
                CoinsInfo = "https://api.coingecko.com/api/v3/coins"
            }
        });
    }
}