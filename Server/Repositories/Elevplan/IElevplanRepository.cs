using MongoDB.Driver;
using Core;

namespace Server
{

    public interface IElevplanRepository
    {
        //Gemmer en elevplan efter studentId = _id
        Task<UpdateResult> SaveElevplan(int studentId, Plan plan);
        
        //Retunerer en elevplan efter studentId = _id
        Task<Plan> GetPlanByStudentId(int studentId);

    }

}