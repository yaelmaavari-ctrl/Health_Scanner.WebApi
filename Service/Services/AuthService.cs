using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Repository.Interfaces;
using Service;
using Service.Dto;
using Service.Exceptions;
using Service.Interfaces;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Services
{
    public class AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration) : IAuthService
    {
        private readonly IUserRepository _repository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _configuration = configuration;

        
            public async Task ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await _repository.GetById(userId)
                ?? throw new NotFoundException("User not found");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                throw new UnauthorizedAccessException("Old password is incorrect");

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _repository.Update(user);
        }
        

        public async Task DeleteUser(int id)
        {
            var user = await _repository.GetById(id)
                ?? throw new NotFoundException("User not found");

             await _repository.Delete(id);
        }

        public string GenerateToken(UserDto user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
             new Claim(ClaimTypes.Email, user.Email),
             new Claim("Name", user.Name)
              // כאן אפשר להוסיף גם תפקידים (Roles) במידת הצורך
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddYears(1), // המפתח יהיה בתוקף לשנה
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserDto> Login(LoginDto login)
        {
            var user = await _repository.GetByEmail(login.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            return _mapper.Map<UserDto>(user);
        }


        public async Task<UserDto> Register(UserCreateDto register)
        {
            var exists = await _repository.GetByEmail(register.Email);
            if (exists != null)
                throw new InvalidOperationException("User already exists");

            var user = new User
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password)
            };

            var result = await _repository.Add(user);
            return _mapper.Map<UserDto>(result);
        }


    }
}
