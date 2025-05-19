using Core;
using MongoDB.Driver;
namespace Server
{

    public interface IUserRepository
    {
        //Get
        Task<User> GetUserById(int id);
        Task<List<User>> GetAllUsers();
        Task<List<User>> GetAllUsersWithOutMyself(int userId);
        Task<User> GetUserByEmail(string email);
        
        
        //Post
        Task<User> SaveBruger(User bruger);
        
        Task<bool> CheckUnique(string email);
        
        
        
        //Put
        Task DeleteUser(int studentId);
        Task DeactivateUser(int studentId);
        Task ActivateUser(int studentId);
        Task UpdateRolle(string rolle, int userId);
        Task<UpdateResult> UpadtePassword(string email, string updatedPassword);
        
        Task<bool> UpdateUser(User user);

        Task<List<User>> GetAllStudents();

    }

}