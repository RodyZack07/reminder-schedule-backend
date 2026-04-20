using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Models;

namespace reminder_schedule_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Schedule> Schedules { get; set; } = null!;
        public DbSet<TaskReminder> TaskReminders { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // PRIMARY KEY CUSTOM
            // =========================
            modelBuilder.Entity<Schedule>()
                .HasKey(s => s.id);

            // =========================
            // UNIQUE INDEX
            // =========================
            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.Nik)
                .IsUnique();

            // =========================
            // TEACHER - SCHEDULE
            // Satu teacher punya banyak schedule
            // =========================
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.teacher)
                .WithMany(t => t.Schedules)
                .HasForeignKey(s => s.teacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // CLASS - SCHEDULE
            // Satu class punya banyak schedule
            // =========================
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Class)
                .WithMany()
                .HasForeignKey(s => s.classId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // SUBJECT - SCHEDULE
            // =========================
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.subject)
                .WithMany()
                .HasForeignKey(s => s.subjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // SESSION START - SCHEDULE
            // =========================
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.sessionStart)
                .WithMany()
                .HasForeignKey(s => s.sessionStartId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // SESSION END - SCHEDULE
            // =========================
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.sessionEnd)
                .WithMany()
                .HasForeignKey(s => s.sessionEndId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // SCHEDULE - TASKREMINDER
            // =========================
            modelBuilder.Entity<TaskReminder>()
                .HasOne(t => t.Schedule)
                .WithMany()
                .HasForeignKey(t => t.scheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // MAX LENGTH / COLUMN CONFIG
            // =========================
            modelBuilder.Entity<Teacher>()
                .Property(t => t.Nik)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Teacher>()
                .Property(t => t.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Class>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Class>()
                .Property(c => c.Grade)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Subject>()
                .Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Session>()
                .Property(s => s.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<TaskReminder>()
                .Property(t => t.description)
                .HasMaxLength(1000)
                .IsRequired();
        }
    }
}