using System.ComponentModel.DataAnnotations;
/// <summary>
/// Class User helps to setup a user with specified ID, name, score and time of creation.
/// </summary>

namespace UserRankingSystem.Models {
    public class User {
        public int Id { get; set; }

        // Ensure user always has name on creation or modification.
        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        // Ensures email always exists.
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address provided.")]
        public required string Email { get; set; }

        // Ensure score is always positive.
        [Range(0, int.MaxValue, ErrorMessage = "Score must be a positive integer.")]
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
