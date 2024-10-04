/// <summary>
/// Class UserController deals with RESTful API calls with regards to CRUD operations.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using UserRankingSystem.Models;

namespace UserRankingSystem.Controllers {
    public class UserController: ControllerBase {
        private UserReadingContext _context;

        public UserController(UserReadingContext context) { _context = context; }

        /// <description>
        /// Handle POST requests. Only accepts new requests with different emails from the database.
        /// </description>
        public async Task<ActionResult<User>> PostUser(User user) {
            // Only allow creation of new user with a different email from database.
            if (_context.Users.Any(u => u.Email == User.email)) {
                return BadRequest("Email already exists.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // Return 201 created response
            return CreateAtAction(nameof(GetUser), new {id = user.Id}, user);
        }

        /// <description>
        /// Handle GET requests. Optional parameters to handle GET requests to filter scores given a minimum score
        /// and to sort scores in descending order.
        /// </description>
        public async Task<ActionResult<IEnumerable<User>>> GetUser([FromQuery] int? minScore, [FromQuery] bool sort = false) {
            var users = _context.Users.AsQueryable();

            // minScore initialised as nullable; if it is not, filter values.
            if (minScore.HasValue) {
                users = users.Where(u => u.Score >= minScore.Value);
            }

            if (sort) {
                users = users.OrderByDescending(u => u.Score);
            }

            return await users.ToListAsync();
        }

        /// <description>
        /// Handle GET requests for a specified user.
        /// </description>
        public async Task<ActionResult<User>> GetUser(int id) {
            var user = await _context.Users.FindAsync(id);

            // Return 404 user not found.
            if (user == null) {
                return NotFound();
            }

            return user;
        }

        /// <description>
        /// Handle DELETE requests for a specified user.
        /// </description>
        public async Task<IActionResult> DeleteUser(int id) {
            var user = await _context.Users.FindAsync(id);

            // Error 404 user not found.
            if (user == null) {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            // 204 No Content response
            return NoContent();
        }

        /// <description>
        /// Handle GET requests for a ranked list of users based on their scores.
        /// </description>
        public async Task<ActionResult<IEnumerable<User>>> GetRankedUsers() {
            return await _context.Users.OrderByDescending(u => u.Score).ToListAsync();
        }

        /// <description>
        /// Handle GET requests for a specified user's rank.
        /// </description>
        public async Task<ActionResult<int>> GetUserRank(int id) {
            // Retrieves the list of all users, ordered by score (highest first).
            var rankedUsers = await _context.Users
                .OrderByDescending(u => u.Score)
                .ToListAsync();
            
            var user = rankedUsers.FirstOrDefault(u => u.Id == id);
            
            // Error 404 User Not Found.
            if (user == null)  
                return NotFound();

            // Gets rank (1-based index) of the user. +1 since index is 0-based.
            var rank = rankedUsers.IndexOf(user) + 1;

            // Return 200 OK response.
            return Ok(rank);
        }
    }

}