using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> GetProductByBarcode(string barcode, List<string> forbiddenAllergens);
        Task<List<ProductResponseDto>> GetAlternativeProducts(List<string> categories, List<string> forbiddenAllergens, string currentNutriscore);
        Task<ScanResultDto> ScanAndProcessProduct(string barcode, List<string> forbiddenAllergens, int userId);
    }

}
