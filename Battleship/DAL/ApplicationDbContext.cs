using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationDbContext : DbContext
    {
        private static string ConnectionString =
            "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_ditarr_BattleShip;MultipleActiveResultSets=true";

        public DbSet<GameConfig> GameConfigs { get; set; } = default!;
        public DbSet<ShipConfig> ShipConfigs { get; set; } = default!;
        public DbSet<SavedGame> SavedGames { get; set; } = default!;
        public DbSet<GameBoard> GameBoards { get; set; } = default!;
        public DbSet<Ship> Ships { get; set; } = default!;
        public DbSet<Coordinate> Coordinates { get; set; } = default!;

        // not recommended - do not hardcode DB conf!
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}
