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
                FirstName = "Jane"
            },
            new User
            {
                Id = 2,
                Email = "theis@comwell.com",
                Password = "123456",
                Rolle = "Køkkenchef",
                FirstName = "Theis",
            },
            new User
            {
                Id = 3,
                Email = "elev@comwell.com",
                Password = "123456",
                Rolle = "Elev",
                FirstName = "Charles",
                Year = "År 1",
                Skole = "Kold Kollege",
                Uddannelse = "EUX",
                StartDate = new DateOnly(2025, 5, 5),
                EndDate = new DateOnly(2028, 5, 5)
            },
            new User
            {
                Id = 4,
                Email = "kok@comwell.com",
                Password = "123456",
                Rolle = "Kok",
                FirstName = "Kok",
        
            }
        };

        public async Task<User> GetBrugerById(int userId)
        {
            var user = _allUsers.FirstOrDefault(x => x.Id == userId);

            return user;
        }

        public async Task<bool> OpdaterBruger(int brugerId, User updatedBruger)
        {
            var user = _allUsers.FirstOrDefault(x => x.Id == brugerId);

            if (user == null)
            {
                return false;
            }

            user.FirstName = updatedBruger.FirstName;
            user.LastName = updatedBruger.LastName;
            user.Email = updatedBruger.Email;
            user.Mobile = updatedBruger.Mobile;
            user.Rolle = updatedBruger.Rolle;
            user.Status = updatedBruger.Status;
            user.Year = updatedBruger.Year;
            user.Skole = updatedBruger.Skole;
            user.Uddannelse = updatedBruger.Uddannelse;
            user.StartDate = updatedBruger.StartDate;
            user.EndDate = updatedBruger.EndDate;

            return true;
        }

        /*
        public async Task<List<ManagerDTO>> GetAllManagers()
        {
            var managers = new List<ManagerDTO>();

            foreach (var user in _allUsers)
            {
                if (user.Rolle == "Køkkenchef")
                {
                    managers.Add(new ManagerDTO
                    {
                        ManagerId = user.Id,
                        ManagerName = user.FirstName + " " + user.LastName
                    });
                }
            }

            return managers;
        }
        */

        public string GeneratePassword()
        {
            var random = new Random();
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var password = "";

            for (int i = 0; i < 8; i++)
            {
                var index = random.Next(chars.Length);
                password += chars[index];
            }

            return password;
        }

        public int GenerateId()
        {
            var random = new Random();
            return random.Next(1000, 9999);
        }

        public bool CheckUniqueMail(string email)
        {
            var exists = _allUsers.Any(x => x.Email == email);
            return !exists;
        }

        public async Task<User> OpretBruger(BrugerCreateDTO nyBruger)
        {
            if (!CheckUniqueMail(nyBruger.Email))
            {
                
            }

            var newUser = new User
            {
                Id = GenerateId(),
                FirstName = nyBruger.FirstName,
                LastName = nyBruger.LastName,
                Email = nyBruger.Email,
                Password = GeneratePassword(),
                Mobile = nyBruger.Mobile,
                Rolle = nyBruger.Rolle,
                StartDate = nyBruger.StartDate,
                EndDate = nyBruger.EndDate,
        
                Year = nyBruger.Year,
                Skole = nyBruger.Skole,
                Uddannelse = nyBruger.Uddannelse
                
            };

            _allUsers.Add(newUser);

            return newUser;
        }

        public async Task<List<ElevOversigtDTO>> GetElevOversigt()
        {
            var elevOversigt = new List<ElevOversigtDTO>();

            var elever = _allUsers.Where(x => x.Rolle == "Elev").ToList();

            foreach (var elev in elever)
            {
                elevOversigt.Add(new ElevOversigtDTO
                {
                    Id = elev.Id,
                    Name = elev.FirstName,
                    HotelId = elev.HotelId,
                    HotelNavn = "test",
                    Roller = elev.Rolle,
                    Ansvarlig = "test ansvarlig",
                    Year = elev.Year,
                    Skole = elev.Skole,
                    Uddannelse = elev.Uddannelse,
                    StartDate = elev.StartDate,
                    EndDate = elev.EndDate
                    
                });
            }

            return await Task.FromResult(elevOversigt);
        }

        public Task<List<ElevOversigtDTO>> GetElevOversigtByHotelId(int hotelId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BrugerLoginDTO>> GetAllUsers()
        {
            return  new List<BrugerLoginDTO>();
        }

        public Task<List<BrugerLoginDTO>> GetAllActiveUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<List<BrugerAdministrationDTO>> GetAllUsersWithOutCurrent(int userId)
        {
            var list = _allUsers.Where(x => x.Id != userId).ToList();
            return new List<BrugerAdministrationDTO>();
        }

        public async Task<List<User>> GetAllUsersByStudentId(List<int> studentIds)
        {
            var users = new List<User>();

            foreach (var id in studentIds)
            {
                var user = _allUsers.FirstOrDefault(x => x.Id == id);

                if (user != null)
                {
                    users.Add(user);
                }
            }
            return users;

        }

        public async Task<bool> DeleteUser(int userId, string rolle)
        {
            _allUsers.RemoveAll(x => x.Id == userId);

            return true;
        }

        public async Task ChangeRolle(string newRolle, int userId)
        {
            var user = _allUsers.FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {
                user.Rolle = newRolle;
            }

        }

        public async Task DeActivateUser(int userId, string rolle)
        {
            var user = _allUsers.FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {
                user.Status = "Deactivated";
            }

        }

        public async Task ActivateUser(int userId)
        {
            var user = _allUsers.FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {
                user.Status = "Active";
            }
            
        }

        public Task<bool> UpdateHotel(int hotelId, string hotelName, int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateHotel(int userId, int hotelId, string updatedHotelNavn)
        {
            var user = _allUsers.FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {
                user.HotelId = hotelId;
            }

            return true;


        }

        public async Task SaveStudentPlan(int studentId, Plan plan)
        {
            var user = _allUsers.FirstOrDefault(x => x.Id == studentId);

            if (user != null)
            {
                user.ElevPlan = plan;
            }
            
        }

        public Task<User> GetUserById(int currentUserId)
        {
            var user = _allUsers.FirstOrDefault(x => x.Id == currentUserId);
            return Task.FromResult(user);
        }

        public Task<List<KursusDeltagerListeDTO>> GetAllStudents()
        {
            throw new NotImplementedException();
        }

        public Task<List<KursusDeltagerListeDTO>> GetAllStudentsMissingCourse(string courseCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendEmail(HashSet<int> studentIds, string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendEmail(HashSet<int> studentIds)
        {
            throw new NotImplementedException();
        }
    }
}