using Core;
using MongoDB.Bson;

namespace Client
{

    public interface ITemplate
    {
        PlanTemplate CreateTemplate();
        Task<List<Goal>> GetGoals();
        Task<PlanTemplate> GetTemplateById(int templateId);
        
    }

}
