using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Service.Exceptions;
using Service.Interfaces;
using Service.Dto;
using Service;


namespace Health_Scanner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService authService = authService;

        // POST: api/<AuthController>/login

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await authService.Login(login);
            var token = authService.GenerateToken(user);

            return Ok(new
            {
                User = user,
                Token = token
            });
        }


        // POST: api/<AuthController>/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto register)
        {
            // 1. יצירת המשתמש החדש במסד הנתונים
            var newUser = await authService.Register(register);
            var token = authService.GenerateToken(newUser);
            return CreatedAtAction("GetMyProfile", "User", null, new
            {
                User = newUser,
                Token = token
            });
        }

        // DELETE: api/<AuthController>/delete
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdClaim = (User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
                              ?? throw new UnauthorizedException("Invalid token");

            int userId = int.Parse(userIdClaim);

            await authService.DeleteUser(userId);

            return Ok(new { Message = "User account deleted successfully" });
        }

    }
}