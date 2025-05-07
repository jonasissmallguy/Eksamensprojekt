using Core;

namespace Client
{

    public interface IElevPlan
    {
        Task<Plan> CreatePlanFromTemplate(int studentId);
        Task SavePlan(Plan plan);
        Task<List<Plan>> GetAllPlans();
        Task<Plan> GetPlanByStudentId(int studentId);
        Task AddComment();

    }

}