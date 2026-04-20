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
    var origin = context.Request.Headers["Origin"].ToString();
    
    // Log di terminal laptop untuk memastikan request masuk
    Console.WriteLine($"[CORS DEBUG] {context.Request.Method} dari: {origin || "No Origin"} ke: {context.Request.Path}");

    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = origin;
    }
    else
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    }

    context.Response.Headers["Access-Control-Allow-Headers"] = "*";
    context.Response.Headers["Access-Control-Allow-Methods"] = "*";
    context.Response.Headers["Access-Control-Allow-Credentials"] = "true";

    // Langsung tangani Pre-flight (OPTIONS)
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


