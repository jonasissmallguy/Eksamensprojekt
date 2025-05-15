using MongoDB.Driver;
using Core;

namespace Server
{

    public interface IElevplan
    {
        
        Task<UpdateResult> SaveElevplan(int studentId, Plan plan);
        
        Task<Plan> GetPlanByStudentId(int studentId);

    }

}