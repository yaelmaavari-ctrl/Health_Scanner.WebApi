using AutoMapper;
using Repository.Entities;
using Repository.Interfaces;
using Service;
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
    public class AllergenService(IAllergenRepository repository, IMapper mapper) : IAllergenService
    {
        private readonly IAllergenRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<AllergenDto> Add(AllergenCreateDto dto)
        {
            if (await _repository.Exists(dto.Name))
            {
                throw new InvalidOperationException($"Allergen with name '{dto.Name}' already exists in the database.");
            }
            var allergen = _mapper.Map<Allergen>(dto);
            var result = await _repository.AddItem(allergen);

            return _mapper.Map<AllergenDto>(result);
        }

        public async Task<bool> Delete(int id)
        {
            var allergen = await _repository.GetById(id)
                ?? throw new NotFoundException("Allergen not found");

            return await _repository.DeleteItem(id);

        }
        public async Task<IEnumerable<AllergenDto>> GetAll()
        {
            var allergens = await _repository.GetAll();
            return allergens.Select(a => _mapper.Map<AllergenDto>(a));
        }

        public async Task<AllergenDto> GetById(int id)
        {
            var allergen = await _repository.GetById(id)
                ?? throw new NotFoundException("Allergen not found");
            return _mapper.Map<AllergenDto>(allergen);
        }

        public async Task<AllergenDto> Update(int id, AllergenCreateDto dto)
        {
            var existingAllergen = await _repository.GetById(id);
            if (existingAllergen == null)
            {
                throw new NotFoundException($"Allergen with ID {id} was not found.");
            }
            // 3. בדיקת כפילות שם: מוודאים שאין אלרגיה אחרת עם אותו שם (חוץ מזו שאנחנו מעדכנים כרגע)
            // הערה: כדאי שתהיה לך פונקציה ב-Repository שבודקת קיום שם ל-ID אחר
            if (await _repository.Exists(dto.Name) && existingAllergen.Name.ToLower() != dto.Name.ToLower())
            {
                throw new InvalidOperationException($"Another allergen with the name '{dto.Name}' already exists.");
            }

            // 4. עדכון השדות מה-DTO לישות הקיימת באמצעות AutoMapper
            // הפעולה הזו מעתיקה את ה-Name מה-dto לתוך ה-existingAllergen
            _mapper.Map(dto, existingAllergen);

            // 5. שמירת השינויים בבסיס הנתונים
            await _repository.UpdateItem(id,existingAllergen);

            // 6. החזרת האובייקט המעודכן כ-Dto
            return _mapper.Map<AllergenDto>(existingAllergen);
        }
    }
}
