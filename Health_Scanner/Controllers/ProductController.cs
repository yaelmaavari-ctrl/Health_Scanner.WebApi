//using Microsoft.AspNetCore.Mvc;
//using Repository.Entities;
//using Repository.Interfaces;
//using Service.Dto;
//using Service.Exceptions;
//using Service.Interfaces;
//using Service.Services;

//namespace Health_Scanner.WebApi.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class ProductController : ControllerBase
//    {
//        private readonly IProductService _productService;
//        private readonly IUserAllergenService _userAllergenService;
//        private readonly IUserAllergenRepository _userAllergenRepo; // הזרקת הריפו הייעודי

//        public ProductController(IProductService productService, IUserAllergenRepository userAllergenRepo)
//        {
//            _productService = productService;
//            _userAllergenRepo = userAllergenRepo;
//        }

//        [HttpGet("scan/{barcode}")]
//        public async Task<IActionResult> ScanProduct(string barcode)
//        {
//            // 1. שליפת ה-ID מה-Header (לפי המימוש הנוכחי שלך)
//            if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) ||
//                !int.TryParse(userIdHeader, out int userId))
//            {
//                return BadRequest(new { message = "Valid X-User-Id header is required." });
//            }

//            try
//            {
//                // 2. שליפת רשימת שמות האלרגנים של המשתמש דרך ה-Service שלו
//                // הערה: וודאי שב-IUserAllergenService יש מתודה שמחזירה List<string> של שמות
//                var forbiddenNames = await _userAllergenService.GetUserAllergens(userId);

//                // 3. הרצת תהליך הסריקה המלא (כולל זיהוי מוצר וחיפוש תחליף אם צריך)
//                // הפונקציה הזו מחזירה ScanResultDto שמכיל את המוצר הסרוק ואולי תחליף
//                //var scanResult = await _productService.ScanAndProcessProduct(barcode, forbiddenNames);
//                var allergenNames = forbiddenNames.Select(a => a.Name).ToList();
//                var result = await _productService.ScanAndProcessProduct(barcode, allergenNames);
//                return Ok(result);
//            }
//            catch (NotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (UnsupportedLanguageException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (ExternalServiceException ex)
//            {
//                return StatusCode(503, new { message = "Data source error", details = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Internal error", details = ex.Message });
//            }
//        }
//    }
//}




using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Service.Exceptions;
using Service.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IUserAllergenService _userAllergenService; // משתמשים ב-Service ולא ב-Repo

    public ProductController(IProductService productService, IUserAllergenService userAllergenService)
    {
        _productService = productService;
        _userAllergenService = userAllergenService;
    }

    [HttpGet("scan/{barcode}")]
    public async Task<IActionResult> ScanProduct(
    string barcode,
    [FromHeader(Name = "X-User-Id")] int userId)
    {
        try
        {
            // 2. שימוש ב-ID כדי לשלוף את האלרגיות הספציפיות שלו מהדאטה-בייס
            var forbiddenAllergens = await _userAllergenService.GetUserAllergens(userId);

            // 3. המרת רשימת האובייקטים לרשימת שמות (Strings) כדי שה-ProductService יבין אותם
            var allergenNames = forbiddenAllergens.Select(a => a.Name).ToList();

            // 4. שליחת הברקוד ורשימת השמות האסורה למנוע הסריקה
            var result = await _productService.ScanAndProcessProduct(barcode, allergenNames, userId);

            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnsupportedLanguageException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal error", details = ex.Message });
        }
    }
}