using Client.Components.Elevoversigt;
using Core;


namespace Client
{

    public interface IGoal
    {
        
        //Goals
        Task DeleteGoal(Goal goal, int studentId);
        Task<bool> AddGoal(Goal goal, int studentId);
        Task<List<Goal>> CreateGoalsForTemplate(int planId, Forløb forløbs, List<GoalTemplate> goalTemplates); //denne skal ikke bruges
        
        //Goal progress 
        Task<Goal> StartGoal(ElevplanComponent.MentorAssignment mentor); //goal.status -> InProgress //Kan de her laves om?
        Task<Goal> ProcessGoal(ElevplanComponent.MentorAssignment bruger); //goal.status -> AwaitingApproval //Kan de her laves om?
        Task<Goal> ConfirmGoal(ElevplanComponent.MentorAssignment leder); //goal.status -> Finished //Kan de her laves om?
         
        //Comments
        Task<Comment> AddComment(NewComment comment);
        Task DeleteComment(int goalId, int commentId); //denne skal ikke bruges
        
        Task<List<GoalNameDTO>> GetAllGoalTypes(); //denne skal ikke bruges

        Task<List<StartedGoalsDTO>> GetAwaitingApproval(int hotelId); //done
        Task<List<KursusManglendeDTO>> GetMissingCourses(int hotelId); //done
        Task<List<GoalNeedActionDTO>> GetNeedActionGoals(int elevId); //done
        Task<List<FutureSchoolDTO>> GetFutureSchools(int elevId);
        Task<List<OutOfHouseDTO>> GetOutOfHouse(int hotelId); //mangler... skole + kursus,
        Task<bool> ConfirmGoalFromHomePage(StartedGoalsDTO goalDto); //done
        Task<List<StartedGoalsDTO>> GetStartedGoals(int hotelId); 
        
        
        
        Task<List<Goal>> GetAllGoalsForBruger(User bruger); //denne bruges ikke?
        Task<List<Goal>> GetGoalsByTypeForUser(User bruger, string kursus); //denne bruges ikke?
        
    }

}