using reminder_schedule_backend.Data;
using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.DTOs.Teacher;
using reminder_schedule_backend.Models;
using reminder_schedule_backend.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;


namespace reminder_schedule_backend.Services
{
    public class TeacherService
    {
        public readonly AppDbContext _db;
        public readonly IConfiguration _config;
        public TeacherService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

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
            if (teacher == null) throw new NotFoundException("Class not found");
            return ToTeacherResponseDto(teacher);
        }

        //---------------------------CREATE TEACHER ACCOUNT----------------------
        public async Task<TeacherResponseDto> CreateTeacherAsync(TeacherCreateDto dto)
        {
            var teacher = new Teacher
            {
                Nik = dto.Nik,
                Name = dto.Name,
                passwordHash = HashPassword(dto.Password)
            };

            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();
            return ToTeacherResponseDto(teacher);
        }

        //---------------------------UPDATE TEACHER ACCOUNT----------------------
        public async Task<TeacherResponseDto> UpdateTeacherAsync(int id, TeacherUpdateDto dto)
        {
            var teacher = await _db.Teachers.FindAsync(id) ?? throw new Exception("Teacher not found");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                teacher.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Password))
                teacher.passwordHash = HashPassword(dto.Password);

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

            if (!VerifyPassword(dto.Password, teacher.passwordHash!))
                throw new UnauthorizedException("NIK atau password salah");

            var token = GenerateToken(teacher);
            var refreshToken = RefreshToken(teacher);

            teacher.refreshToken = HashPassword(refreshToken);
            teacher.refreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _db.SaveChangesAsync();

            httpResponse.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(7)
            });



            return TeacherLoginResponseDto(teacher, token);
        }

        //---------------------------TEACHER REFRESH TOKEN----------------------
        public async Task<TeacherLoginResponseDto> RefreshTokenAsync(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var refreshToken = httpRequest.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedException("Refresh token is missing");

            var hashedToken = HashPassword(refreshToken);

            var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.refreshToken == hashedToken);

            if (teacher == null || teacher.refreshTokenExpiryTime <= DateTime.Now)
                throw new UnauthorizedException("Invalid or expired refresh token");

            var newRefreshToken = RefreshToken(teacher);
            teacher.refreshToken = HashPassword(newRefreshToken);

            teacher.refreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _db.SaveChangesAsync();

            httpResponse.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(7)
            });

            return TeacherLoginResponseDto(teacher, GenerateToken(teacher));
        }

        //---------------------------TEACHER LOGOUT----------------------
        public async Task LogoutTeacherAsync (HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var refreshToken= httpRequest.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var hashedToken = HashPassword(refreshToken);
                var teacher = await _db.Teachers
                    .FirstOrDefaultAsync(t => t.refreshToken == hashedToken);

                if (teacher != null)
                {
                    teacher.refreshToken = null;
                    teacher.refreshTokenExpiryTime = null;
                    await _db.SaveChangesAsync();
                }
            }

            httpResponse.Cookies.Delete("refreshToken");

        }

        //============================PRIVATE HELPER METHODS============================

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static bool VerifyPassword(string password, string passwordHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == passwordHash;
        }

        public static string RefreshToken(Teacher teacher)
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public string GenerateToken(Teacher teacher)
        {
            var jwtKey = _config["Jwt:Key"] ?? throw new Exception("JWT Key belum diatur di appsettings.json");

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey));
            var credintials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, teacher.Id.ToString()),
                new Claim(ClaimTypes.Name, teacher.Name),
                new Claim("nik", teacher.Nik),
                new Claim(ClaimTypes.Role, "teacher")
            };

            var token = new JwtSecurityToken(

                issuer: _config["Jwt:Issuer"],
                claims: claims,
                audience: _config["Jwt:Audience"],
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credintials
            );
                
            return new JwtSecurityTokenHandler().WriteToken(token);
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
