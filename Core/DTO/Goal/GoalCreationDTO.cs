namespace Core.DTO.Goal
{

    public class GoalCreationDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } // Kursus, delmål...
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Active";
        public string Semester { get; set; }
        public int? StarterId { get; set; }
        public string StarterName { get; set; } = string.Empty;
        public DateTime DeadLineAt { get; set; }
        public DateOnly? SkoleStart { get; set; }
        public DateOnly? SkoleEnd { get; set; }

    }


}