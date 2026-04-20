using Microsoft.EntityFrameworkCore;
using reminder_schedule_backend.Data;
using reminder_schedule_backend.Middleware;
using reminder_schedule_backend.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register DB CONTEXT
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//  JWT AUTHENTICATION
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

//REGISTER SERVICES
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<FirebaseNotificationService>();
builder.Services.AddHostedService<ScheduleWatcher>();

var app = builder.Build();

// --- MIDDLEWARE CORS MANUAL (BRUTE FORCE) ---
app.Use(async (context, next) =>
{
    // Ambil origin dari mana pun permintaan datang
    var origin = context.Request.Headers["Origin"].ToString();
    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
    }
    else
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    }

    context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

    // Jika ini adalah permintaan "Cek Pintu" (OPTIONS), langsung kembalikan OK
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("OK");
        return;
    }

    await next();
});
// --------------------------------------------

app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Matikan sementara karena pakai Cloudflare Tunnel
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();


