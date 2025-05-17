using Client;
using Client.Components.Elevoversigt;
using Core;

namespace Server
{

    public interface IGoalRepository 
    {
        
        Task<bool> DeleteGoal(int studentId, int planId, int forløbId, int goalId);

        Task<bool> AddComment(Comment comment);
        
        Task StartGoal(ElevplanComponent mentor);

        Task<List<Goal>> GetAwaitingApproval();
        Task<List<Goal>> GetMissingCourses(int userId);
        Task<List<Goal>> GetOutOfHouse();
        Task<bool> ConfirmGoalFromHomePage(Goal goal);

        Task<List<Goal>> GetGoalsByTypeForUser(string type, int userId);
    }

}