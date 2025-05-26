using Core;
using MongoDB.Driver;
namespace Server
{

    public interface IUserRepository
    {
        
               
        //Create
        Task<User> SaveBruger(User bruger);
        
        //Read
        Task<User> GetUserById(int id);
        Task<List<User>> GetAllUsers();
        Task<List<User>> GetAllActiveUsers();
        Task<List<User>> GetAllUsersWithOutMyself(int userId);
        Task<User> GetUserByEmail(string email);
        Task<List<User>> GetAllStudents();
        Task<List<User>> GetAllStudentsByHotelId(int hotelId);
        Task<List<User>> GetAllStudentsMissingCourse(string kursusCode);
        Task<bool> CheckUnique(string email);
        
        //Update
        Task<UpdateResult> DeactivateUser(int studentId);
        Task<UpdateResult> ActivateUser(int studentId);
        Task<UpdateResult> UpdateRolle(string rolle, int userId);
        Task<UpdateResult> UpadtePassword(string email, string updatedPassword);
        Task<UpdateResult> UpdateHotel(int userId, int hotelId, string updatedHotelNavn);
        
        //Delete
        Task<DeleteResult> DeleteUser(int studentId);
    }

}