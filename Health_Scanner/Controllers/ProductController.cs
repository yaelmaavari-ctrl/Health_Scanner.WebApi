
//using Azure.Core;
//using Microsoft.AspNetCore.Mvc;
//using Service.Exceptions;
//using Service.Interfaces;

//[ApiController]
//[Route("api/[controller]")]
//public class ProductController : ControllerBase
//{
//    private readonly IProductService _productService;
//    private readonly IUserAllergenService _userAllergenService; // משתמשים ב-Service ולא ב-Repo

//    public ProductController(IProductService productService, IUserAllergenService userAllergenService)
//    {
//        _productService = productService;
//        _userAllergenService = userAllergenService;
//    }

//    [HttpGet("scan/{barcode}")]
//    public async Task<IActionResult> ScanProduct(
//    string barcode,
//    [FromHeader(Name = "X-User-Id")] int userId)
//    {
//        try
//        {
//            // 2. שימוש ב-ID כדי לשלוף את האלרגיות הספציפיות שלו מהדאטה-בייס
//            var forbiddenAllergens = await _userAllergenService.GetUserAllergens(userId);

//            // 3. המרת רשימת האובייקטים לרשימת שמות (Strings) כדי שה-ProductService יבין אותם
//            var allergenNames = forbiddenAllergens.Select(a => a.Name).ToList();

//            // 4. שליחת הברקוד ורשימת השמות האסורה למנוע הסריקה
//            var result = await _productService.ScanAndProcessProduct(barcode, allergenNames, userId);

//            return Ok(result);
//        }
//        catch (NotFoundException ex)
//        {
//            return NotFound(new { message = ex.Message });
//        }
//        catch (UnsupportedLanguageException ex)
//        {
//            return BadRequest(new { message = ex.Message });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new { message = "Internal error", details = ex.Message });
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Service.Exceptions;
using Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Authorize] 
[ApiController]
[Route("api/products")]
public class ProductController(IProductService productService, IUserAllergenService userAllergenService) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly IUserAllergenService _userAllergenService = userAllergenService;

    private int GetUserIdFromToken()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new UnauthorizedAccessException("User identification missing in token.");
        return int.Parse(claim);
    }

    [HttpGet("scan/{barcode}")]
    public async Task<IActionResult> ScanProduct(string barcode)
    {
        try
        {
            int userId = GetUserIdFromToken();

            var forbiddenAllergens = await _userAllergenService.GetUserAllergens(userId);

            var allergenNames = forbiddenAllergens.Select(a => a.Name).ToList();

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