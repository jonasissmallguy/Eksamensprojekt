using Client;
using Client.Components.Elevoversigt;
using Core;

namespace Server
{

    public interface IGoalRepository
    {
        Task<bool> DeleteGoal(int studentId, int planId, int forløbId, int goalId);
        Task<Comment> AddComment(Comment comment);
        Task<Goal> StartGoal(ElevplanComponent.MentorAssignment mentor);
        Task<Goal> ProcessGoal(ElevplanComponent.MentorAssignment mentor);
        Task<Goal> ConfirmGoal(ElevplanComponent.MentorAssignment mentor);

        Task<List<User>> GetActionGoals(int elevId);
        Task<List<User>> GetAwaitingApproval(int hotelId);
        Task<List<User>> GetMissingCourses(int hotelId);
        Task<List<User>> GetOutOfHouse(int hotelId);
        
        
        Task<bool> ConfirmGoalFromHomePage(int planId, int forløbId, int goalId);
        Task<List<Goal>> GetGoalsByTypeForUser(string type, int userId);
        
        Task<List<string>> GetAllGoalTypes();
        Task<List<Goal>> GetAllGoalsForUser(int userId);
    }



}