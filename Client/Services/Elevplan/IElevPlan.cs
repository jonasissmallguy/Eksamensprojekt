using Core;

namespace Client
{

    public interface IElevPlan
    {
        Task SavePlan(Plan plan);
        Task<Plan> GetPlanByStudentId(int studentId);
        Task<List<Plan>> GetAllPlans(); //Used in mock to find the right plan.. maybe not nessecarry later on?
    }
}