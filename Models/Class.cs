namespace reminder_schedule_backend.Models
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Teacher? Teacher { get; set; } = null!;
        public int? TeacherId { get; set; } 
        
        public ICollection<Student> students { get; set; } = new List<Student>();
        public ICollection<Schedule> schedules { get; set; } = new List<Schedule>();
       
    }
}
