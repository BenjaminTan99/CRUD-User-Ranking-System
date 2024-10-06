using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UserRankingSystem.Controllers;
using UserRankingSystem.Data;
using UserRankingSystem.Models;
using Xunit;

/// <summary>
/// Class UserControllerTests deals with testing the user controller.
/// </summary>
public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly UserRankingContext _context;

    public UserControllerTests() {
        var options = new DbContextOptionsBuilder<UserRankingContext>()
            .UseInMemoryDatabase(databaseName: "TestDB").Options;
        _context = new UserRankingContext(options);
        _controller = new UserController(_context);
    }

    // ------- POST TESTING --------------------------------
    [Fact]
    public async Task PostUser_ReturnsCreatedUser() {
        var user = new User { Name = "Test User", Email = "test@example.com", Score = 100 };

        var result = await _controller.PostUser(user);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdUser = Assert.IsType<User>(createdResult.Value);

        Assert.Equal("Test User", createdUser.Name);
        Assert.Equal("test@example.com", createdUser.Email);
        Assert.Equal(100, createdUser.Score);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task PostUser_PreventsDuplicateUser() {
        var user1 = new User { Name = "Test User", Email = "test@example.com", Score = 100 };
        _context.Users.Add(user1);
        await _context.SaveChangesAsync();

        var user2 = new User { Name = "Test User 2", Email = "test@example.com", Score = 200 };

        var result = await _controller.PostUser(user2);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Email already exists.", badRequestResult.Value);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task PostUser_PreventsNonPositiveScore() {
        var user1 = new User { Name = "Test User", Email = "test@example.com", Score = 0 };
        var result = await _controller.PostUser(user1);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Score must be a positive integer.", badRequestResult.Value);

        var user2 = new User { Name = "Test User 2", Email = "test@example.com", Score = -10 };

        result = await _controller.PostUser(user2);
        badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Score must be a positive integer.", badRequestResult.Value);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    // ------------UPDATE TESTING---------------------------------------
    [Fact]
    public async Task UpdateUser_ShouldModifyExistingUser() {
        var existingUser = new User { Name = "Test User", Email = "test@example.com", Score = 100 };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var updatedUser = new User { Id = existingUser.Id, Name = "Updated User", Email = "updated@example.com", Score = 150 };
        
        var result = await _controller.UpdateUser(existingUser.Id, updatedUser);

        Assert.IsType<NoContentResult>(result);

        // Verify the update in the database
        var userInDb = await _context.Users.FindAsync(existingUser.Id);
        Assert.Equal("Updated User", userInDb.Name);
        Assert.Equal("updated@example.com", userInDb.Email);
        Assert.Equal(150, userInDb.Score);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task UpdateUser_ShouldNotModifyExistingUser_WhenUserDoesNotExist() {
        var nonExistentUserId = 999;
        var updatedUser = new User { Id = nonExistentUserId, Name = "NonExistent", Email = "nonexistent@example.com", Score = 200 };

        var result = await _controller.UpdateUser(nonExistentUserId, updatedUser);
        Assert.IsType<NotFoundResult>(result);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task UpdateUser_ShouldNotModifyExistingUser_WhenDuplicateEmail() {
        var existingUser = new User { Name = "Test User", Email = "test@example.com", Score = 100 };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var updatedUser = new User { Name = "Updated User", Email = "test@example.com", Score = 150 };
        
        var result = await _controller.UpdateUser(existingUser.Id, updatedUser);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Email already exists.", badRequestResult.Value);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task UpdateUser_ShouldNotModifyExistingUser_WhenScoreZeroOrBelow() {
        var existingUser = new User { Name = "Test User", Email = "test@example.com", Score = 100 };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var updatedUser = new User { Id = existingUser.Id, Name = "Updated User", Email = "test2@example.com", Score = 0 };
        
        var result = await _controller.UpdateUser(existingUser.Id, updatedUser);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Score must be a positive integer.", badRequestResult.Value);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    // ------------DELETE TESTING---------------------------------------
    [Fact]
    public async Task DeleteUser_ShouldRemoveUser() {
        var existingUser = new User { Name = "Test User", Email = "test@example.com", Score = 100 };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var result = await _controller.DeleteUser(existingUser.Id);
        Assert.IsType<NoContentResult>(result);

        // Verify the user is deleted
        var userInDb = await _context.Users.FindAsync(existingUser.Id);
        Assert.Null(userInDb);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task DeleteUser_ShouldNotRemoveUser_WhenInvalidID() {
        var testId = 99;

        var result = await _controller.DeleteUser(testId);
        Assert.IsType<NotFoundResult>(result);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    // ------------GET TESTING (UNSPECIFIED)------------------------------------
    [Fact]
    public async Task GetUsers_WithoutFilter_ShouldGiveOrderItWasAdded() {
        _context.Users.AddRange(
            new User { Name = "User1", Email = "user1@example.com", Score = 100 },
            new User { Name = "User2", Email = "user2@example.com", Score = 150 }
        );
        await _context.SaveChangesAsync();

        var result = await _controller.GetUsers(minScore: null, sort: false);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);

        Assert.Equal(2, users.Count);
        Assert.Equal(100, users.First().Score);  // User with 100 score should be first
        Assert.Equal(150, users.Last().Score);   // User with 150 score should be second

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUsers_ShouldFilterByMinScore() {
        _context.Users.AddRange(
            new User { Name = "User1", Email = "user1@example.com", Score = 100 },
            new User { Name = "User2", Email = "user2@example.com", Score = 150 }
        );
        await _context.SaveChangesAsync();

        var result = await _controller.GetUsers(minScore: 120, sort: false);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);

        Assert.Single(users); // Should only have 1 score above 120.
        Assert.Equal(150, users.First().Score);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUsers_ShouldSortByScoreDescending() {
        _context.Users.AddRange(
            new User { Name = "User1", Email = "user1@example.com", Score = 100 },
            new User { Name = "User2", Email = "user2@example.com", Score = 150 }
        );
        await _context.SaveChangesAsync();

        var result = await _controller.GetUsers(minScore: null, sort: true);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);

        Assert.Equal(2, users.Count);
        Assert.Equal(150, users.First().Score);  // Highest score should be first
        Assert.Equal(100, users.Last().Score);   // Lowest score should be last

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnEmptyList_WhenNoUsersExist() {
        var result = await _controller.GetUsers(null, false);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);
        Assert.Empty(users);  // Ensure that no users are returned

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUsers_ShouldFilterAndSortInDescending() {
        _context.Users.AddRange(
            new User { Name = "User1", Email = "user1@example.com", Score = 100 },
            new User { Name = "User2", Email = "user2@example.com", Score = 150 },
            new User { Name = "User3", Email = "user3@example.com", Score = 80 }
        );
        await _context.SaveChangesAsync();

        var result = await _controller.GetUsers(minScore: 100, sort: true);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);

        Assert.Equal(2, users.Count); // User with 80 score should have been filtered out.
        Assert.Equal(150, users.First().Score);  // Highest score should be first
        Assert.Equal(100, users.Last().Score);   // Lowest score should be last

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnEmptyList_WhenNoUsersExist_WithSortingAndFiltering(){
        var result = await _controller.GetUsers(minScore: 100, sort: true);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);
        Assert.Empty(users);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    // ------------GET TESTING (SPECIFIED)------------------------------------
    [Fact]
    public async Task GetUser_ShouldReturnCorrectlyIfValidID() {
        _context.Users.AddRange(
            new User { Name = "User1", Email = "user1@example.com", Score = 100 },
            new User { Name = "User2", Email = "user2@example.com", Score = 150 }
        );
        await _context.SaveChangesAsync();

        var result = await _controller.GetUser(1);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var user = Assert.IsType<User>(okResult.Value);

        Assert.Equal("User1", user.Name);
        Assert.Equal("user1@example.com", user.Email);
        Assert.Equal(100, user.Score);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist() {
        var nonExistentUserId = 999;  // Assumed non-existent ID

        var result = await _controller.GetUser(nonExistentUserId);

        Assert.IsType<NotFoundResult>(result.Result);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnCorrectUsers_WhenMinScoreIsAtBoundary() {
        _context.Users.AddRange(
            new User { Name = "User1", Email = "user1@example.com", Score = 100 },
            new User { Name = "User2", Email = "user2@example.com", Score = 120 }
        );
        await _context.SaveChangesAsync();

        var result = await _controller.GetUsers(minScore: 100, sort: false);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);

        // Verify that both users are returned (since both have a score >= 100)
        Assert.Equal(2, users.Count);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    // ------------GET TESTING BY RANKED (UNSPECIFIED)------------------------------------
    [Fact]
    public async Task GetRankedUsers_ShouldReturnUsersSortedByScore() {
        _context.Users.AddRange(
            new User { Name = "User1", Email = "user1@example.com", Score = 100 },
            new User { Name = "User2", Email = "user2@example.com", Score = 150 }
        );
        await _context.SaveChangesAsync();

        var result = await _controller.GetRankedUsers();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);

        Assert.Equal(2, users.Count);
        Assert.Equal(150, users.First().Score);  // Highest score first
        Assert.Equal(100, users.Last().Score);   // Lowest score last

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetRankedUsers_ShouldReturnEmptyIfNoUsers() {
        var result = await _controller.GetRankedUsers();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsType<List<User>>(okResult.Value);
        Assert.Empty(users);  // Ensure that no users are returned

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    // ------------GET TESTING BY RANKED (SPECIFIED)------------------------------------
    [Fact]
    public async Task GetUserRank_ShouldReturnCorrectRank() {
        _context.Users.AddRange(
            new User { Name = "User1", Email = "user1@example.com", Score = 100 },
            new User { Name = "User2", Email = "user2@example.com", Score = 150 }
        );
        await _context.SaveChangesAsync();
        // Use User1's ID.
        var userId = _context.Users.First(u => u.Score == 100).Id;

        var result = await _controller.GetUserRank(userId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var rank = Assert.IsType<int>(okResult.Value);

        Assert.Equal(2, rank);  // "User1" should have rank 2

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUserRank_ShouldReturnNotFound_WhenUserDoesNotExist() {
        var nonExistentUserId = 999;

        var result = await _controller.GetUserRank(nonExistentUserId);

        Assert.IsType<NotFoundResult>(result.Result);

        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
    }
}