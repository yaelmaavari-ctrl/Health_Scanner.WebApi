using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class ProductResponseDto
    {
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public string IngredientsText { get; set; } // רשימת הרכיבים המלאה
        public List<string> Allergens { get; set; } // אלרגנים מזוהים מה-API
        public string NutriscoreGrade { get; set; } // A, B, C, D, E
        public string ImageUrl { get; set; }
        public bool IsSafeForUser { get; set; } // שדה שנחשב אצלנו בשרת
    }
}
