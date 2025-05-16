using Core;

namespace Client
{

    public interface IAuth
    {
        Task<BrugerLoginDTO?> GetBruger();
        Task<BrugerLoginDTO> Login(string username, string password);
        Task Logout();
        Task GetUserByEmail(string email);
        Task<bool> CheckVerficiationCode(string email, string kode);
        Task<string> GetLocalStorageResetEmail();
        Task DeleteLocalStorageResetEmail();
        Task<bool> UpdatePassword(string updatedPassword, string confirmedPassword);
        

    }

}