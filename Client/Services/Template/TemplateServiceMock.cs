using Core;
using MongoDB.Bson;

namespace Client
{

    public class TemplateServiceMock : ITemplate
    {
        
        private List<PlanTemplate> _templates;

        public TemplateServiceMock()
        {
            _templates = new List<PlanTemplate>();
            _templates.Add(CreateTemplate());
        }


        public int GenerateId()
        {
            Random rnd = new Random();
            int id = rnd.Next(1,99999);
            return id;
        }
        
        public Task<Dictionary<int,Goal>> GetGoals()
        {
            throw new NotImplementedException();
        }

        public async Task<PlanTemplate> GetTemplateById(int templateId)
        {
            return (CreateTemplate());
        }

        public async Task<List<PlanTemplate>> GetAllTemplates()
        {
            return _templates;
        }

        public PlanTemplate CreateTemplate()
        {
            var template = new PlanTemplate
            {
                Id = 1,
                Title = "Kokke Uddannelsesplan",
                Description = "Standard",
                CreatedAt = DateTime.Now,
                Forløbs = new List<ForløbTemplate>()
            };

            var introduktion = new ForløbTemplate
                {
                Id = GenerateId(),
                Semester = "År 1",
                Title = "Introduktion",
                Goals = new List<GoalTemplate>
                {
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Udlevering af tøj", Description = "Modtagelse og gennemgang af køkkenuniform" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Faciliteter", Description = "Gennemgang af køkken og personalefaciliteter" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Personlige ejendele", Description = "Håndtering af personlige ejendele på arbejdspladsen" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Ind/ud stempling", Description = "Procedure for ind- og udstemplning" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Rundvisning", Description = "Grundig rundvisning på hele hotellet" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Elevansvarlig og mentor", Description = "Introduktion til elevansvarlig og mentorordning" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Afdelingsledere", Description = "Præsentation af køkkenets ledelsesstruktur" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Sygdom", Description = "Procedure ved sygdom og fraværsmelding" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Comwell Connect", Description = "Introduktion til Comwell Connect-systemet" }
                }
                };

                // 2. Information
                var information = new ForløbTemplate
                {
                Id = GenerateId(),
                Semester = "År 1",
                Title = "Information",
                Goals = new List<GoalTemplate>
                {
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Vagtplaner", Description = "Forståelse og håndtering af vagtplaner" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Ferie", Description = "Procedure for ferieplanlægning" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Medarbejderhåndbog", Description = "Gennemgang af medarbejderhåndbogen" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Koldt/varmt køkken", Description = "Introduktion til koldt og varmt køkken" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Morgen- og aftenvagter", Description = "Rutiner for morgen- og aftenvagter" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Uddannelsesplan", Description = "Gennemgang af den samlede uddannelsesplan" }
                }
                };

                // 3. Sikkerhed og Miljø
                var sikkerhedOgMiljø = new ForløbTemplate
                {
                Id = GenerateId(),
                Semester = "År 1",
                Title = "Sikkerhed og Miljø",
                Goals = new List<GoalTemplate>
                {
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Intro til arbejdsmiljø", Description = "Grundlæggende introduktion til arbejdsmiljø" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Ergonomi", Description = "Ergonomiske principper i køkkenet" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Arbejdsulykker", Description = "Forebyggelse og håndtering af arbejdsulykker" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Sikkerhedsrutiner", Description = "Generelle sikkerhedsrutiner i køkkenet" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Hjertestarter og førstehjælpskasse", Description = "Brug af hjertestarter og førstehjælpskasse" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Sikkerhedsvideo", Description = "Gennemgang af sikkerhedsvideo" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Brand og evakuering", Description = "Procedurer ved brand og evakuering" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Brandslukning", Description = "Grundlæggende brandslukningsteknikker" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Sikkerhedsrutiner", Description = "Detaljeret gennemgang af sikkerhedsrutiner" }
                }
                };

                // 4. Forventningsafstemning
                var forventningsafstemning = new ForløbTemplate
                {
                Id = GenerateId(),
                Semester = "År 1",
                Title = "Forventningsafstemning",
                Goals = new List<GoalTemplate>
                {
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "6 ugers samtale", Description = "Opfølgningssamtale efter 6 uger" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "3 måneders samtale", Description = "Opfølgningssamtale efter 3 måneder" }
                }
                };

