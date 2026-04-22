using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Dto;

namespace Health_Scanner.Controllers
{
    [Route("api/users/{userId}/allergens")]
    [ApiController]
    public class UserAllergensController(IUserAllergenService userAllergenService) : ControllerBase
    {
        private readonly IUserAllergenService _service = userAllergenService;
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllergenDto>>> GetUserAllergens(int userId)
        {
            var allergens = await _service.GetUserAllergens(userId);
            return Ok(allergens);
        }

        [HttpPost]
        public async Task<ActionResult<AllergenDto>> AddAllergenToUser(int userId, [FromQuery] int allergenId)
        {
            var result = await _service.AddAllergenToUser(userId, allergenId);
            return Ok(result);
        }

        [HttpDelete("{allergenId}")]
        public async Task<IActionResult> RemoveAllergenFromUser(int userId, int allergenId)
        {
            await _service.RemoveAllergenFromUser(userId, allergenId);
            return NoContent(); // מחזירים 204 No Content כי המחיקה הצליחה ואין מידע להחזיר
        }
    }
}