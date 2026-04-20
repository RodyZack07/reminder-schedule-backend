using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using reminder_schedule_backend.Data;
using reminder_schedule_backend.DTOs.Admin;
using reminder_schedule_backend.Exceptions;
using reminder_schedule_backend.Models;

namespace reminder_schedule_backend.Services
{
    public class AdminService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AdminService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // ============================================================
        // REGISTER — buat akun admin baru
        // ============================================================
        public async Task<AdminResponseDto> RegisterAsync(AdminRegisterDto dto)
        {
            // Cek duplikat email
            if (await _db.Admins.AnyAsync(a => a.Email == dto.Email))
                throw new ConflictException("Email sudah terdaftar");

            var admin = new Admin
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password)
            };

            _db.Admins.Add(admin);
            await _db.SaveChangesAsync();

            return ToResponseDto(admin);
        }

        // ============================================================
        // LOGIN
        // ============================================================
        public async Task<AdminLoginResponseDto> LoginAsync(
            AdminLoginDto dto, HttpResponse httpResponse)
        {
            var admin = await _db.Admins
                .FirstOrDefaultAsync(a => a.Email == dto.Email)
                ?? throw new UnauthorizedException("Email atau password salah");

            if (!VerifyPassword(dto.Password, admin.PasswordHash))
                throw new UnauthorizedException("Email atau password salah");

            var accessToken = GenerateAccessToken(admin);
            var refreshToken = GenerateRefreshToken();

            admin.RefreshToken = HashPassword(refreshToken);
            admin.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _db.SaveChangesAsync();

            httpResponse.Cookies.Append("adminRefreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return ToLoginResponseDto(admin, accessToken);
        }

        // ============================================================
        // REFRESH TOKEN
        // ============================================================
        public async Task<AdminLoginResponseDto> RefreshTokenAsync(
            HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var refreshToken = httpRequest.Cookies["adminRefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                throw new UnauthorizedException("Refresh token tidak ditemukan");

            var hashed = HashPassword(refreshToken);
            var admin = await _db.Admins
                .FirstOrDefaultAsync(a => a.RefreshToken == hashed)
                ?? throw new UnauthorizedException("Refresh token tidak valid");

            if (admin.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token sudah expired, silakan login ulang");

            var newAccessToken = GenerateAccessToken(admin);
            var newRefreshToken = GenerateRefreshToken();

            admin.RefreshToken = HashPassword(newRefreshToken);
            admin.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _db.SaveChangesAsync();

            httpResponse.Cookies.Append("adminRefreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return ToLoginResponseDto(admin, newAccessToken);
        }

        // ============================================================
        // LOGOUT
        // ============================================================
        public async Task LogoutAsync(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            var refreshToken = httpRequest.Cookies["adminRefreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var hashed = HashPassword(refreshToken);
                var admin = await _db.Admins
                    .FirstOrDefaultAsync(a => a.RefreshToken == hashed);

                if (admin != null)
                {
                    admin.RefreshToken = null;
                    admin.RefreshTokenExpiry = null;
                    await _db.SaveChangesAsync();
                }
            }

            httpResponse.Cookies.Delete("adminRefreshToken");
        }

        // ============================================================
        // PRIVATE HELPERS
        // ============================================================
        private static string HashPassword(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hash)
            => HashPassword(password) == hash;

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private string GenerateAccessToken(Admin admin)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name,           admin.Name),
                new Claim(ClaimTypes.Email,          admin.Email),
                new Claim(ClaimTypes.Role,           "admin")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ============================================================
        // MAPPERS
        // ============================================================
        private static AdminResponseDto ToResponseDto(Admin a) => new()
        {
            Id = a.Id,
            Name = a.Name,
            Email = a.Email
        };

        private static AdminLoginResponseDto ToLoginResponseDto(Admin a, string token) => new()
        {
            Id = a.Id,
            Name = a.Name,
            Email = a.Email,
            Token = token
        };
    }
}