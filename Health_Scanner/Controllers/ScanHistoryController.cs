using Microsoft.AspNetCore.Mvc;
using Repository.Entities;
using Repository.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class ScanHistoryController : ControllerBase
{
    private readonly IScanHistoryRepository _historyRepo;

    public ScanHistoryController(IScanHistoryRepository historyRepo)
    {
        _historyRepo = historyRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserHistory([FromHeader(Name = "X-User-Id")] int userId)
    {
        // שליפת 20 הסריקות האחרונות
        var history = await _historyRepo.GetUserHistory(userId, limit: 20);

        if (history == null || !history.Any())
        {
            return Ok(new List<ScanHistory>()); // עדיף להחזיר רשימה ריקה מאשר 404
        }

        return Ok(history);
    }

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteHistoryItem(int id, [FromHeader(Name = "X-User-Id")] int userId)
    //{
    //    // כאן תוכלי להוסיף בעתיד לוגיקה למחיקת פריט ספציפי מההיסטוריה
    //    return NoContent();
    //}
}