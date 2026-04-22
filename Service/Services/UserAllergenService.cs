using AutoMapper;
using Repository.Interfaces;
using Service.Dto;
using Service.Exceptions;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UserAllergenService(
        IUserAllergenRepository userAllergenRepository,
        IUserRepository userRepository,
        IAllergenRepository allergenRepository,
        IMapper mapper) : IUserAllergenService
    {
        private readonly IUserAllergenRepository _userAllergenRepo = userAllergenRepository;
        private readonly IUserRepository _userRepo = userRepository;
        private readonly IAllergenRepository _allergenRepo = allergenRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<AllergenDto> AddAllergenToUser(int userId, int allergenId)
        {
            var user = await _userRepo.GetById(userId)
                ?? throw new NotFoundException($"User with ID {userId} not found.");

            var allergen = await _allergenRepo.GetById(allergenId)
                ?? throw new NotFoundException($"Allergen with ID {allergenId} not found.");

            if (await _userAllergenRepo.IsUserAllergicTo(userId, allergenId))
            {
                throw new InvalidOperationException("User already has this allergen assigned.");
            }
            await _userAllergenRepo.AddAllergenToUser(userId, allergenId);
            return _mapper.Map<AllergenDto>(allergen);
        }


        public async Task<IEnumerable<AllergenDto>> GetUserAllergens(int userId)
        {
            var user = await _userRepo.GetById(userId)
            ?? throw new NotFoundException($"User with ID {userId} not found.");

            var allergens = await _userAllergenRepo.GetAllergensByUserId(userId);

            return _mapper.Map<IEnumerable<AllergenDto>>(allergens);
        }

        public async Task RemoveAllergenFromUser(int userId, int allergenId)
        {
            var deleted = await _userAllergenRepo.RemoveAllergenFromUser(userId, allergenId);

            if (!deleted)
            {
                throw new NotFoundException("The connection between the user and this allergen does not exist.");
            }
        }
    }
}
