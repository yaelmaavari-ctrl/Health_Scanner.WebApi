using Repository.Entities;


namespace Repository.Interfaces
{
    public interface IUserAllergenRepository
    {
        Task<UserAllergen> AddAllergenToUser(int userId, int allergenId);
        Task<bool> RemoveAllergenFromUser(int userId, int allergenId);
        Task<IEnumerable<Allergen>> GetAllergensByUserId(int userId);
        Task<bool> IsUserAllergicTo(int userId, int allergenId);

        // בונוס: מציאת כל המשתמשים שאלרגיים למשהו ספציפי (שימושי לסטטיסטיקה או התראות)
        //Task<IEnumerable<User>> GetUsersByAllergenId(int allergenId);
    }   
}


