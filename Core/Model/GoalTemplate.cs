namespace Core
{

    public class GoalDefinition
    {
        public string Id { get; set; }
        public string Type { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public int Semester { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int SortOrder { get; set; } 

    }
}