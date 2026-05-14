using Service.Dto;

namespace Service.Interfaces
{
        public interface IAuthService
        {
            Task<UserDto> Register(UserCreateDto register);
            Task<UserDto> Login(LoginDto login);
            string GenerateToken(UserDto user);
            Task DeleteUser(int userId);
            Task ChangePassword(int userId, string oldPassword, string newPassword);
    }
}
