namespace Server
{

    public interface IGoalRepository 
    {
        
        Task<bool> DeleteGoal(int planId, int studentId, int goalId);

    }

}