using AutoMapper;
using Repository.Interfaces;
using Service.Dto;
using Service.Interfaces;
using Service.Exceptions;
using Repository.Entities;

namespace Service.Services
{
    public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
    {
        private readonly IUserRepository _repository = userRepository;
        private readonly IMapper _mapper = mapper;

        public async Task ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await _repository.GetById(userId) ?? throw new NotFoundException("User not found");
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                throw new UnauthorizedAccessException("Old password is incorrect");
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _repository.Update(user);          
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _repository.GetById(id)
                ?? throw new NotFoundException("User not found");

            return await _repository.Delete(id);
        }

        public async Task<UserDto> GetByEmail(string email)
        {
            var user = await _repository.GetByEmail(email) ?? throw new NotFoundException("User not found");
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await _repository.GetById(id) ?? throw new NotFoundException("User not found");
            return _mapper.Map<UserDto>(user);
        }


        public async Task<UserDto> Login(string email, string password)
        {
            var user = await _repository.GetByEmail(email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            return _mapper.Map<UserDto>(user);
        }


        public async Task<UserDto> Register(UserCreateDto dto)
        {
            var exists = await _repository.GetByEmail(dto.Email);
            if(exists !=null)
                throw new Exception("User already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            var result = await _repository.Add(user);
            return new UserDto
            {
                Id = result.Id,
                Name = result.Name,
                Email = result.Email
            };
        }


        public async Task<UserDto> UpdateUser(int id, UserUpdateDto dto)
        {
            var user = await _repository.GetById(id)
                ?? throw new NotFoundException("User not found");

            user.Name = dto.Name;
            user.Email = dto.Email;

            await _repository.Update(user);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UserExists(string email)
        {
            return await _repository.GetByEmail(email) != null;
        }
    }
}
