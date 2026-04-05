using Microsoft.AspNetCore.Diagnostics;
using reminder_schedule_backend.Exceptions;


namespace reminder_schedule_backend.Middleware
{
    public static class ExceptionHandlerExtension
    {
        public static void UseGlobalExceptionHandler( this WebApplication app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                    context.Response.StatusCode = ex switch
                    {
                        NotFoundException =>404,
                        BadRequestException => 401,
                        ConflictException => 409,
                        UnauthorizedException => 400,
                       
                    };

                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = ex?.Message ?? "Terjadi kesalahan pada server"
                    });
                });
            });
        }
    }
}
