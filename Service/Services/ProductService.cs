//using LanguageDetection;
//using Service.Dto;
//using Service.Exceptions;
//using Service.Interfaces;
//using System.Text.Json;

//namespace Service.Services
//{
//    public class ProductService : IProductService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly LanguageDetector _detector;
//        private readonly string _baseUrl = "https://world.openfoodfacts.org/api/v2/product/";

//        public ProductService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//            _detector = new LanguageDetector();

//            // טעינת שפות נבחרות לשיפור דיוק וביצועים (אנגלית כשפת יעד, והשאר כדי לזהות זליגות)
//            //_detector.AddLanguages("en", "he", "fr", "es", "de");
//            _detector.AddAllLanguages();
//        }

//        public async Task<ProductResponseDto> GetProductByBarcode(string barcode, List<string> forbiddenAllergens)
//        {
//            var fields = "product_name,product_name_en,ingredients_text_en,allergens_tags,nutriscore_grade,image_url";
//            var url = $"{_baseUrl}{barcode}.json?lc=en&fields={fields}";

//            try
//            {
//                var response = await _httpClient.GetAsync(url);
//                if (!response.IsSuccessStatusCode)
//                {
//                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
//                        throw new NotFoundException(barcode);
//                    throw new ExternalServiceException($"API Error: {response.StatusCode}");
//                }

//                var jsonString = await response.Content.ReadAsStringAsync();

//                using (JsonDocument doc = JsonDocument.Parse(jsonString))
//                {
//                    if (!doc.RootElement.TryGetProperty("product", out JsonElement productElement))
//                        throw new NotFoundException(barcode);

//                    // 1. חילוץ נתונים בסיסי לבדיקת שפה
//                    string ingredients = productElement.TryGetProperty("ingredients_text_en", out var ingEn)
//                        ? ingEn.GetString() ?? ""
//                        : "";

//                    string productName = productElement.TryGetProperty("product_name_en", out var nameEn) && !string.IsNullOrEmpty(nameEn.GetString())
//                        ? nameEn.GetString()!
//                        : (productElement.TryGetProperty("product_name", out var name) ? name.GetString() : "Unknown")!;

//                    var allergens = ProcessAllergens(productElement);

//                    // 2. שלב אימות השפה המאוחד (Comprehensive Safety Check)
//                    // מאחדים את כל הטקסטים כדי למנוע מצב שבו שדה אחד באנגלית והשאר בשפה זרה
//                    string textToVerify = $"{productName} {ingredients} {string.Join(" ", allergens)}".Trim();

//                    if (string.IsNullOrWhiteSpace(ingredients) || textToVerify.Length < 5)
//                    {
//                        throw new UnsupportedLanguageException("Safety Block: Essential product data is missing or too short to verify.");
//                    }
//                    // 2. בדיקת חסימה לעברית (אלגוריתם "אפס סובלנות")
//                    // אם יש תו אחד בטווח של עברית (0x0590-0x05FF), אנחנו פוסלים מיד
//                    if (textToVerify.Any(c => c >= 0x0590 && c <= 0x05FF))
//                    {
//                        throw new UnsupportedLanguageException("Safety Block: Hebrew characters detected. English-only data is required for safety.");
//                    }

//                    string detectedLanguage = _detector.Detect(textToVerify);

//                    // חסימה קריטית: אם השפה שזוהתה אינה אנגלית מובהקת
//                    if (detectedLanguage != "en")
//                    {
//                        throw new UnsupportedLanguageException($"Safety Block: Detected language is '{detectedLanguage}'. Only English is supported for allergen safety.");
//                    }


//                    // 3. בניית אובייקט התשובה לאחר אימות בטיחות
//                    return new ProductResponseDto
//                    {
//                        Barcode = barcode,
//                        ProductName = productName, 
//                        IngredientsText = ingredients,
//                        Allergens = allergens,
//                        NutriscoreGrade = productElement.TryGetProperty("nutriscore_grade", out var score)
//                            ? score.GetString()?.ToLower() ?? "n/a"
//                            : "n/a",
//                        ImageUrl = productElement.TryGetProperty("image_url", out var img)
//                            ? img.GetString() ?? ""
//                            : "",
//                        IsSafeForUser = !forbiddenAllergens.Any(allergy =>
//                        textToVerify.ToLower().Contains(allergy.ToLower().Trim()))
//                    };
//                }
//            }
//            catch (Exception ex) when (ex is not UnsupportedLanguageException && ex is not NotFoundException)
//            {
//                throw new ExternalServiceException("An unexpected error occurred during product processing.", ex);
//            }
//        }

//        private List<string> ProcessAllergens(JsonElement productElement)
//        {
//            if (productElement.TryGetProperty("allergens_tags", out var tagsElem) && tagsElem.ValueKind == JsonValueKind.Array)
//            {
//                return tagsElem.EnumerateArray()
//                    .Select(t => t.GetString())
//                    .Where(t => t != null && t.StartsWith("en:"))
//                    .Select(t => t!.Replace("en:", ""))
//                    .ToList();
//            }
//            return new List<string>();
//        }

