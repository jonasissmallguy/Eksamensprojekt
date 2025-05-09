using Core;


namespace Client
{

    public interface IGoal
    {
        Task<Goal> GetGoalByGoalId(int goalId);

        Task<List<Goal>> GetAllUncompletedCourses();
        
    }

}