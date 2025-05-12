using Core;
using MongoDB.Bson;

namespace Client
{

    public class TemplateServiceMock : ITemplate
    {


        public int GenerateId()
        {
            Random rnd = new Random();
            int id = rnd.Next(1,99999);
            return id;
        }
        
        public Task<List<Goal>> GetGoals()
        {
            //Ryk logik her
            throw new NotImplementedException();
        }

        public Task<PlanTemplate> GetTemplateById(int templateId)
        {
            return Task.FromResult(CreateTemplate());
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

            var år1 = new ForløbTemplate
            {
                Id = GenerateId(),
                Semester = 1,
                Title = "Introduktion",
                Goals = new List<GoalTemplate>
                {
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kompetence",
                        Title = "Udlevering af tøj",
                        Description = ""
                        
                    },
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kompetence",
                        Title = "Personalelokaler",
                        Description = ""
                    },
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kompetence",
                        Title = "Personlige ejendele",
                        Description = ""
                    },
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kompetence",
                        Title = "Ind/ud stempling",
                        Description = ""
                    },
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kompetence",
                        Title = "Rundvisning",
                        Description = ""
                    },
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kompetence",
                        Title = "Hotellets ledelse",
                        Description = ""
                    },
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kompetence",
                        Title = "Sygdom",
                        Description = ""
                    },
                    new GoalTemplate
                    {
                        Id = GenerateId(),
                        Type = "Kompetence",
                        Title = "Comwell Connect",
                        Description = ""
                    }
                }
            };
            
            var år2 = new ForløbTemplate
            {
                Id = GenerateId(),
                Semester = 2,
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
                Semester = 3,
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

            template.Forløbs.Add(år1);
            template.Forløbs.Add(år2);
            template.Forløbs.Add(år3);

            return template;
        }
        }
    

}