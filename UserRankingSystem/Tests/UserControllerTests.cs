using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;
using UserRankingSystem.Controllers;
using UserRankingSystem.Data;
using UserRankingSystem.Models;
using Xunit;

/// <summary>
/// Class UserControllerTests deals with testing the user controller.
/// </summary>
public class UserControllerTests {
    private readonly UserController _controller;
    private readonly UserRankingContext _context;

    public UserControllerTests() {
        var options = new DbContextOptionsBuilder<UserRankingContext>()
            .UseInMemoryDatabase(databaseName: "TestDB").Options;
        _context = new UserRankingContext(options);
        _controller = new UserController(_context);
    }

    public void PostUser_ReturnsCreatedUser() {
        // Arrange
        var user = new User { Name = "Test User", Email = "test@example.com", Score = 100 };

        // Act
        var result = _controller.PostUser(user);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result.Result);
    }
}