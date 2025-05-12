using Core;
using MongoDB.Bson;

namespace Client
{

    public class ElevPlanServiceMock : IElevPlan
    {
        private List<Plan> _allePlaner = new();
        private ITemplate _template;
        private IGoal _goal;
        
        public ElevPlanServiceMock(ITemplate template, IGoal goal)
        {
            _template = template;
            _goal = goal;
        }
        

        public int GenerateId()
        {
            Random rnd = new();
            int id = rnd.Next(1,99999);
            return id;
        }

        public async Task<Plan> CreateElevPlan(int studentId)
        {
            var template =  await _template.GetTemplateById(1);

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
                    StartDate = DateOnly.MaxValue, //denne???
                    GoalIds = new List<int>()
                };
                
                //Opretter goals
                await _goal.CreateGoalsForTemplate(nyPlan.Id, forløb, forløbTemplate.Goals);
                nyPlan.Forløbs.Add(forløb);
            }
            return nyPlan;
        }

        public async Task SavePlan(Plan plan)
        {
            _allePlaner.Add(plan);    
        }

        public async Task<List<Plan>> GetAllPlans()
        {
            return _allePlaner;
        }

        public async Task RemoveGoalIdFromForløb(Goal goal)
        {
            var plan = _allePlaner.FirstOrDefault(p => p.Id == goal.PlanId);
            var forløb = plan.Forløbs.FirstOrDefault(f => f.Id == goal.ForløbId);
            forløb.GoalIds.Remove(goal.Id);
            
        }

        public async Task<Plan> GetPlanByStudentId(int studentId)
        {
            var plan = _allePlaner.FirstOrDefault(x => x.StudentId == studentId);
            return plan;
        }
    }
}