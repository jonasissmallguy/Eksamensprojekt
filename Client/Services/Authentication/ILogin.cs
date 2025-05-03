using Core;

namespace Client
{

    public interface ILogin
    {
        Task<BrugerLoginDTO?> GetBruger();
        Task<BrugerLoginDTO> Login(string username, string password);
        Task Logout();
        Task GetUserByEmail(string email);

        Task<bool> CheckVerficiationCode(string kode);

    }

}