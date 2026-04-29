using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class ScanResultDto
    {
        public ProductResponseDto ScannedProduct { get; set; }
        public List<ProductResponseDto> AlternativeProducts { get; set; } = new List<ProductResponseDto>();
    }
}
