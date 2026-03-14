namespace reminder_schedule_backend.Models
{
    public class Admin
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
