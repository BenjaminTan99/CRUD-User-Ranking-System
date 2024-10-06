/// <summary>
/// Class UserController deals with RESTful API calls with regards to CRUD operations.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserRankingSystem.Data;
using UserRankingSystem.Models;

namespace UserRankingSystem.Controllers {

    [Route("api/users")]
    [ApiController]
    public class UserController: ControllerBase {
        private readonly UserRankingContext _context;

        public UserController(UserRankingContext context) { 
            _context = context;
            Console.WriteLine("User Controller Initialised.");
        }

        /// <description>
        /// Handle POST requests. Only accepts new requests with different emails from the database.
        /// </description>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user) {
            // 400 Bad Request if validation fails.
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            // Only allow creation of new user with a different email from database.
            if (_context.Users.Any(u => u.Email == user.Email)) {
                return BadRequest("Email already exists.");
            }

            if (user.Score <= 0) {
                return BadRequest("Score must be a positive integer.");
            }

            Console.WriteLine("Successful user addition.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // Return 201 created response
            return CreatedAtAction(nameof(GetUser), new {id = user.Id}, user);
        }

        /// <description>
        /// Handles UPDATE requests. Ensures that only the user with specified id is updated.
        /// </description>
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser) {
            var existingUser = await _context.Users.FindAsync(id);
            // 404 User Not Found
            if (existingUser == null) {
                return NotFound();
            }

            // 400 Bad Request if email already exists for a different user
            if (_context.Users.Any(u => u.Email == updatedUser.Email)) {
                return BadRequest("Email already exists.");
            }

            // 400 Bad Request if new score is 0 or below.
            if (updatedUser.Score <= 0) {
                return BadRequest("Score must be a positive integer.");
            }

            // Update user details
            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;
            existingUser.Score = updatedUser.Score;

            await _context.SaveChangesAsync();

            Console.WriteLine("User successfully updated.");
            // 204 No Content Response
            return NoContent();
        }

        /// <description>
        /// Handle GET requests. Optional parameters to handle GET requests to filter scores given a minimum score
        /// and to sort scores in descending order.
        /// </description>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] int? minScore, [FromQuery] bool sort = false) {
            var users = _context.Users.AsQueryable();

            // minScore initialised as nullable; if it is not, filter values.
            if (minScore.HasValue) {
                users = users.Where(u => u.Score >= minScore.Value);
            }

            if (sort) {
                users = users.OrderByDescending(u => u.Score);
            } else {
                users = users.OrderBy(u=> u.Score);
            }

            Console.WriteLine("Users' scores successfully obtained.");
            return Ok(await users.ToListAsync());
        }

        /// <description>
        /// Handle GET requests for a specified user.
        /// </description>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id) {
            var user = await _context.Users.FindAsync(id);

            // Return 404 user not found.
            if (user == null) {
                return NotFound();
            }

            Console.WriteLine("User's score successfully obtained.");
            return Ok(user);
        }

        /// <description>
        /// Handle DELETE requests for a specified user.
        /// </description>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id) {
            var user = await _context.Users.FindAsync(id);

            // Error 404 user not found.
            if (user == null) {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            Console.WriteLine("User successfully deleted.");
            // 204 No Content response
            return NoContent();
        }

        /// <description>
        /// Handle GET requests for a ranked list of users based on their scores.
        /// </description>
        [HttpGet("rank")]
        public async Task<ActionResult<IEnumerable<User>>> GetRankedUsers() {
            return Ok(await _context.Users.OrderByDescending(u => u.Score).ToListAsync());
        }

        /// <description>
        /// Handle GET requests for a specified user's rank.
        /// </description>
        [HttpGet("rank/{id}")]
        public async Task<ActionResult<int>> GetUserRank(int id) {
            // Retrieves the list of all users, ordered by score (highest first).
            var rankedUsers = await _context.Users.OrderByDescending(u => u.Score).ToListAsync();
            
            var user = rankedUsers.FirstOrDefault(u => u.Id == id);
            
            // Error 404 User Not Found.
            if (user == null) {
                return NotFound();
            }

            // Gets rank (1-based index) of the user. +1 since index is 0-based.
            var rank = rankedUsers.IndexOf(user) + 1;

            Console.WriteLine("User's rank successfully obtained.");
            // Return 200 OK response.
            return Ok(rank);
        }
    }

}