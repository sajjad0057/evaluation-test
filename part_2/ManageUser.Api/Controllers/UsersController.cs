using ManageUser.Infrastructure.DTOs;
using ManageUser.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManageUser.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUsersService userService, ILogger<UsersController> logger) : ControllerBase
{
    private readonly IUsersService _userService = userService;
    private readonly ILogger<UsersController> _logger = logger;

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser(UserDto userDto)
    {
        try
        {
            await _userService.CreateUserAsync(userDto);
            return Ok("User created");
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("create-bulk-users")]
    public async Task<IActionResult> CreateBulkUsers()
    {
        try
        {
            await _userService.CreateBulkUsersAsync(10000);
            return Ok("10,000 users created");

        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("fetch-users")]
    public async Task<IActionResult> FetchUsers()
    {
        try
        {
            var users = await _userService.FetchUsersAsync();
            return Ok(users);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
