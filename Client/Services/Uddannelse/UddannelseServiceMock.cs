using Core;

namespace Client
{

    public class UddannelseServiceMock : IUddannelse
    {
        private List<Uddannelse> uddannelser = new List<Uddannelse>()
        {
            new Uddannelse()
            {
                Id = 1,
                UddannelseNavn = "EUX",
            },
            new Uddannelse()
            {
                Id = 2,
                UddannelseNavn = "Voksenelev",
            }
        };
        
        public async Task<List<UddannelseDTO>> GetAllUddannelseNames()
        {
            List<UddannelseDTO> uddannelseNames = new();

            foreach (Uddannelse uddannelse in uddannelser)
            {
                uddannelseNames.Add(new UddannelseDTO
                {
                    UddannelseId = uddannelse.Id,
                    UddannelseNavn = uddannelse.UddannelseNavn
                });
            }
            return uddannelseNames;
        }

        public Task<List<Uddannelse>> GetUddannelser()
        {
            throw new NotImplementedException();
        }

        public Task CreateUddannelse(UddannelseDTO newUddannelse)
        {
            throw new NotImplementedException();
        }
    }
}