//        //private List<string> GetProductCategory(string barcode)
//        //{

//        //}
//    }
//}



using LanguageDetection;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Interfaces;
using Service.Dto;
using Service.Exceptions;
using Service.Interfaces;
using System.Text.Json;

public class ProductService: IProductService
{
    private readonly HttpClient _httpClient;
    private readonly LanguageDetector _detector;
    private readonly string _baseUrl = "https://world.openfoodfacts.org/api/v2/product/";
    private readonly ILogger<ProductService> _logger;
    private readonly IScanHistoryRepository _scanHistoryRepository;
    public ProductService(HttpClient httpClient, ILogger<ProductService> logger, IScanHistoryRepository scanHistoryRepository)
    {
        _httpClient = httpClient;
        _detector = new LanguageDetector();
        _detector.AddAllLanguages();
        _logger = logger;
        _scanHistoryRepository = scanHistoryRepository;
    }

    public async Task<ProductResponseDto> GetProductByBarcode(string barcode, List<string> forbiddenAllergens)
    {
        try
        {
            using var doc = await FetchProductJson(barcode);
            if (!doc.RootElement.TryGetProperty("product", out JsonElement productElement))
                throw new NotFoundException(barcode);

            var productDto = MapToResponseDto(productElement, barcode);

            // אימות שפה - שימי לב שאנחנו מעבירים רק שם ורכיבים כדי שהקטגוריות לא יקפיצו שגיאה
            ValidateLanguageSafety(productDto.ProductName, productDto.IngredientsText);

            // בדיקת בטיחות מול המשתמש - עברנו לשימוש ב-Regex
            productDto.IsSafeForUser = CheckSafety(productDto, forbiddenAllergens);

            return productDto;
        }
        catch (Exception ex) when (ex is not UnsupportedLanguageException && ex is not NotFoundException)
        {
            throw new ExternalServiceException("Error processing product data.", ex);
        }
    }

