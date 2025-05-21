using Core;
using MongoDB.Bson;

namespace Client
{

    public class ElevPlanServiceMock : IElevPlan
    {
        private List<Plan> _allePlaner = new();
        private ITemplate _template;
        private IGoal _goal;
        private IBruger _user;
        
        public ElevPlanServiceMock(ITemplate template, IGoal goal, IBruger user)
        {
            _template = template;
            _goal = goal;
            _user = user;
        }
        

        public int GenerateId()
        {
            Random rnd = new();
            int id = rnd.Next(1,99999);
            return id;
        }

        public async Task<Plan> CreateElevPlan(int studentId)
        {
            var template = await _template.GetTemplateById(1);

            var nyPlan = new Plan
            {
                Id = GenerateId(),
                StudentId = studentId,
                Title = template.Title,
                Description = "test" + "s " + "Udannelseplan",
                CreatedAt = DateTime.Now,
                Forløbs = new List<Forløb>()
            };

            foreach (var forløbTemplate in template.Forløbs)
            {
                var forløb = new Forløb
                {
                    Id = GenerateId(),
                    Title = forløbTemplate.Title,
                    Semester = forløbTemplate.Semester,
                    //StartDate = DateOnly.MaxValue, 
                    Goals = new List<Goal>() 
                };
        
                //Opretter goals
                await _goal.CreateGoalsForTemplate(nyPlan.Id, forløb, forløbTemplate.Goals);
                nyPlan.Forløbs.Add(forløb);
            }
            
            await _user.SaveStudentPlan(studentId, nyPlan);
            
            return nyPlan;
        }

        public async Task SavePlan(Plan plan)
        {
            await _user.SaveStudentPlan(plan.StudentId, plan);
        }
        

        public async Task<Plan> GetPlanByStudentId(int studentId)
        {
            var user = await _user.GetBrugerById(studentId); 
            return user.ElevPlan;
        }
    }
}