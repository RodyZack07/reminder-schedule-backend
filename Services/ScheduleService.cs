using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using NpgsqlTypes;
using reminder_schedule_backend.Helpers;
using reminder_schedule_backend.Data;   
using reminder_schedule_backend.DTOs.Schedule;
using reminder_schedule_backend.Models;
using reminder_schedule_backend.Exceptions;


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
                    .Include(s => s.sessionStart)
                    .Include(s => s.sessionEnd)
                    .OrderBy(s => s.day)
                    .ThenBy(s => s.sessionStart.startTime)
                    .ToListAsync();

            return schedules.Select(ToScheduleResponseDto).ToList();
        }

        //---------------------------GET SCHEDULE BY TEACHER---------------------------
        public async Task<List<ScheduleResponseDto>> GetScheduleByTeacherAsync(int teacherId)
        {
            var schedules = await _db.Schedules
                .Where(s => s.teacherId == teacherId)
                .Include(s => s.teacher)
                .Include(s => s.Class)
                .Include(s => s.subject)
                .Include(s => s.sessionStart)
                .Include(s => s.sessionEnd)
                .OrderBy(s => s.day)
                .ThenBy(s => s.sessionStart.startTime)
                .ToListAsync();

            return schedules.Select(ToScheduleResponseDto).ToList();
        }

        //---------------------------CREATE SCHEDULE---------------------------
        public async Task<ScheduleResponseDto> CreateScheduleAsync (ScheduleCreateDto dto)
        {
            await ValidateForeignKey(dto.teacherId, dto.classId, dto.subjectId, dto.sessionStartId, dto.sessionEndId);
            await ValidateSessionOrderAsync(dto.sessionStartId, dto.sessionEndId);
            await CheckConflictAsync((DayOfWeek)dto.day, dto.sessionStartId, dto.classId, dto.teacherId);


            var schedule = new Schedule
            {
                day = (DayOfWeek)dto.day,
                teacherId = dto.teacherId,
                classId = dto.classId,
                subjectId = dto.subjectId,
                sessionStartId = dto.sessionStartId,
                sessionEndId = dto.sessionEndId
            };
            
            _db.Schedules.Add(schedule);
            await _db.SaveChangesAsync();
            await LoadRelationAsync(schedule);

            return ToScheduleResponseDto(schedule);
        }

        //---------------------------DELETE SCHEDULE---------------------------
        public async Task DeleteScheduleAsync(int id)
        {
            var schedule = await _db.Schedules.FindAsync(id);
            if (schedule == null)
                throw new NotFoundException(" Jadwal tidak ditemukan");


            _db.Schedules.Remove(schedule);
            await _db.SaveChangesAsync();
        }

        //---------------------------CHECK FOREIGN KEY VALIDATION---------------------------
        private async Task ValidateForeignKey (int teacherId, int classId, int subjectId, int sessionStartId, int sessionEndId)
        {
            if (!await _db.Teachers.AnyAsync(t => t.Id == teacherId))
                throw new NotFoundException("Guru tidak ditemukan");
            if (!await _db.Classses.AnyAsync(c => c.Id == classId))
                throw new NotFoundException("Kelas tidak ditemukan");
            if (!await _db.Subjects.AnyAsync(s => s.Id == subjectId))   
                throw new NotFoundException("Mata pelajaran tidak ditemukan");
            if (!await _db.Sessions.AnyAsync(s => s.Id == sessionStartId))
                throw new NotFoundException("Sesi mulai tidak ditemukan");
            if (!await _db.Sessions.AnyAsync(s => s.Id == sessionEndId))
                throw new NotFoundException("Sesi akhir tidak ditemukan");
        }

        private async Task ValidateSessionOrderAsync(int sessionStartId, int sessionEndId)
        {
            var start = await _db.Sessions.FindAsync(sessionStartId);
            var end = await _db.Sessions.FindAsync(sessionEndId);

            if (start != null && end != null && start.startTime > end.startTime)
                throw new BadRequestException("Sesi mulai tidak boleh lebih akhir dari sesi selesai");
        }


        //---------------------------CHECK SCHEDULE CONFLICT---------------------------
        private async Task CheckConflictAsync(DayOfWeek day, int sessionId, int classId, int teacherId, int excluded = 0)
        {
            if (await _db.Schedules.AnyAsync(s  =>
                s.id != excluded &&
                s.day == day &&
                s.classId == classId &&
                s.sessionStartId == sessionId &&
                s.sessionEndId == sessionId 
                )) throw new ConflictException(" Jadwal bentrok untuk kelas ini pada hari dan sesi yang sama");

            if (await _db.Schedules.AnyAsync(s =>
                s.id != excluded &&
                s.day == day &&
                s.teacherId == teacherId &&
                s.sessionStartId == sessionId &&
                s.sessionEndId == sessionId
                )) throw new ConflictException(" Jadwal bentrok untuk guru ini pada hari dan sesi yang sama");
        }

        //--------------------LOAD FOERIGN CLASS---------------------//
        private async Task LoadRelationClassAsync(Schedule s)
        {
            await _db.Entry(s).Reference(sc => sc.teacher).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.Class).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.subject).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.sessionStart).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.sessionEnd).LoadAsync();
        }


        //--------------------LOAD RELATION CLASS---------------------//
        private async Task LoadRelationAsync(Schedule s)
        {
            await _db.Entry(s).Reference(sc => sc.teacher).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.Class).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.subject).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.sessionStart).LoadAsync();
            await _db.Entry(s).Reference(sc => sc.sessionEnd).LoadAsync();
        }


        //--------------------RESPONSE DARI SCHEDLE KE SCHEDULE RESPONSE DTO---------------------//
        private static ScheduleResponseDto ToScheduleResponseDto(Schedule s) => new()
        {   
            id = s.id,
            day = s.day,
            teacherId = s.teacherId,
            teacherName = s.teacher.Name,
            classId = s.classId,
            className = s.Class?.Name,
            subjectId = s.subjectId,
            subjectName = s.subject?.Name,

            sessionStartId = s.sessionStartId,
            sessionStartName = s.sessionStart?.Name,
            sessionEndId = s.sessionEndId,
            sessionEndName = s.sessionEnd?.Name,

            startTime = s.sessionStart?.startTime,
            endTime = s.sessionEnd?.endTime

        };

    }
}
