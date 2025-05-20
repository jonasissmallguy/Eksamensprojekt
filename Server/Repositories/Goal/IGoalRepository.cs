using Client;
using Client.Components.Elevoversigt;
using Core;

namespace Server
{

    public interface IGoalRepository
    {
        /// <summary>
        /// Sletter et mål for en given elev og plan.
        /// </summary>
        Task<bool> DeleteGoal(int studentId, int planId, int forløbId, int goalId);

        /// <summary>
        /// Tilføjer en kommentar til et mål.
        /// </summary>
        Task<Comment> AddComment(Comment comment);

        
        Task<Goal> StartGoal(ElevplanComponent.MentorAssignment mentor);

        Task<Goal> ProcessGoal(ElevplanComponent.MentorAssignment mentor);
        
        Task<Goal> ConfirmGoal(ElevplanComponent.MentorAssignment mentor);
        
        
        /// <summary>
        /// Henter mål der venter på godkendelse.
        /// </summary>
        Task<List<Goal>> GetAwaitingApproval();

        /// <summary>
        /// Henter manglende kurser for en bruger.
        /// </summary>
        Task<List<Goal>> GetMissingCourses(int userId);

        /// <summary>
        /// Henter mål af typen 'Skole' (out of house).
        /// </summary>
        Task<List<Goal>> GetOutOfHouse();

        /// <summary>
        /// Bekræfter et mål via forsiden.
        /// </summary>
        Task<bool> ConfirmGoalFromHomePage(Goal goal);

        /// <summary>
        /// Henter aktive mål for en bruger og type.
        /// </summary>
        Task<List<Goal>> GetGoalsByTypeForUser(string type, int userId);

        /// <summary>
        /// Henter alle målnavne / typer.
        /// </summary>
        Task<List<string>> GetAllGoalTypes();
        
        /// <summary>
        /// Henter alle mål for en given bruger.
        /// </summary>
        Task<List<Goal>> GetAllGoalsForUser(int userId);
    }



}