using Core;

namespace Client
{

    public class GoalServiceMock : IGoal
    {
        private IElevPlan _elevPlan;

        public GoalServiceMock(IElevPlan elevPlan)
        {
            _elevPlan = elevPlan;
        }
        
        public async Task<Goal> GetGoalByGoalId(int goalId)
        {
            var plan = await _elevPlan.GetAllPlans();
            var goal = plan.SelectMany(f => f.Forløbs).SelectMany(g => g.Goals).FirstOrDefault(f => f.Id == goalId);   
            
            return goal;
        }

        public async Task<List<Goal>> GetAllUncompletedCourses()
        {
            var allePlaner = await _elevPlan.GetAllPlans();

            var alleKurser = allePlaner.SelectMany(p => p.Forløbs)
                .SelectMany(f => f.Goals).Where(g => g.Type == "Kursus" && g.Status == "Active").ToList();
            
            return alleKurser;
        }
        
    }

}