using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ScanHistoryRepository(IContext context) : IScanHistoryRepository
    {
        private readonly IContext _context = context; 

        public async Task AddScan(ScanHistory scan)
        {
            await _context.ScanHistories.AddAsync(scan);
            await _context.Save();
        }

        public async Task<IEnumerable<ScanHistory>> GetUserHistory(int userId, int limit = 10)
        {
            return await _context.ScanHistories
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.ScannedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}




