using Microsoft.EntityFrameworkCore;
using UserRankingSystem.Models;

/// <summary>
/// UserRankingSystem class deals with interacting with the users data in the database.
/// </summary>

namespace UserRankingSystem.Data {
    public class UserRankingContext: DbContext {

        // Data table of Users.
        public DbSet<User> Users { get; set; }

        public UserRankingContext(DbContextOptions<UserRankingContext> options) : base(options) {}

        protected override void OnModelCreation(ModelBuilder modelBuilder) {
            // Email for all users should be unique.
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}