using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Service;
using Service.Dto;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Health_Scanner.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController(IUserService service, IConfiguration configuration) : ControllerBase
    {
        private readonly IUserService _service = service;
        private readonly IConfiguration _configuration = configuration;



        //// GET: api/user/{id}
        //[Authorize]
        //[HttpGet("{id}")]
        //public async Task<ActionResult<UserDto>> Get(int id)
        //{
        //    var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //    if (userIdFromToken == null || int.Parse(userIdFromToken) != id)
        //    {
        //        return StatusCode(403, new { message = "You are not authorized to view this profile." });
        //    }
        //    var result = await _service.GetById(id);
        //    return Ok(result);
        //}



        //[Authorize]
        //[HttpGet("email/{email}")]
        //public async Task<ActionResult<UserDto>> GetByEmail(string email)
        //{
        //    var userEmailFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        //    if (userEmailFromToken == null || userEmailFromToken != email)
        //    {
        //        return StatusCode(403, new { message = "You are not authorized to view this profile." });
        //    }
        //    var result = await _service.GetByEmail(email);
        //    return Ok(result);
        //}

        [Authorize]
        [HttpGet("my-profile")]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { message = "Token is missing user identification." });
            }

            int userId = int.Parse(userIdFromToken);

            var result = await _service.GetById(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("my-details-by-email")]
        public async Task<ActionResult<UserDto>> GetMyProfileByEmail()
        {
            var userEmailFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmailFromToken))
            {
                return Unauthorized(new { message = "Email claim missing in token." });
            }
            var result = await _service.GetByEmail(userEmailFromToken);
            return Ok(result);
        }






    }
}

