namespace Core
{
    public class Comment
    { 
        public int Id { get; set; }
        public string Text { get; set; }
        public int CreatorId { get; set; }
        public string CreatorName { get; set; }
    }
}