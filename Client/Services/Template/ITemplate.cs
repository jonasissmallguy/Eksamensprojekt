using Core;
using MongoDB.Bson;

namespace Client
{

    public interface ITemplate
    {
        PlanTemplate CreateTemplate();
        Task<Dictionary<int,Goal>> GetGoals();
        Task<PlanTemplate> GetTemplateById(int templateId);
        
        Task<List<PlanTemplate>> GetAllTemplates();
        
    }

}
