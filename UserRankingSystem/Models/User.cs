/// <summary>
/// Class User helps to setup a user with specified ID, name, score and time of creation.
/// </summary>

namespace UserRankingSystem.Models {
    public class User {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
