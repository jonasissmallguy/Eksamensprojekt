namespace Core
{

    public class KursusKommendeDTO
    {
        
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }    
        public DateOnly? StartDate { get; set; }
        
        public int Participants { get; set; }
        public int MaxParticipants { get; set; }
        
    }
    
}