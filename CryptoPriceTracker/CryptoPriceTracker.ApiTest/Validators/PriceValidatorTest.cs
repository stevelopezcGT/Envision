namespace ApiTest.Validators;

/// <summary>
/// Contains unit tests for the <see cref="PriceValidator"/> class.
/// </summary>
public class Tests
{
    /// <summary>
    /// The <see cref="PriceValidator"/> instance used for testing.
    /// </summary>
    private readonly PriceValidator _validator = new PriceValidator();

    /// <summary>
    /// Tests that <see cref="PriceValidator.ShouldSavePrice"/> returns false when the price is zero.
    /// </summary>
    [Test]
    public void ShouldSavePrice_ReturnsFalse_WhenPriceIsZero()
    {
        // Arrange
        var history = new List<CryptoPriceHistory>();
        decimal newPrice = 0m;
        DateTime date = DateTime.UtcNow;

        // Act
        var result = _validator.ShouldSavePrice(newPrice, date, history);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that <see cref="PriceValidator.ShouldSavePrice"/> returns false when the price is negative.
    /// </summary>
    [Test]
    public void ShouldSavePrice_ReturnsFalse_WhenPriceIsNegative()
    {
        // Arrange
        var history = new List<CryptoPriceHistory>();
        decimal newPrice = -10m;
        DateTime date = DateTime.UtcNow;

        // Act
        var result = _validator.ShouldSavePrice(newPrice, date, history);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that <see cref="PriceValidator.ShouldSavePrice"/> returns false when the price and date already exist in the history.
    /// </summary>
    [Test]
    public void ShouldSavePrice_ReturnsFalse_WhenPriceAndDateExistInHistory()
    {
        // Arrange
        var date = new DateTime(2024, 5, 13);
        var history = new List<CryptoPriceHistory>
        {
            new CryptoPriceHistory { Date = date, Price = 100m }
        };
        decimal newPrice = 100m;

        // Act
        var result = _validator.ShouldSavePrice(newPrice, date, history);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that <see cref="PriceValidator.ShouldSavePrice"/> returns true when the price and date do not exist in the history.
    /// </summary>
    [Test]
    public void ShouldSavePrice_ReturnsTrue_WhenPriceAndDateDoNotExistInHistory()
    {
        // Arrange
        var date = new DateTime(2024, 5, 13);
        var history = new List<CryptoPriceHistory>
        {
            new CryptoPriceHistory { Date = date, Price = 99m },
            new CryptoPriceHistory { Date = date.AddDays(-1), Price = 100m }
        };
        decimal newPrice = 100m;

        // Act
        var result = _validator.ShouldSavePrice(newPrice, date, history);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that <see cref="PriceValidator.ShouldSavePrice"/> returns true when the history is empty and the price is positive.
    /// </summary>
    [Test]
    public void ShouldSavePrice_ReturnsTrue_WhenHistoryIsEmpty_AndPriceIsPositive()
    {
        // Arrange
        var history = new List<CryptoPriceHistory>();
        decimal newPrice = 50m;
        DateTime date = DateTime.UtcNow;

        // Act
        var result = _validator.ShouldSavePrice(newPrice, date, history);

        // Assert
        Assert.That(result, Is.True);
    }
}