using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Service.Dto;
using Service.Interfaces;

public class ScanHistoryService(IScanHistoryRepository scanHistoryRepository, IMapper mapper) : IScanHistoryService
{
    private readonly IScanHistoryRepository _scanHistoryRepository = scanHistoryRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ScanHistoryDto>> GetUserHistory(int userId)
    {
        var historyEntities = await _scanHistoryRepository.GetUserHistory(userId);

        return _mapper.Map<IEnumerable<ScanHistoryDto>>(historyEntities);
    }

    public async Task AddToHistory(int userId, ScanResultDto scanResult, string barcode)
    {
        if (scanResult?.ScannedProduct != null)
        {
            var historyEntry = new ScanHistory
            {
                UserId = userId,
                Barcode = barcode,
                ProductName = scanResult.ScannedProduct.ProductName,
                ImageUrl = scanResult.ScannedProduct.ImageUrl,
                IsSafe = scanResult.ScannedProduct.IsSafeForUser,
                ScannedAt = DateTime.Now
            };
            await _scanHistoryRepository.AddScan(historyEntry);
        }  
    }
}