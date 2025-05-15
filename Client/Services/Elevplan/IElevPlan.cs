using Core;

namespace Client
{

    public interface IElevPlan
    {
        //Plan
        Task<Plan> CreateElevPlan(int studentId);
        Task SavePlan(Plan plan);
        Task<Plan> GetPlanByStudentId(int studentId);
    }
}