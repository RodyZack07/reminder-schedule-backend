using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Data;
using reminder_schedule_backend.DTOs.Session;
using reminder_schedule_backend.Models;
using reminder_schedule_backend.Exceptions;

namespace reminder_schedule_backend.Services
{
    public class SessionService
    {
        public readonly AppDbContext _db;
        public SessionService(AppDbContext db)  => _db = db;


        //---------------------------GET ALL SESSIONS----------------------



        //---------------------------DELETE SESSION----------------------
        public async Task DeleteSession(int id)
        {
            var session = await _db.Sessions.FindAsync(id);
            if (session == null) throw new NotFoundException("Session not found");

            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync();
        }


        //---------------------------CHECK CONFLICT----------------------
        public async Task<bool> CheckConflict(TimeSpan startTime, TimeSpan endTime)
        {
            return await _db.Sessions.AnyAsync(s =>
                (startTime < s.endime && endTime > s.startime) 
            );
        }

        //---------------------------SESSION RESPONSE---------------------------
        public static SessionResponseDto ToSessionResponseDto(Session session) => new()
        {
            Id = session.Id,
            Name = session.Name,
            StarTime = session.startime,
            EndTime = session.endime
        };



    }
}
