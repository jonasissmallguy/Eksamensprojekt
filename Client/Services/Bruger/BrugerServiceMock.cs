using Core;

namespace Client
{

    public class BrugerServiceMock : IBruger
    {
        private List<Bruger> _allUsers = new List<Bruger>
        {
            new Bruger
            {
                Id = 1,
                Email = "admin@admin.com",
                Password = "123456",
                Rolle = "HR"
            },
            new Bruger
            {
                Id = 2,
                Email = "elev1@admin.com",
                Password = "123456",
                Rolle = "Elev",
                MentorId = 1,
                RestaurantId = 1,
                Navn = "Han Solo"
            },
            new Bruger
            {
                Id = 3,
                Email = "elev1@admin.com",
                Password = "123456",
                Rolle = "Mentor"
            },
            new Bruger
            {
                Id = 4,
                Email = "tjoernevej53@gmail.com",
                Password = "123456",
                Rolle = "Mentor"
            }
            
        };

        
        public async Task<BrugerProfilDTO> GetBrugerById(int userId)
        {
            Bruger? bruger = _allUsers.FirstOrDefault(x => x.Id == userId);

            if (bruger == null)
            {
                return null;
            }

            return new BrugerProfilDTO
            {
                Email = bruger.Email,
                MentorNavn = "Martin",
                Navn = bruger.Navn,
                RegionNavn = "Fyn",
                RestaurantNavn = "Comwell Aarhus",
                Rolle = bruger.Rolle,
                Telefon = bruger.Telefon

            };
        }

        public async Task<bool> OpdaterBruger(int brugerId, BrugerProfilDTO updatedBruger)
        {
            Bruger? bruger = _allUsers.FirstOrDefault(x => x.Id == brugerId);

            if (bruger == null)
            {
                return false;
            }

            return true;

        }
    }
}