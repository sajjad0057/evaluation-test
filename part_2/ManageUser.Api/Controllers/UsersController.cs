using ManageUser.Api.Utils;
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
    [ProducesResponseType(typeof(ApiResults<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResults<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(UserDto userDto)
    {
        try
        {
            await _userService.CreateUserAsync(userDto);
            return Ok(ApiResults<string>.Success(StatusCodes.Status200OK, "user created"));
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ApiResults<string>.Fail(StatusCodes.Status400BadRequest, ex.Message));
        }
    }

    [HttpPost("create-bulk-users")]
    [ProducesResponseType(typeof(ApiResults<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResults<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBulkUsers()
    {
        try
        {
            await _userService.CreateBulkUsersAsync(10000);
            return Ok(ApiResults<string>.Success(StatusCodes.Status200OK, "10,000 users created"));

        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ApiResults<string>.Fail(StatusCodes.Status400BadRequest, ex.Message));
        }
    }

    [HttpGet("fetch-users")]
    [ProducesResponseType(typeof(ApiResults<List<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResults<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FetchUsers()
    {
        try
        {
            var users = await _userService.FetchUsersAsync();

            if(users == null || !users.Any())
                return BadRequest(ApiResults<string>.Fail(StatusCodes.Status400BadRequest, "No users found!"));

            return Ok(ApiResults<List<UserDto>>.Success(StatusCodes.Status200OK, users));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(ApiResults<string>.Fail(StatusCodes.Status400BadRequest, ex.Message));
        }
    }
}
