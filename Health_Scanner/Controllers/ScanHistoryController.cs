//using Microsoft.AspNetCore.Mvc;
//using Repository.Entities;
//using Repository.Interfaces;

//[Route("api/[controller]")]
//[ApiController]
//public class ScanHistoryController : ControllerBase
//{
//    private readonly IScanHistoryRepository _historyRepo;

//    public ScanHistoryController(IScanHistoryRepository historyRepo)
//    {
//        _historyRepo = historyRepo;
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetUserHistory([FromHeader(Name = "X-User-Id")] int userId)
//    {
//        // שליפת 20 הסריקות האחרונות
//        var history = await _historyRepo.GetUserHistory(userId, limit: 20);

//        if (history == null || !history.Any())
//        {
//            return Ok(new List<ScanHistory>()); // עדיף להחזיר רשימה ריקה מאשר 404
//        }

//        return Ok(history);
//    }

//    //[HttpDelete("{id}")]
//    //public async Task<IActionResult> DeleteHistoryItem(int id, [FromHeader(Name = "X-User-Id")] int userId)
//    //{
//    //    // כאן תוכלי להוסיף בעתיד לוגיקה למחיקת פריט ספציפי מההיסטוריה
//    //    return NoContent();
//    //}
//}



using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Health_Scanner.Controllers
{
    [Authorize] 
    [Route("api/my-scan-history")] 
    [ApiController]
    public class ScanHistoryController(IScanHistoryRepository historyRepo) : ControllerBase
    {
        private readonly IScanHistoryRepository _historyRepo = historyRepo;
        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? throw new UnauthorizedAccessException("User identification missing in token.");
            return int.Parse(claim);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserHistory()
        {
            // חילוץ המזהה מהטוקן במקום מה-Header הידני
            int userId = GetUserIdFromToken();

            // שליפת 20 הסריקות האחרונות של המשתמש הספציפי הזה
            var history = await _historyRepo.GetUserHistory(userId, limit: 20);

            if (history == null || !history.Any())
            {
                return Ok(new List<ScanHistory>());
            }

            return Ok(history);
        }
    }
}