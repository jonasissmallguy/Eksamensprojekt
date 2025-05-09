using Core;

namespace Client
{

    public class CommentServiceMock : IComment
    {
        private IGoal _goalService;
        private IElevPlan _elevPlanService;

        public CommentServiceMock(IGoal goalService, IElevPlan elevPlanService)
        {
            _goalService = goalService;
            _elevPlanService = elevPlanService;
        }
        
        //Kan dette laves bedre? Ja formentlig hvis jeg passer forløb id på comment samt goal id...
        //Vi mangler navn på person.. 
                public async Task AddComment(NewComment comment, BrugerLoginDTO brugerLoginDTO)
                {
                    
                    var allPlans = await _elevPlanService.GetAllPlans();
                    
                    //fuldstændig unødevendig i live...
                    var goal = allPlans.SelectMany(plan => plan.Forløbs).SelectMany(goal => goal.Goals).FirstOrDefault(id => id.Id == comment.GoalId);
                   //var goal =  await  _goalService.GetGoalByGoalId(comment.GoalId); //skal bruges ved skift..
        
                   if (goal == null)
                   {
                       return;
                   }
        
                   //Hivs ingen kommentar på dette goal, så initialiserer vi en tom liste af kommentar, tror ikke nødvendig i rigtig version
                   if (goal.Comments == null)
                   {
                       goal.Comments = new List<Comment>();
                   }
                   
                   //Random commentId.. simulerer..
                   Random random = new Random();
                   var commentId = random.Next(1,9999);
                   
                   //Tilføjer kommentar til goal

           goal.Comments.Add(new Comment
           {
               Id = commentId,
               CreatorId = brugerLoginDTO.Id,
               Text = comment.Comment,
               CreatorName = "TESTER"
           });
           
           Console.WriteLine($"Comment Id: {commentId}");
            
        }

        public async Task DeleteComment()
        {
            throw new NotImplementedException();
        }
    }

}