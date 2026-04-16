using System.Threading.Tasks;
using Repository.Entities;

namespace Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetById(int id);
        Task<User?> GetByEmail(string email);
        Task<User> Add(User user);
        Task Update(User user);
        Task<bool> Delete(int id);
    }
}
