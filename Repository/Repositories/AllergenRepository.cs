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
    public class AllergenRepository(IContext context) : IAllergenRepository
    {
        private readonly IContext _context = context;
        public async Task<Allergen> AddItem(Allergen item)
        {
           await _context.Allergens.AddAsync(item);
           await _context.Save();
           return item;
        }

        public async Task<bool> DeleteItem(int id)
        {
            var allergen = await _context.Allergens.FirstOrDefaultAsync(a => a.Id == id);
            if(allergen != null)
            {
                _context.Allergens.Remove(allergen);
                await _context.Save();
                return true;
            }
            return false;
        }

        public async Task<List<Allergen>> GetAll()
        {
            return await _context.Allergens.ToListAsync();
        }

        public async Task<Allergen?> GetById(int id)
        {
            return await _context.Allergens.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> Exists(string name)
        {
            return await _context.Allergens
                .AnyAsync(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public async Task<Allergen?> UpdateItem(int id, Allergen item)
        {
            var allergen = await _context.Allergens.FirstOrDefaultAsync(a => a.Id == id);
            if(allergen == null)
                return null;
            allergen.Name = item.Name;
            await _context.Save();
            return allergen;
        }
    }
}
