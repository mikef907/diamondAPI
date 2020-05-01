using Common.Lib.Models.EM;
using Microsoft.EntityFrameworkCore;

namespace Games.Lib
{
    public class GamesContext : DbContext
    {
        public GamesContext(DbContextOptions options) : base(options) { }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<GameState> GameStates { get; set; }
        public DbSet<PlayerMatch> PlayerMatches { get; set; }
        public DbSet<GameMove> GameMoves { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerMatch>().HasKey(t => new { t.PlayerId, t.MatchId });

            modelBuilder.Entity<PlayerMatch>()
                .HasOne(pt => pt.Player)
                .WithMany(p => p.PlayerMatches)
                .HasForeignKey(pt => pt.PlayerId);

            modelBuilder.Entity<PlayerMatch>()
                .HasOne(pt => pt.Match)
                .WithMany(t => t.PlayerMatches)
                .HasForeignKey(pt => pt.MatchId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
