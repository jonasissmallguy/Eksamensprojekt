using Core;
using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson;

namespace Server
{
    public class GoalRepository : IGoalRepository
    {
        public async Task<bool> DeleteGoal(int planId, int studentId, int goalId)
        {
            return false;
        }
    }
}