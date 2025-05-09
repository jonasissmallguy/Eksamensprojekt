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
                FirstName = "Charles",
                Email = "elev1@admin.com",
                Password = "123456",
                Rolle = "Elev",
                HotelId = 1
                
            },
            new User
            {
                Id = 3,
                Email = "elev1@2424admin.com",
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
            },
            new User
            {
                Id = 6,
                FirstName = "Hans",
                Email = "elev2@admin.com",
                Password = "123456",
                Rolle = "Elev",
                HotelId = 1
                
            },
            new User
            {
                Id = 7,
                FirstName = "Lene",
                Email = "elev3@admin.com",
                Password = "123456",
                Rolle = "Elev",
                HotelId = 2
            },

            new User
            {
                Id = 8,
                FirstName = "Jens",
                Email = "elev4@admin.com",
                Password = "123456",
                Rolle = "Elev",
                HotelId = 2
            },

            new User
            {
                Id = 9,
                FirstName = "Anna",
                Email = "elev5@admin.com",
                Password = "123456",
                Rolle = "Elev",
                HotelId = 1
            },

            new User
            {
                Id = 10,
                FirstName = "Peter",
                Email = "elev6@admin.com",
                Password = "123456",
                Rolle = "Elev",
                HotelId = 1
            },

            new User
            {
                Id = 11,
                FirstName = "Mette",
                Email = "elev7@admin.com",
                Password = "123456",
                Rolle = "Elev",
                HotelId = 1
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

        public async Task<User> OpretBruger(BrugerCreateDTO nyBruger)
        {
             var bruger = (new User
             {
                Id = 2,
                FirstName = nyBruger.FirstName,
                LastName = nyBruger.LastName,
                Email = nyBruger.Email,
                Password = GeneratePassword(),
                Mobile = nyBruger.Mobile,
                Rolle = nyBruger.Rolle,
                HotelId = nyBruger.HotelId,
                StartDate = nyBruger.StartDate
            });

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
                    // Tilføjet af Rasmus
                    HotelId = elev.HotelId,
                    Hotel = "test",
                    Roller = elev.Rolle,
                    Ansvarlig = "test ansvarlig"
                });
            }
            
            return alleElevOversigts;
        }
    }
}