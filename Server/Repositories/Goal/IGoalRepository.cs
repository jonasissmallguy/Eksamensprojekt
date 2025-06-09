using Client;
using Client.Components.Elevoversigt;
using Core;
using MongoDB.Bson;

namespace Server
{

    public interface IGoalRepository
    {
        
        //Sletter et delmål efter studentId, forløbId og goalId
        Task<bool> DeleteGoal(int studentId, int forløbId, int goalId);
        //Tilføjer et nyt delmål til en elevplan efter studentId, forløbId
        Task<bool> AddGoal(int studentId, int forløbId, Goal newGoal);
        //Tilføjer en ny kommentar
        Task<Comment> AddComment(Comment comment);
        //Opdater et skoleophold og sætter Skolenavn, StartDate, EndDate og Status = InProgress
        Task<bool> UpdateSchoolWithDate(Goal goal, int studentId);
        //Opdater Status = InProgress, Sætter StarterId, StarterName, StartedAt
        Task<Goal> StartGoal(MentorAssignment mentor); 
        //Opdater Status = "AwaitingApproval", sætter ConfirmedId, ConfirmerName, ConfirmedAt
        Task<Goal> ProcessGoal(MentorAssignment mentor); 
        //Hovedmetode der kalder CompleteGoal og UpdateForløbStatus
        Task<Goal> ConfirmGoalAndHandleProgress(int planId, int forløbId, int goalId);
        //Opdater Status = "Completed" og sætter CompletedAt
        Task<Goal> CompleteGoal(int planId, int forløbId, int goalId);
        //Opdater Forløbets Status = Completed, hvis alle mål har Status = Completed
        Task UpdateForløbStatus(int planId, int forløbId);
        //Retunerer alle forløb, hvor studentId = Id
        Task<List<Forløb>> GetGoalsByStudentId(int studentId);
        //Opdater skoleophold og sætter Status = Completed og CompletedAt
        Task<Goal> UpdateSchoolStatus(int planId, int forløbId, int goalId);
        //Opdater en elevperiode til næste periode 
        Task UpdateYearStatus(int planId);
        //Tilføjer en elev til et kursus i elevplanen, hvor studentId = _id, Status = InProgress, StartDate og EndDate
        Task<bool> AddStudentToACourse(int studentId, Kursus kursus);
        //Færdiggør alle elever med kursusCode = CourseCode og alle elever i listen studentIds
        Task<bool> CompleteAllStudentsOnCourse(List<int> studentIds, string kursusCode);
        //Fjerner et kursus fra en elevplan, hvor kursusCode = CourseCode og studentId = _id
        Task<bool> RemoveStudentFromACourse(int studentId, string kursusCode);
        //Retunerer alle mål hvor type = Skoleophold efter elevId = Id
        Task<List<Goal>> GetFutureSchools(int elevId);
        //Retunerer en liste af brugere, hvor type = Delmål og Status er InProgress, AwaitingApproval
        Task<List<BsonDocument>> GetActionGoals(int elevId);
        //Retunerer en liste af brugere hvor Status = AwaitingApproval efter hotelId = HotelId
        Task<List<BsonDocument>> GetAwaitingApproval(int hotelId);
        //Retunerer en liste af brugere hvor type = Kursus og Status = Active efter hotelId = HotelId
        Task<List<BsonDocument>> GetMissingCourses(int hotelId);
        //Retunerer en liste af brugere hvor type er Kursus eller Skoleophold, Status = InProgress og StartDate er 1 fra dd.
        Task<List<BsonDocument>> GetOutOfHouse(int hotelId);
        //Retunerer en liste af mål hvor type = Delmål og Status = InProgress
        Task<List<BsonDocument>> GetStartedGoals(int hotelId);  
        //Retunerer alle elever hvor kursusCode = CourseCode, Type = Kursus og Status = Active
        Task<List<User>> GetAllStudentsMissingCourse(string kursusCode);
    }



}