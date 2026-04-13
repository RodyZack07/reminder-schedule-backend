namespace reminder_schedule_backend.Models
{
    public class TaskReminder
    {      
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string description { get; set; } = null!;
            public bool status { get; set; }
            public DateTime remindAt { get; set; }
            public int scheduleId { get; set; }
            public Schedule Schedule { get; set; } = null!;
        
    }
}
