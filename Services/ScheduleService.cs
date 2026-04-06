using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NpgsqlTypes;
using reminder_schedule_backend.Helpers;
using reminder_schedule_backend.Data;   
using reminder_schedule_backend.DTOs.Schedule;
using reminder_schedule_backend.Models;
using reminder_schedule_backend.Exceptions;
using System.Security.Authentication.ExtendedProtection;

namespace reminder_schedule_backend.Services
{
    public class ScheduleService
    {
        private readonly AppDbContext _db;

        public ScheduleService(AppDbContext db) => _db = db;

    

        //---------------------------GET ALL SCHEDULES---------------------------
        public async Task<List<ScheduleResponseDto>> GetAllAsync()
        {
                var schedules = await _db.Schedules
                    .Include(t => t.teacher)
                    .Include(s => s.Class)
                    .Include(s => s.subject)
                    .Include(s => s.session)
                    .ToListAsync();

            return schedules.Select(ToScheduleResponseDto).ToList();
        }


        //---------------------------CHECK FOREIGN KEY VALIDATION---------------------------
        private async Task ValidateForeignKey (int teacherId, int classId, int subjectId, int sessionId)
        {
            if (!await _db.Teachers.AnyAsync(t => t.Id == teacherId))
                throw new NotFoundException("Guru tidak ditemukan");
            if (!await _db.Classses.AnyAsync(c => c.Id == classId))
                throw new NotFoundException("Kelas tidak ditemukan");
            if (!await _db.Subjects.AnyAsync(s => s.Id == subjectId))   
                throw new NotFoundException("Mata pelajaran tidak ditemukan");
            if (!await _db.Sessions.AnyAsync(s => s.Id == sessionId))
                throw new NotFoundException("Sesi tidak ditemukan");
        }


        //---------------------------CHECK SCHEDULE CONFLICT---------------------------
        private async Task CheckConflictAsync(DayOfWeek day, int sessionId, int classId, int teacherId, int excluded = 0)
        {
            if (await _db.Schedules.AnyAsync(s  =>
                s.id != excluded &&
                s.day == day &&
                s.classId == classId &&
                s.sessionId == sessionId
                )) throw new ConflictException(" Jadwal bentrok untuk kelas ini pada hari dan sesi yang sama");

            if (await _db.Schedules.AnyAsync(s =>
                s.id != excluded &&
                s.day == day &&
                s.teacherId == teacherId &&
                s.sessionId == sessionId
                )) throw new ConflictException(" Jadwal bentrok untuk guru ini pada hari dan sesi yang sama");
        }

        //--------------------LOAD FOERIGN CLASS---------------------//
        private async Task LoadRelationClassAsync(Schedule s)
        {
            await _db.Entry(s).Reference(sc => sc.teacher).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.Class).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.subject).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.session).LoadAsync();
        }
        

        //--------------------RESPONSE DARI SCHEDLE KE SCHEDULE RESPONSE DTO---------------------//
        private static ScheduleResponseDto ToScheduleResponseDto(Schedule s) => new()
        {   
            
            day = s.day,
            teacherId = s.teacherId,
            teacherName = s.teacher.Name,
            classId = s.classId,
            className = s.teacher?.Name,
            subjectId = s.subjectId,
            subjectName = s.subject?.Name,
            sessioId = s.sessionId,
            startTime = s.session?.startime,
            endTime = s.session?.endime

        };

    }
}
