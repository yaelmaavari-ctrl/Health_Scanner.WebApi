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
    public class UserRepository(IContext context) : IUserRepository
    {
        private readonly IContext _context = context;
        public async Task<User> Add(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.Save();
            return user;
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null) 
            { 
                _context.Users.Remove(user);
                await _context.Save();
                return true;
            }
            return false;
        }

        public async Task<User?> GetByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;

        }

        public Task<User?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(User updatedUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == updatedUser.Id);
            if(user == null)
              throw new Exception("User not found");

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;
            await _context.Save();
 
        }
    }
}
