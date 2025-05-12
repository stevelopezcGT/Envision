using Newtonsoft.Json;

namespace CryptoPriceTracker.Application.Dto;

public class CoinDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("image")]
    public Image Image { get; set; } = null!;
}

public class Image
{
    [JsonProperty("thumb")]
    public string Thumb { get; set; } = string.Empty;

    [JsonProperty("small")]
    public string Small { get; set; } = string.Empty;

    [JsonProperty("large")]
    public string Large { get; set; } = string.Empty;
}