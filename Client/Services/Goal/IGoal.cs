using Client.Components.Elevoversigt;
using Core;


namespace Client
{

    public interface IGoal
    {
        
        //Goals
        Task DeleteGoal(Goal goal, int studentId);
        Task<List<Goal>> CreateGoalsForTemplate(int planId, Forløb forløbs, List<GoalTemplate> goalTemplates);
        
        //Goal progress
        Task<Goal> StartGoal(ElevplanComponent.MentorAssignment mentor); //goal.status -> InProgress
        Task<Goal> ProcessGoal(ElevplanComponent.MentorAssignment bruger); //goal.status -> AwaitingApproval
        Task<Goal> ConfirmGoal(ElevplanComponent.MentorAssignment leder); //goal.status -> Finished
        
        //Comments
        Task<Comment> AddComment(NewComment comment);
        Task DeleteComment(int goalId, int commentId);
        
        Task<List<GoalNameDTO>> GetAllGoalTypes();

        Task<List<Goal>> GetAwaitingApproval();
        Task<List<Goal>> GetMissingCourses(User bruger);
        Task<List<Goal>> GetOutOfHouse();
        Task ConfirmGoalFromHomePage(Goal goal);
        Task<List<Goal>> GetAllGoalsForBruger(User bruger);
        Task<List<Goal>> GetGoalsByTypeForUser(User bruger, string kursus);
    }

}