using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PriceNegotiationAPI.Services;

namespace PriceNegotiationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogInAsync([FromBody] LoginRequest login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                return BadRequest("You are already logged in!");

            var result = await _userService.LoginAsync(login, useCookies, useSessionCookies);

            if (!result.Succeeded)
            {
                return Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
            }

            return Empty;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOutAsync()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                await _userService.LogOutAsync();
                return Ok();
            }

            return BadRequest("You are not logged in!");
        }
    }
}
