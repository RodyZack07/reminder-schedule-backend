namespace reminder_schedule_backend.Models
{
    public class Student
    {   
            public int id { get; set; }
            public int nis { get; set; }
            public string nama { get; set; }
            public DateTime tanggalLahir { get; set; }
            public int? classId { get; set; }
            public Class Class { get; set; } = null!;
            
       
    }
}
