namespace CryptoPriceTracker.Application.Dto;

/// <summary>
/// Represents a cryptocurrency with its details and price information.
/// </summary>
public class CryptoCurrencyDto
{
    /// <summary>
    /// Gets or sets the name of the cryptocurrency.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the symbol of the cryptocurrency (e.g., BTC for Bitcoin).
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current price of the cryptocurrency.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Gets or sets the currency in which the price is denominated (e.g., USD).
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL or path to the icon representing the cryptocurrency.
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the price was last updated.
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets the trend of the cryptocurrency price.
    /// 1 indicates an upward trend, -1 indicates a downward trend, and 0 indicates no change.
    /// </summary>
    public int Trend { get; set; }

    /// <summary>
    /// Gets or sets the percentage change in price over the last 24 hours.
    /// </summary>
    public decimal PriceChangePercentage24h { get; set; }
}