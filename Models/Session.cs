namespace reminder_schedule_backend.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
    }
}
