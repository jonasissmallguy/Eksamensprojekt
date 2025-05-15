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
        Task StartGoal(ElevplanComponent.MentorAssignment mentor); //goal.status -> InProgress
        Task ProcessGoal(ElevplanComponent.MentorAssignment bruger); //goal.status -> AwaitingApproval
        Task ConfirmGoal(ElevplanComponent.MentorAssignment leder); //goal.status -> Finished
        
        //Comments
        Task AddComment(NewComment comment, BrugerLoginDTO currentUser);
        Task DeleteComment(int goalId, int commentId);
        
        Task<List<GoalNameDTO>> GetAllGoalTypes();
        
    }

}