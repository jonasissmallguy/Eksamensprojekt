using Client;

namespace Server
{

    public interface IGoalRepository 
    {
        
        Task<bool> DeleteGoal(int studentId, int planId, int forløbId, int goalId);

        Task<bool> AddComment(NewComment comment);

    }

}