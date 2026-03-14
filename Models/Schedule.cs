namespace reminder_schedule_backend.Models
{
    public class Schedule
    {
        public int id { get; set; }

        public DayOfWeek day{ get; set; }

        public int? classId { get; set; }
        public Class Class { get; set; } = null!;

        public int subjectId { get; set; }
        public Subject subject { get; set; } = null!; 

        public int sessionId { get; set; }
        public Session session { get; set; } = null!;
    }
}
