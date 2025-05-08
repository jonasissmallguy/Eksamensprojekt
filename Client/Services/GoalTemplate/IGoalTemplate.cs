using Core;

namespace Client
{

    public interface IGoalTemplate
    {
        Plan CreateTemplate(); //SKAL LAVES ASYNC
        Task<List<Goal>> GetGoals();
        
        
    }

}
