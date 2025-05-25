namespace Core
{

    public class GoalCreationDTO
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public int ForløbId { get; set; }
        public string Type { get; set; } // Kursus, delmål...
        public string Title { get; set; }
        public string Description { get; set; }
        
        public string Status { get; set; } = "Active";
        public string SkoleNavn { get; set; }
        public DateTime? SkoleStart { get; set; }
        public DateTime? SkoleEnd { get; set; }

    }


}