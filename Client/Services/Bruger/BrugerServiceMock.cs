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
                Rolle = "HR",
                FirstName = "Jane",
            },
            new User
            {
                Id = 2,
                Email = "theis@comwell.com",
                Password = "123456",
                Rolle = "Køkkenchef",
                FirstName = "Theis"
            },
            new User
            {
                Id = 3,
                Email = "elev@comwell.com",
                Password = "123456",
                Rolle = "Elev",
                FirstName = "Charles"
            },
            new User
            {
                Id = 4,
                Email = "kok@comwell.com",
                Password = "123456",
                Rolle = "Kok",
                FirstName = "Kok"
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
                Id = bruger.Id,
                Email = bruger.Email,
                MentorNavn = "Martin",
                Navn = bruger.FirstName ,
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

        public string GeneratePassword()
        {
            Random rnd = new();
            string password = String.Empty;
            
            string bogstaver = "abcdefghijklmnopqrstuvwxyz0123456789";
            int size = 8;

            for (int i = 0; i < size; i++)
            {
                int x = rnd.Next(bogstaver.Length);
                password = password + bogstaver[x];
            }
            Console.WriteLine(password);
            return password;
        }

        public int GenerateId()
        {
            Random rnd = new();
            return rnd.Next(1,9999);
        }

        //Hjælpefunktion til at tjekke, at mail er unik
        public bool CheckUniqueMail(string email)
        {
            if (_allUsers.Any(x => x.Email == email))
            {
                return false;
            }

            return true;
        }

        public async Task<User> OpretBruger(BrugerCreateDTO nyBruger)
        {

            if (!CheckUniqueMail(nyBruger.Email))
            {
                
            }
            
            var bruger = (new User
                {
                    Id = GenerateId(),
                    FirstName = nyBruger.FirstName,
                    LastName = nyBruger.LastName,
                    Email = nyBruger.Email,
                    Password = GeneratePassword(),
                    Mobile = nyBruger.Mobile,
                    Rolle = nyBruger.Rolle,
                    HotelId = nyBruger.HotelId,
                    StartDate = nyBruger.StartDate
                });
            
             _allUsers.Add(bruger);
             return bruger;

        }

        public async Task<List<ElevOversigtDTO>> GetElevOversigt()
        {
            List<ElevOversigtDTO> alleElevOversigts = new();
            var elever = _allUsers.Where(x => x.Rolle == "Elev").ToList();

            foreach (var elev in elever)
            {
                alleElevOversigts.Add(new ElevOversigtDTO
                {
                    Id = elev.Id,
                    Name = elev.FirstName + " " + elev.LastName,
                    HotelId = elev.HotelId,
                    Hotel = "test",
                    Roller = elev.Rolle,
                    Ansvarlig = "test ansvarlig"
                });
            }
            
            return alleElevOversigts;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return _allUsers;
        }
    }
}