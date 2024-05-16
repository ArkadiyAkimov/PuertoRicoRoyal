using Microsoft.EntityFrameworkCore;
using PuertoRicoAPI.Data.DataClasses;

// dotnet ef database update 
// dotnet ef migrations add <name>

namespace PuertoRicoAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<DataSlot> Slots => Set<DataSlot>();
        public DbSet<DataRole> Roles => Set<DataRole>();
        public DbSet<DataPlayer> Players => Set<DataPlayer>();
        public DbSet<DataBuilding> Buildings => Set<DataBuilding>();
        public DbSet<DataPlayerBuilding> PlayerBuildings => Set<DataPlayerBuilding>();
        public DbSet<DataPlantation> Plantations => Set<DataPlantation>();
        public DbSet<DataPlayerPlantation> PlayerPlantations => Set<DataPlayerPlantation>();
        public DbSet<DataGameState> Games => Set<DataGameState>();
        public DbSet<DataPlayerGood> Goods => Set<DataPlayerGood>();
        public DbSet<DataTradeHouse> TradeHouses => Set<DataTradeHouse>();
        public DbSet<DataShip> Ships => Set<DataShip>();
    }
}
