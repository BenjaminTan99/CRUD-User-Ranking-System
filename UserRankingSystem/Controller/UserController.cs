/// <summary>
/// Class UserController deals with RESTful API calls with regards to CRUD operations.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using UserRankingSystem.Models;

namespace UserRankingSystem.Controllers {
    public class UserController: ControllerBase {
        private UserReadingContext _context;
    }
}