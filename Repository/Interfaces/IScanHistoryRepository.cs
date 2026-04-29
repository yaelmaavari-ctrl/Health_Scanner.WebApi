using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IScanHistoryRepository
    {
        Task AddScan(ScanHistory scan);
        Task<IEnumerable<ScanHistory>> GetUserHistory(int userId, int limit = 10);
    }
}
