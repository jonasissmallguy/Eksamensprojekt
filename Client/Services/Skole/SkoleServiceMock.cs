using Core;

namespace Client
{

    public class SkoleServiceMock : ISkole
    {
        private List<Skole> skoler = new List<Skole>()
        {
            new Skole()
            {
                Id = 1,
                SkoleNavn = "Kold College",
                Adresse = "Et sted i Kolding"

            }
        };
        
        public async Task<List<SkoleDTO>> GetAllSkoleNames()
        {
            List<SkoleDTO> skoleNames = new();

            foreach (Skole skole in skoler)
            {
                skoleNames.Add(new SkoleDTO
                {
                    SkoleId = skole.Id,
                    SkoleNavn = skole.SkoleNavn,
                    Adresse = skole.Adresse
                });
            }
            return skoleNames;
        }

        public Task<List<Skole>> GetSkoler()
        {
            throw new NotImplementedException();
        }

        public Task CreateSkole(SkoleDTO newSkole)
        {
            throw new NotImplementedException();
        }
    }
}