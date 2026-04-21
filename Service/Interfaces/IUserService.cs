using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetById(int id);
        Task<UserDto> GetByEmail(string email);
        Task<UserDto> Register(UserCreateDto dto);
        Task<UserDto> Login(string email, string password);
        Task<UserDto> UpdateUser(int id, UserUpdateDto dto);
        Task ChangePassword(int userId, string oldPassword, string newPassword);
        Task<bool> DeleteUser(int id);
    }
}
