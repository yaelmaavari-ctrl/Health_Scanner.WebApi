using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IScanHistoryService
    {
        Task<IEnumerable<ScanHistoryDto>> GetUserHistory(int userId);
        Task AddToHistory(int userId, ScanResultDto scanResult, string barcode);
    }
}
