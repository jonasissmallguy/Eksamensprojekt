using Client.Components.Elevoversigt;
using Core;


namespace Client
{

    public interface IGoal
    {
        
        //Goals
        Task DeleteGoal(Goal goal, int studentId);
        Task<bool> AddGoal(Goal goal, int studentId);
        Task<bool> UpdateSkole(Goal goal, int studentId);
        
        //Goal progress 
        Task<Goal> StartGoal(MentorAssignment mentor); 
        Task<Goal> ProcessGoal(MentorAssignment bruger); 
        Task<Goal> ConfirmGoal(int planId, int forløbId, int goalId);
        Task<Goal> ConfirmSchool(int planId, int forløbId, int goalId);
        Task<List<GoalProgessDTO>> GoalProgess(int studentId);
        
        //Comments
        Task<Comment> AddComment(NewComment comment);
        
        Task<List<StartedGoalsDTO>> GetAwaitingApproval(int? hotelId); 
        Task<List<KursusManglendeDTO>> GetMissingCourses(int? hotelId); 
        Task<List<GoalNeedActionDTO>> GetNeedActionGoals(int elevId); 
        Task<List<FutureSchoolDTO>> GetFutureSchools(int? elevId);
        Task<List<OutOfHouseDTO>> GetOutOfHouse(int? hotelId); 
        Task<List<StartedGoalsDTO>> GetStartedGoals(int? hotelId); 
        
    }

}