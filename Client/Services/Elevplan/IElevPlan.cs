using Core;

namespace Client
{

    public interface IElevPlan
    {
    
        Task<Plan> GetElevPlanTemplate(int studentId);
        Task<bool> SaveElevPlan(Plan plan, int studentId);
    }
}