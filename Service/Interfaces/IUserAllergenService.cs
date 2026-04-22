using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserAllergenService
    {
        Task AddAllergenToUser(int userId, int allergenId);
        Task RemoveAllergenFromUser(int userId, int allergenId);
        Task<IEnumerable<AllergenDto>> GetUserAllergens(int userId);
    }
}

