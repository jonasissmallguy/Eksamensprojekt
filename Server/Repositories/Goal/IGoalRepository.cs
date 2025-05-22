using Client;
using Client.Components.Elevoversigt;
using Core;
using Core.DTO.Goal;

namespace Server
{

    public interface IGoalRepository
    {
        

        Task<bool> DeleteGoal(int studentId, int planId, int forløbId, int goalId);
        Task<bool> AddGoal(int studentId, int planId, int forløbId, Goal newGoal);
        Task<Comment> AddComment(Comment comment);
        
        //Goals progress
        Task<Goal> StartGoal(ElevplanComponent.MentorAssignment mentor); //kan dette laves om?
        Task<Goal> ProcessGoal(ElevplanComponent.MentorAssignment mentor); //kan dette laves om?
        Task<Goal> ConfirmGoalHelper(int planId, int forløbId, int goalId);
        Task<Goal> ConfirmGoalAndHandleProgress(int planId, int forløbId, int goalId);
        
        Task UpdateForløbStatus(int planId, int forløbId);
        
        //School
        Task<Goal> UpdateSchoolStatus(int planId, int forløbId, int goalId);
        Task UpdateYearStauts(int planId);
        
        //Home
        Task<List<Goal>> GetFutureSchools(int elevId);
        Task<List<User>> GetActionGoals(int elevId);
        Task<List<User>> GetAwaitingApproval(int hotelId);
        Task<List<User>> GetMissingCourses(int hotelId);
        Task<List<User>> GetOutOfHouse(int hotelId);
        Task<List<User>> GetStartedGoals(int hotelId);
    }



}