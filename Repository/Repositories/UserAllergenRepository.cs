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
    public class UserAllergenRepository(IContext context): IUserAllergenRepository
    {
        private readonly IContext _context = context;

        public async Task<UserAllergen> AddAllergenToUser(int userId, int allergenId)
        {
            var userAllergen = new UserAllergen
            {
                UserId = userId,
                AllergenId = allergenId
            };
            await _context.UserAllergens.AddAsync(userAllergen);
            await _context.Save();
            return userAllergen;
        }

        public async Task<IEnumerable<Allergen>> GetAllergensByUserId(int userId)
        {
            return await _context.UserAllergens
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.Allergen) 
                .ToListAsync();
        }

        public async Task<bool> IsUserAllergicTo(int userId, int allergenId)
        {
            return await _context.UserAllergens
                .AnyAsync(ua => ua.UserId == userId && ua.AllergenId == allergenId);
        
        }

        public async Task<bool> RemoveAllergenFromUser(int userId, int allergenId)
        {
            var userAllergen = await _context.UserAllergens
                .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AllergenId == allergenId);

            if (userAllergen == null) return false;     

            _context.UserAllergens.Remove(userAllergen);
            await _context.Save();
            return true;
        }

        // 5. מציאת כל המשתמשים שאלרגיים למשהו ספציפי
        //public async Task<IEnumerable<User>> GetUsersByAllergenId(int allergenId)
        //{
        //    return await _context.UserAllergens
        //        .Where(ua => ua.AllergenId == allergenId)
        //        .Select(ua => ua.User)
        //        .ToListAsync();
        //}
    }
}
