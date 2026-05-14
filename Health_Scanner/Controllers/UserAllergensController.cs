using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Dto;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Health_Scanner.Controllers
{
    [Authorize] // הגנה על כל הפונקציות בקונטרולר
    [Route("api/my-allergens")] // נתיב נקי ללא ID
    [ApiController]
    public class UserAllergensController(IUserAllergenService userAllergenService) : ControllerBase
    {
        private readonly IUserAllergenService _service = userAllergenService;

        // פונקציית עזר פרטית לחילוץ ה-ID מהטוקן
        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? throw new UnauthorizedAccessException("User identification missing in token.");
            return int.Parse(claim);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllergenDto>>> GetUserAllergens()
        {
            int userId = GetUserIdFromToken();
            var allergens = await _service.GetUserAllergens(userId);
            return Ok(allergens);
        }

        [HttpPost("{allergenId}")] // שיניתי מ-FromQuery לתוך הנתיב לצורך נוחות
        public async Task<ActionResult<AllergenDto>> AddAllergenToUser(int allergenId)
        {
            int userId = GetUserIdFromToken();
            var result = await _service.AddAllergenToUser(userId, allergenId);

            return CreatedAtAction("Get", "Allergen", new { id = result.Id }, result);

        }


        [HttpDelete("{allergenId}")]
        public async Task<IActionResult> RemoveAllergenFromUser(int allergenId)
        {
            int userId = GetUserIdFromToken();
            await _service.RemoveAllergenFromUser(userId, allergenId);
            return NoContent();
        }
    }
}