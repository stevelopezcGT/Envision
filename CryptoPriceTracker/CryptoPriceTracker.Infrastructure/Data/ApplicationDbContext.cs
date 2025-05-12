using CryptoPriceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoPriceTracker.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<CryptoAsset> CryptoAssets { get; set; }

    public DbSet<CryptoPriceHistory> CryptoPriceHistories { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CryptoAsset>().HasData(
            new CryptoAsset { Id = 1, Name = "Bitcoin", Symbol = "BTC", ExternalId = "bitcoin", IconUrl = string.Empty },
            new CryptoAsset { Id = 2, Name = "Ethereum", Symbol = "ETH", ExternalId = "ethereum", IconUrl = string.Empty }
        );
    }
}

//dotnet ef migrations add "InitialCreate" --project CryptoPriceTracker.Infrastructure --startup-project CryptoPriceTracker.Api --output-dir Data\Migrations
//dotnet ef database update --project CryptoPriceTracker.Infrastructure --startup-project CryptoPriceTracker.Api 