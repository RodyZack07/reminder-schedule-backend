namespace reminder_schedule_backend.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? passwordHash { get; set; }
        public ICollection<Class>? Classes { get; set; } = new List<Class>();
        public ICollection<Schedule> Schedules { get; set; }   = new List<Schedule>();

    }
}
