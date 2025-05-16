using Core;

namespace Server
{

    public interface ITemplateRepository
    {
        Task<PlanTemplate> GetPlanTemplate(int id);
        Task<List<PlanTemplate>> GetAllPlanTemplates();

    }

}