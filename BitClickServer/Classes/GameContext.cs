using System;
using Microsoft.EntityFrameworkCore;

namespace BitClickServer
{
    public class GameContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Account> Accounts { get; set; }
       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source=bitclick.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Player
            modelBuilder.Entity<Player>().HasKey(c => c.Id);

            // Inventory
            modelBuilder.Entity<Inventory>().HasKey(c => c.Id);

            // Item
            modelBuilder.Entity<Item>().HasKey(c => c.GUID);

            // Account
            modelBuilder.Entity<Account>().HasKey(c => c.Id);

            base.OnModelCreating(modelBuilder);
        }

        public GameContext()
        {
        }
    }
}