                // 5. Kurser
                var kurser = new ForløbTemplate
                {
                Id = GenerateId(),
                Semester = "År 1",
                Title = "Obligatoriske Kurser",
                Goals = new List<GoalTemplate>
                {
                new GoalTemplate { Id = GenerateId(), Type = "Kursus", Title = "Kernen i Comwell", Description = "Grundlæggende kursus om Comwell" },
                new GoalTemplate { Id = GenerateId(), Type = "Kursus", Title = "Bæredygtighed", Description = "Kursus om bæredygtighed i hotelbranchen" },
                new GoalTemplate { Id = GenerateId(), Type = "Kursus", Title = "Intro for elever", Description = "Introduktionskursus for nye elever" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Crosstræning", Description = "Tværfaglig træning i køkkenet" }
                }
                };

                // 6. Faglige Mål
                var fagligeMål = new ForløbTemplate
                {
                Id = GenerateId(),
                Semester = "År 1",
                Title = "Faglige Kompetencer",
                Goals = new List<GoalTemplate>
                {
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Systemer", Description = "Gennemgang af køkkenets systemer" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Knivgennemgang", Description = "Korrekt brug og vedligeholdelse af knive" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Kvalitetskendetegn", Description = "Forståelse af kvalitetsstandarder" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Grovudskæring af kødstykker", Description = "Teknikker til udskæring af kød" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Fonder", Description = "Tilberedning og brug af fonder" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Grundsaucer", Description = "Tilberedning af grundlæggende saucer" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Tilberedningsteknikker", Description = "Forskellige madlavningsteknikker" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Anretninger", Description = "Kreativ og professionel anretning" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Rengøring af værktøj", Description = "Korrekt rengøring af køkkenudstyr" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Forplejningsmuligheder", Description = "Forskellige forplejningsformer" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Konferencemenuer", Description = "Sammensætning af konferencemenuer" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "A la Carte menuer", Description = "Konstruktion af A la Carte menuer" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Selskabsmenuer", Description = "Planlægning af selskabsmenuer" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Barkort", Description = "Sammensætning af barkort" }
                }
                };

                // 7. Madspild og Affaldssortering
                var madspildOgAffaldssortering = new ForløbTemplate
                {
                Id = GenerateId(),
                Semester = "År 1",
                Title = "Bæredygtighed og Miljøhensyn",
                Goals = new List<GoalTemplate>
                {
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Esmiley-system", Description = "Introduktion til Esmiley-systemet" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Affaldssortering i køkkenet", Description = "Korrekt affaldssortering i køkkenområdet" },
                new GoalTemplate { Id = GenerateId(), Type = "Delmål", Title = "Affaldssortering på hotellet", Description = "Overordnet affaldssortering på hele hotellet" }
                }
                };


            
            var år2 = new ForløbTemplate
            {
                Id = GenerateId(),
                Semester = "År 2",
                Title = "Avancerede teknikker og præsentation",
                Goals = new List<GoalTemplate>  
                {
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kursus",
                        Title = "Model 4 - Attachè",
                        Description = "Avancerede teknikker for attachè i køkkenet"
                    },
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kompetence",
                        Title = "Attractiva",
                        Description = "Lær at lave visuelt tiltalende anretninger"
                    }
                }
            };

         
            var år3 = new ForløbTemplate
            {
                Id = GenerateId(),
                Semester = "År 3",
                Title = "Afsluttende projekter og svendeprøve",
                Goals = new List<GoalTemplate>
                {
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kursus",
                        Title = "Introduction til OOPI",
                        Description = "Object-Orienteret Principer i madlavning"
                    },
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Delmål",
                        Title = "Avr.",
                        Description = "Afsluttende projekter og svendeprøve"
                    }
                }
            };

            template.Forløbs.Add(introduktion);
            template.Forløbs.Add(information);
            template.Forløbs.Add(sikkerhedOgMiljø);
            template.Forløbs.Add(forventningsafstemning);
            template.Forløbs.Add(kurser);
            template.Forløbs.Add(fagligeMål);
            template.Forløbs.Add(madspildOgAffaldssortering);
            template.Forløbs.Add(år2);
            template.Forløbs.Add(år3);

            return template;
        }
    }

}