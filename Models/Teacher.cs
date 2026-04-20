namespace reminder_schedule_backend.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Nik { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? passwordHash { get; set; }
        public string? fcmToken { get; set; }

        public string? refreshToken { get; set; }   
        public DateTime? refreshTokenExpiryTime { get; set; }


        //----------------------------NAVIGATION PROPERTIES----------------------
        public ICollection<Class> Classes { get; set; } = new List<Class>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }    
}
