using MongoDB.Bson;

namespace Client
{
    public class NewComment
    {
        
        public int PlanId { get; set; }
        
        public int ForløbId { get; set; }
        public int GoalId { get; set; }
        public string Comment { get; set; }
        
        public int CommentorId { get; set; }
        public string CommentName { get; set; }

    }

}