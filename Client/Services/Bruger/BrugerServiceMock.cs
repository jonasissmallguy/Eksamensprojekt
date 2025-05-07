using Core;

namespace Client
{

    public class BrugerServiceMock : IBruger
    {
        private List<User> _allUsers = new List<User>
        {
            new User
            {
                Id = 1,
                Email = "admin@admin.com",
                Password = "123456",
                Rolle = "HR"
            },
            new User
            {
                Id = 2,
                Email = "elev1@admin.com",
                Password = "123456",
                Rolle = "Elev",
                RestaurantId = 1
                
            },
            new User
            {
                Id = 3,
                Email = "elev1@admin.com",
                Password = "123456",
                Rolle = "Mentor"
            },
            new User
            {
                Id = 4,
                Email = "tjoernevej53@gmail.com",
                Password = "123456",
                Rolle = "Mentor"
            },
            new User
            {
                FirstName = "Theis",
                LastName = "Jones",
                Id = 5,
                Email = "test@gmail.com",
                Password = "123456",
                Rolle = "Køkkenchef"
            }
            
        };

        
        public async Task<BrugerProfilDTO> GetBrugerById(int userId)
        {
            User? bruger = _allUsers.FirstOrDefault(x => x.Id == userId);

            if (bruger == null)
            {
                return null;
            }

            return new BrugerProfilDTO
            {
                Email = bruger.Email,
                MentorNavn = "Martin",
                Navn = bruger.FirstName,
                RegionNavn = "Fyn",
                RestaurantNavn = "Comwell Aarhus",
                Rolle = bruger.Rolle
            };
        }

        public async Task<bool> OpdaterBruger(int brugerId, BrugerProfilDTO updatedBruger)
        {
            User? bruger = _allUsers.FirstOrDefault(x => x.Id == brugerId);

            if (bruger == null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<ManagerDTO>> GetAllManagers()
        {
            List<ManagerDTO> alleManagers = new();

            foreach (var user in _allUsers)
            {
                if (user.Rolle == "Køkkenchef")
                {
                    alleManagers.Add(new ManagerDTO
                    {
                        ManagerId = user.Id,
                        ManagerName = user.FirstName + " " + user.LastName
                    } );
                }
            }
            return alleManagers;
        }
    }
}