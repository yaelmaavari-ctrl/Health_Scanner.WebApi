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

        public async Task<User?> Update(int Id, User updatedUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);
            if(user == null)
                return null;
            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;
            await _context.Save();
            return user;
        }
    }
}
