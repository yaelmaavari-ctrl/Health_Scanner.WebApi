using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IAllergenRepository
    {
        Task<List<Allergen?>> GetAll();
        Task<Allergen?> GetById(int id);
        Task<Allergen> AddItem(Allergen item);
        Task<Allergen> UpdateItem(int id, Allergen item);
        Task<bool> DeleteItem(int id);
        Task<bool> Exists(string name);
    }
}
