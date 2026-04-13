using reminder_schedule_backend.Data;
using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.DTOs.Teacher;
using reminder_schedule_backend.Models;
using reminder_schedule_backend.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;


namespace reminder_schedule_backend.Services
{
    public class TeacherService
    {
        public readonly AppDbContext _db;
        public TeacherService(AppDbContext db) => _db = db;

        //---------------------------GET ALL TEACHERS----------------------
        public async Task<List<TeacherResponseDto>> GetAllTeachersAsync()
        {
            var teachers = await _db.Teachers.ToListAsync();
            return teachers.Select(ToTeacherResponseDto).ToList();
        }

        //---------------------------GET TEACHER BY ID----------------------
        public async Task<TeacherResponseDto?> GetTeacherByIdAsync(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            return teacher is null ? null : ToTeacherResponseDto(teacher);
        }

        //---------------------------CREATE TEACHER ACCOUNT----------------------
        public async Task<TeacherResponseDto> CreateTeacherAsync(TeacherCreateDto dto)
        {
            var teacher = new Teacher
            {
                Nik = dto.Nik,
                Name = dto.Name,
                passwordHash = dto.Password
            };

            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();
            return ToTeacherResponseDto(teacher);
        }


        //---------------------------DELETE TEACHER ACCOUNT--------------
        public async  Task DeleteTeacherAsync(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id)
            ?? throw new Exception("Teacher not found");

            _db.Teachers.Remove(teacher);
            await _db.SaveChangesAsync();
        }


        //---------------------------TEACHER LOGIN-----------------------
        public async Task<TeacherLoginResponseDto> LoginTeacherAsync (TeacherLoginDto dto, HttpResponse httpResponse )
        {
            var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.Nik == dto.Nik) 
                ?? throw new Exception("Teacher not found");

            if (teacher.Nik != dto.Nik || teacher.passwordHash != dto.Password)
                throw new UnauthorizedException("Password or Nik Invalid");


            
            return TeacherLoginResponseDto(teacher, "token");
        }


        //---------------------------MAPPING TEACHER TO TEACHER RESPONSE DTO----------------------
        public static TeacherResponseDto ToTeacherResponseDto(Teacher teacher) => new()
        {
            Id = teacher.Id,
            Name = teacher.Name,
            Nik = teacher.Nik

        };


        public static TeacherLoginResponseDto TeacherLoginResponseDto(Teacher t, string token) => new()
        {
            Id = t.Id,
            Name = t.Name,
            Nik = t.Nik,
            Token = token
        };
    }
}
