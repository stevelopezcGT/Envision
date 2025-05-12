namespace CryptoPriceTracker.Application.Dto;

public class CoinGeckoSetting
{
    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public EndPoint EndPoints { get; set; } = null!;
}

public class EndPoint
{
    public string SimplePrice { get; set; } = string.Empty;

    public string CoinsInfo { get; set; } = string.Empty;
}