using Core;

namespace Client
{

    public interface IElevPlan
    {
    
        Task<Plan> CreateElevPlan(int studentId);
    }
}