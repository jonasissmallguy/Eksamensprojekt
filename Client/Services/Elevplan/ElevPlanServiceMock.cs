using Core;

namespace Client
{

    public class ElevPlanServiceMock : IElevPlan
    {
        private List<Plan> allePlaner = new();

        private IGoalTemplate _goalTemplate;
        
        public ElevPlanServiceMock(IGoalTemplate goalTemplate)
        {
            _goalTemplate = goalTemplate;
            
            Plan nyplan =  _goalTemplate.CreateTemplate();
            allePlaner.Add(nyplan);
        }
        
        public async Task SavePlan(Plan plan)
        {
            allePlaner.Add(plan);    
        }

        public async Task<List<Plan>> GetAllPlans()
        {
            return allePlaner;
        }

        public async Task<Plan> GetPlanByStudentId(int studentId)
        {
            var plan = allePlaner.FirstOrDefault(x => x.StudentId == studentId);
            return plan;
        }
    }
}