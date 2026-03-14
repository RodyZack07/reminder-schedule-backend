using System.Security.Authentication.ExtendedProtection;
using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Models;

namespace reminder_schedule_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

           
        }
        

        public DbSet<Student> Students { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Class> Classses { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {   

            //--------------------------ENTITY RELATIONS---------------------------//
            model.Entity<Student>().HasIndex(s => s.nis).IsUnique();

            //CLASS - TEACHER
            model.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);


            //CLASS - STUDENTS
            model.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(s => s.students)
                .HasForeignKey(s => s.classId)
                .OnDelete(DeleteBehavior.SetNull);
            
            //CLASS - SCHEDULE
            model.Entity<Schedule>()
                .HasOne(s => s.Class)
                .WithMany(c => c.schedules)
                .HasForeignKey(s => s.classId)
                .OnDelete(DeleteBehavior.Cascade);
            
            //SESSION - SCHEDULE
            model.Entity<Schedule>()
                .HasOne(s => s.session)
                .WithMany()
                .HasForeignKey(s => s.sessionId)
                .OnDelete(DeleteBehavior.Restrict);


            //SUBJECT - SCHEDULE
            model.Entity<Schedule>()
                .HasOne(s => s.subject)
                .WithMany()
                .HasForeignKey(s => s.subjectId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(model);
        }
        
    }
}
