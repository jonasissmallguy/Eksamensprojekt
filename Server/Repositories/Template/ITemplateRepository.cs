using Core;

namespace Server
{

    public interface ITemplateRepository
    {
        //Retunerer en PlanTemplate efter id
        Task<PlanTemplate> GetPlanTemplate(int id);
        //Retunerer alle PlanTemplates
        Task<List<PlanTemplate>> GetAllPlanTemplates();

    }

}