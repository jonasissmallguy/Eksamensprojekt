using Client;
using Client.Components.Elevoversigt;
using Core;
using Core.DTO.Goal;

namespace Server
{

    public interface IGoalRepository
    {
        
        /// <summary>
        /// Sletter et mål for en given elev og plan.
        /// </summary>
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
        
        Task<List<User>> GetStartedGoals(int hotelId);
        
        Task<List<Goal>> GetGoalsByTypeForUser(string type, int userId); //Slet?
        
        Task<List<string>> GetAllGoalTypes(); //Slet?
        Task<List<Goal>> GetAllGoalsForUser(int userId); //Slet?
    }



}