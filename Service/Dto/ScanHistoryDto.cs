using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class ScanHistoryDto
    {
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public bool IsSafe { get; set; }
        public DateTime ScannedAt { get; set; }
    }
}
