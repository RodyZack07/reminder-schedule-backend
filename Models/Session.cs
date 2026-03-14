namespace reminder_schedule_backend.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan startime { get; set; }
        public TimeSpan endime { get; set; }
    }
}
