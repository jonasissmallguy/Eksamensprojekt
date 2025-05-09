using Core;

namespace Client
{

    public class GoalTemplateServiceMock : IGoalTemplate
    {
        
        
        public Task<List<Goal>> GetGoals()
        {
            //Ryk logik her
            
            throw new NotImplementedException();
        }

        public Plan CreateTemplate()
        {
            var template = new Plan
            {
                Id = 9,
                Title = "Kokke Uddannelsesplan",
                StudentId = 2,
                Description = "Standard uddannelsesplan for kokkeelever ved Comwell",
                CreatedAt = DateTime.Now.AddDays(-30),
                Forløbs = new List<Forløb>()
            };

            // År 1 - Introduktion
            var år1 = new Forløb
            {
                Id = 1,
                Semester = "År 1",
                Title = "Introduktion og arbejdspladskultur",
                StartDate = new DateOnly(DateTime.Now.Year, 1, 27),
                Goals = new List<Goal>
                {
                    new Goal
                    {
                        Id = 101,
                        Type = "Kursus",
                        Title = "Velkommen til og introduktion til kollegaer",
                        Description = "Introduktion til arbejdspladsen og møde med kollegaer",
                        Status = "Template",
                        Semester = 1,
                        SortOrder = 1,
                        DeadLineAt = DateTime.Now.AddDays(30)
                    },
                    new Goal
                    {
                        Id = 102,
                        Type = "Kompetence",
                        Title = "Neuroinstructure",
                        Description = "Grundlæggende forståelse af neurogastronomi",
                        Status = "Template",
                        Semester = 1,
                        SortOrder = 2,
                        DeadLineAt = DateTime.Now.AddDays(60)
                    }
                }
            };

            // År 2 - Avancerede teknikker
            var år2 = new Forløb
            {
                Id = 2,
                Semester = "År 2",
                Title = "Avancerede teknikker og præsentation",
                StartDate = new DateOnly(DateTime.Now.Year + 1, 1, 15),
                Goals = new List<Goal>
                {
                    new Goal
                    {
                        Id = 201,
                        Type = "Kursus",
                        Title = "Model 4 - Attachè",
                        Description = "Avancerede teknikker for attachè i køkkenet",
                        Status = "Active",
                        Semester = 2,
                        SortOrder = 1,
                        DeadLineAt = DateTime.Now.AddDays(365)
                    },
                    new Goal
                    {
                        Id = 202,
                        Type = "Kompetence",
                        Title = "Attractiva",
                        Description = "Lær at lave visuelt tiltalende anretninger",
                        Status = "Template",
                        Semester = 2,
                        SortOrder = 2,
                        DeadLineAt = DateTime.Now.AddDays(395)
                    }
                }
            };

            // År 3 - Afslutning og svendeprøve
            var år3 = new Forløb
            {
                Id = 3,
                Semester = "År 3",
                Title = "Afsluttende projekter og svendeprøve",
                StartDate = new DateOnly(DateTime.Now.Year + 2, 2, 10),
                Goals = new List<Goal>
                {
                    new Goal
                    {
                        Id = 301,
                        Type = "Kursus",
                        Title = "Introduction til OOPI",
                        Description = "Object-Orienteret Principer i madlavning",
                        Status = "Active",
                        Semester = 3,
                        SortOrder = 1,
                        DeadLineAt = DateTime.Now.AddDays(730)
                    },
                    new Goal
                    {
                        Id = 302,
                        Type = "Delmål",
                        Title = "Avr.",
                        Description = "Afsluttende projekter og svendeprøve",
                        Status = "Template",
                        Semester = 3,
                        SortOrder = 2,
                        DeadLineAt = DateTime.Now.AddDays(760)
                    }
                }
            };

            template.Forløbs.Add(år1);
            template.Forløbs.Add(år2);
            template.Forløbs.Add(år3);

            return template;
        }
        }
    

}