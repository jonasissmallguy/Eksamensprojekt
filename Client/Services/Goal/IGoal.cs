using Client.Components.Elevoversigt;
using Core;


namespace Client
{

    public interface IGoal
    {
        
        //Goals
        Task<Goal> GetGoalByGoalId(int goalId);
        Task<Dictionary<int, Goal>> GetAllGoalsByPlanId(int planId);
        Task<List<Goal>> GetAllUncompletedCourses();
        Task<List<User>> GetUsersByGoalId(int goalId);
        Task DeleteGoal(Goal goal);
        Task<List<Goal>> CreateGoalsForTemplate(int planId, Forløb forløbs, List<GoalTemplate> goalTemplates);

        Task<List<Goal>> GetAwaitingApproval();
        
        //Goal progress
        Task StartGoal(ElevplanComponent.MentorAssignment mentor); //goal.status -> InProgress //ikke specifik!
        Task ProcessGoal(ElevplanComponent.MentorAssignment bruger); //goal.status -> AwaitingApproval //ikke specifik!
        Task ConfirmGoal(ElevplanComponent.MentorAssignment leder); //goal.status -> Finished//ikke specifik! 
        
        //
        Task ConfirmGoalFromHomePage(Goal goal); //ikke specifik!
        Task<List<Goal>> GetMissingCourses(); //ikke specifik!
        Task<List<Goal>> GetOutOfHouse(); //ikke specifikt!
        
        //Comments
        Task AddComment(NewComment comment, BrugerLoginDTO currentUser);
        Task DeleteComment(int goalId, int commentId);
        
        Task<List<GoalNameDTO>> GetAllGoalTypes();
        
    }

}