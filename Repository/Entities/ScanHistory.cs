using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class ScanHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public bool IsSafe { get; set; } // התוצאה שחישבנו
        public DateTime ScannedAt { get; set; } = DateTime.Now;

        // קשר למשתמש (אופציונלי, תלוי אם הגדרת Navigation Properties)
        public virtual User User { get; set; }
    }
}
