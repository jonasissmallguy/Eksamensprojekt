using Client;
using Client.Components.Elevoversigt;
using Core;

namespace Server
{

    public interface IGoalRepository 
    {
        
        Task<bool> DeleteGoal(int studentId, int planId, int forløbId, int goalId);

        Task<bool> AddComment(Comment comment);
        
        Task StartGoal(ElevplanComponent mentor);

    }

}