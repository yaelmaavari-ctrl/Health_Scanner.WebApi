using Service.Dto;

namespace Service.Interfaces
{
        public interface IAllergenService
        {
            Task<IEnumerable<AllergenDto>> GetAll();
            Task<AllergenDto> GetById(int id);
            Task<AllergenDto> Add(AllergenCreateDto dto);
            Task<AllergenDto> Update(int id, AllergenCreateDto dto);
            Task<bool> Delete(int id);
        }
}


