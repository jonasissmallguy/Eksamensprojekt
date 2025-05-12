using Client.Components.Elevoversigt;
using Core;
using MongoDB.Bson;


namespace Client
{

    public interface IGoal
    {
        
        //Goals
        Task<Goal> GetGoalByGoalId(int goalId);
        Task<Dictionary<int, Goal>> GetAllGoalsByPlanId(int planId);
        Task<List<Goal>> GetAllUncompletedCourses();
        Task DeleteGoal(Goal goal);
        Task<List<Goal>> CreateGoalsForTemplate(int planId, Forløb forløbs, List<GoalTemplate> goalTemplates);
        
        Task ConfirmGoal(int goalId);
        
        //Mentor
        Task AddMentorToGoal(ElevplanComponent.MentorAssignment mentor);
        Task RemoveMentorFromGoal(int goalId);
        
        //Comments
        Task AddComment(NewComment comment, BrugerLoginDTO currentUser);
        Task DeleteComment(int goalId, int commentId);
        
        Task<List<GoalNameDTO>> GetAllGoalTypes();
        
    }

}