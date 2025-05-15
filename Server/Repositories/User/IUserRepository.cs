using Core;

namespace Server
{

    public interface IUserRepository
    {
        
        Task<User> GetUserById(int id);
        Task<User> SaveBruger(User bruger);
        Task<List<User>> GetAllUsers();

    }

}