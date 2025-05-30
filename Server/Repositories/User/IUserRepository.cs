using Core;
using MongoDB.Driver;
namespace Server
{

    public interface IUserRepository
    {
        
               
        //Indsætter en bruger
        Task<User> SaveBruger(User bruger);
        //Retunerer en bruger
        Task<User> GetUserById(int userId);
        //Retunerer alle brugere
        Task<List<User>> GetAllUsers();
        //Retunerer alle brugere hvor Status = Active
        Task<List<User>> GetAllActiveUsers();
        //Retunerer alle brugere uden currentUser
        Task<List<User>> GetAllUsersWithOutMyself(int userId);
        //Retunerer en bruger, hvor email = Email
        Task<User> GetUserByEmail(string email);
        //Retunerer alle elever med Rolle = Elev
        Task<List<User>> GetAllStudents();
        //Retunerer alle elever med hotelId = HotelId og Rolle = Elev
        Task<List<User>> GetAllStudentsByHotelId(int hotelId);
        //Tjekker om email er unik
        Task<bool> CheckUnique(string email);
        //Sætter Status = Deactivated efter id
        Task<UpdateResult> DeactivateUser(int userId);
        //Sætter Status = Active efter userId
        Task<UpdateResult> ActivateUser(int userId);
        //Sætter rolle = Rolle efter userId
        Task<UpdateResult> UpdateRolle(string rolle, int userId);
        //Sætter updatedPassword = Password efter email
        Task<UpdateResult> UpdatePassword(string email, string updatedPassword);
        //Sætter hotelId = HotelId og updatedHotelNavn = Hotelnavn efter userId
        Task<UpdateResult> UpdateHotel(int userId, int hotelId, string updatedHotelNavn);
        //Sletter en bruger efter userId 
        Task<DeleteResult> DeleteUser(int userId);
    }

}