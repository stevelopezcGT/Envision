namespace CryptoPriceTracker.ApplicationTest.Helpers;

public static class SettingsHelper
{
    public static IOptions<CoinGeckoSettings> GetSettings()
    {
        return Options.Create(new CoinGeckoSettings
        {
            EndPoints = new Application.Dto.EndPoint
            {
                SimplePrice = "https://api.coingecko.com/api/v3/simple/price",
                CoinsInfo = "https://api.coingecko.com/api/v3/coins"
            }
        });
    }
}