    private async Task<JsonDocument> FetchProductJson(string barcode)
    {
        // הוספנו את categories_tags לשאילתה
        var fields = "product_name,product_name_en,ingredients_text_en,allergens_tags,nutriscore_grade,image_url,categories_tags";
        var url = $"{_baseUrl}{barcode}.json?lc=en&fields={fields}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new NotFoundException(barcode);
            throw new ExternalServiceException($"API Error: {response.StatusCode}");
        }

        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    }

    private ProductResponseDto MapToResponseDto(JsonElement element, string barcode)
    {
        return new ProductResponseDto
        {
            Barcode = barcode,
            ProductName = GetValue(element, "product_name_en") ?? GetValue(element, "product_name") ?? "Unknown",
            IngredientsText = GetValue(element, "ingredients_text_en") ?? "",
            Allergens = ProcessTags(element, "allergens_tags"),
            Categories = ProcessTags(element, "categories_tags"), // חילוץ הקטגוריות לתוך ה-DTO
            NutriscoreGrade = GetValue(element, "nutriscore_grade")?.ToLower() ?? "n/a",
            ImageUrl = GetValue(element, "image_url") ?? ""
        };
    }

    private void ValidateLanguageSafety(string productName, string ingredients)
    {
        string textToVerify = $"{productName} {ingredients}".Trim();

        if (string.IsNullOrWhiteSpace(ingredients) || textToVerify.Length < 5)
            throw new UnsupportedLanguageException("Safety Block: Missing essential data.");

        if (textToVerify.Any(c => c >= 0x0590 && c <= 0x05FF))
            throw new UnsupportedLanguageException("Safety Block: Hebrew characters detected.");

        if (_detector.Detect(textToVerify) != "en")
            throw new UnsupportedLanguageException("Safety Block: Non-English language detected.");
    }

    private bool CheckSafety(ProductResponseDto dto, List<string> forbidden)
    {
        if (forbidden == null || !forbidden.Any()) return true;

        var fullText = $"{dto.ProductName} {dto.IngredientsText} {string.Join(" ", dto.Allergens)}".ToLower();

        foreach (var allergy in forbidden)
        {
            // שימוש ב-Regex כדי לוודא התאמה של מילה שלמה בלבד (\b)
            string pattern = $@"\b{System.Text.RegularExpressions.Regex.Escape(allergy.ToLower().Trim())}\b";
            if (System.Text.RegularExpressions.Regex.IsMatch(fullText, pattern))
            {
                return false;
            }
        }
        return true;
    }

    private List<string> ProcessTags(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var tags) && tags.ValueKind == JsonValueKind.Array)
        {
            return tags.EnumerateArray()
                .Select(t => t.GetString())
                .Where(t => t != null && t.StartsWith("en:"))
                .Select(t => t!.Replace("en:", ""))
                .ToList();
        }
        return new List<string>();
    }

    private string? GetValue(JsonElement element, string prop) =>
        element.TryGetProperty(prop, out var val) ? val.GetString() : null;



    public async Task<List<ProductResponseDto>> GetAlternativeProducts(List<string> categories, List<string> forbiddenAllergens, string currentNutriscore)
    {
        var allResults = new List<ProductResponseDto>();

        // אנחנו רצים על הקטגוריות מהסוף להתחלה (מהספציפי לכללי)
        // משתמשים ב-Reverse כדי להתחיל במוצר הכי דומה
        var reversedCategories = categories.AsEnumerable().Reverse().ToList();

        foreach (var category in reversedCategories)
        {
            _logger.LogInformation("Searching for alternatives in category: {Category}", category);

            var searchUrl = $"https://world.openfoodfacts.org/cgi/search.pl?action=process" +
                            $"&tagtype_0=categories&tag_contains_0=contains&tag_0={category}" +
                            $"&fields=code,product_name,product_name_en,ingredients_text_en,allergens_tags,nutriscore_grade,image_url" +
                            $"&json=true&page_size=24"; // נבקש קצת יותר תוצאות כדי שיהיה ממה לסנן

            var response = await _httpClient.GetAsync(searchUrl);
            if (!response.IsSuccessStatusCode) continue;

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (!doc.RootElement.TryGetProperty("products", out JsonElement productsArray)) continue;

            foreach (var product in productsArray.EnumerateArray())
            {
                var dto = MapToResponseDto(product, GetValue(product, "code") ?? "");

                // בדיקת בטיחות + בדיקה שהמוצר לא כבר ברשימה שלנו + בדיקת ציון תזונתי
                if (CheckSafety(dto, forbiddenAllergens) &&
                    !allResults.Any(r => r.Barcode == dto.Barcode) &&
                    IsBetterOrEqualNutriscore(dto.NutriscoreGrade, currentNutriscore))
                {
                    allResults.Add(dto);
                }

                // אם הגענו ל-5 תחליפים טובים, אפשר לעצור
                if (allResults.Count >= 5) break;
            }

            if (allResults.Count >= 3) break; // אם מצאנו לפחות 3 בקטגוריה ספציפית, זה מספיק טוב
        }

        // מיון סופי: מה-Nutriscore הטוב ביותר (A) להכי פחות טוב
        return allResults.OrderBy(p => p.NutriscoreGrade).ToList();
    }
    // פונקציית עזר להשוואת ציוני Nutriscore (מכיוון שהם אותיות A-E)
    private bool IsBetterOrEqualNutriscore(string newGrade, string currentGrade)
    {
        if (string.IsNullOrEmpty(newGrade) || newGrade == "n/a") return false;
        // ב-Nutriscore, אות קטנה יותר היא טובה יותר (A < B)
        return string.Compare(newGrade, currentGrade, StringComparison.OrdinalIgnoreCase) <= 0;
    }


    //public async Task<ScanResultDto> ScanAndProcessProduct(string barcode, List<string> forbiddenAllergens)
    //{
    //    // 1. סריקת המוצר המקורי (השתמשנו בפונקציה הקיימת שלנו)
    //    var scannedProduct = await GetProductByBarcode(barcode, forbiddenAllergens);

    //    var result = new ScanResultDto { ScannedProduct = scannedProduct };

    //    // 2. אם המוצר אסור - נחפש תחליף באופן אוטומטי
    //    if (!scannedProduct.IsSafeForUser)
    //    {
    //        result.AlternativeProduct = await GetAlternativeProduct(
    //            scannedProduct.Categories,
    //            forbiddenAllergens,
    //            scannedProduct.NutriscoreGrade
    //        );
    //    }

    //    return result;
    //}


    public async Task<ScanResultDto> ScanAndProcessProduct(string barcode, List<string> forbiddenAllergens, int userId)
    {
        // א. קריאה לפונקציה הקיימת שלך שמביאה את המוצר ובודקת שפה
        var scannedProduct = await GetProductByBarcode(barcode, forbiddenAllergens);

        // ב. הכנת אובייקט התשובה המאוחד
        var result = new ScanResultDto
        {
            ScannedProduct = scannedProduct
        };

        // ג. לוגיקת ה"ערך המוסף": אם המוצר אסור למשתמש, נחפש לו אוטומטית תחליף
        if (!scannedProduct.IsSafeForUser)
        {
            result.AlternativeProducts = await GetAlternativeProducts(
                scannedProduct.Categories,
                forbiddenAllergens,
                scannedProduct.NutriscoreGrade
            );
        }

        var historyEntry = new ScanHistory
        {
            UserId = userId,
            Barcode = barcode,
            ProductName = scannedProduct.ProductName,
            ImageUrl = scannedProduct.ImageUrl,
            IsSafe = scannedProduct.IsSafeForUser,
            ScannedAt = DateTime.Now
        };

        // אנחנו לא רוצים לעכב את המשתמש, אז אפשר לשמור אסינכרונית
        _ = _scanHistoryRepository.AddScan(historyEntry);

        return result;
    }
}
