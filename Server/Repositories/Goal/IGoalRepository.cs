using Client;
using Client.Components.Elevoversigt;
using Core;

namespace Server
{

    public interface IGoalRepository
    {
        Task<bool> DeleteGoal(int studentId, int planId, int forløbId, int goalId);
        Task<bool> AddGoal(int studentId, int planId, int forløbId, Goal newGoal);
        Task<Comment> AddComment(Comment comment);
        Task<bool> UpdateSchoolWithDate(Goal goal, int studentId);
        
        //Goals progress
        Task<Goal> StartGoal(MentorAssignment mentor); 
        Task<Goal> ProcessGoal(MentorAssignment mentor); 
        Task<Goal> ConfirmGoalHelper(int planId, int forløbId, int goalId);
        Task<Goal> ConfirmGoalAndHandleProgress(int planId, int forløbId, int goalId);
        
        Task<List<Forløb>> GetGoalsByStudentId(int studentId);
        
        //Forløb
        Task UpdateForløbStatus(int planId, int forløbId);
        
   
        Task<Goal> UpdateSchoolStatus(int planId, int forløbId, int goalId);
        Task UpdateYearStauts(int planId);
        
        //Kursus
        Task<bool> AddStudentToACourse(int studentId, Kursus kursus);
        Task<bool> CompleteAllStudentsOnCourse(List<int> studentIds, string kursusCode);
        Task<bool> RemoveStudentFromACourse(int studentId, string kursusCode);
        
        //Home
        Task<List<Goal>> GetFutureSchools(int elevId);
        Task<List<User>> GetActionGoals(int elevId);
        Task<List<User>> GetAwaitingApproval(int hotelId);
        Task<List<User>> GetMissingCourses(int hotelId);
        Task<List<User>> GetOutOfHouse(int hotelId);
        Task<List<User>> GetStartedGoals(int hotelId);
    }



}