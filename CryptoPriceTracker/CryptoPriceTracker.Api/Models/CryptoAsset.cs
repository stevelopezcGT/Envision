namespace CryptoPriceTracker.Api.Models;

public class CryptoAsset
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public string ExternalId { get; set; } = null!;

    public ICollection<CryptoPriceHistory> PriceHistory { get; set; } = null!;
}