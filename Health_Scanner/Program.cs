using CompileLab.WebApi; // וודאי שזה ה-Namespace של ה-ExceptionHandler שלך
using DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Repositories;
using Service;
using Service.Interfaces;
using Service.Services;

var builder = WebApplication.CreateBuilder(args);

// --- רישום שירותים (Services) ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// רישום ה-Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// הגדרת בסיס הנתונים
builder.Services.AddDbContext<HealthScannerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// הזרקת תלויות (DI)
builder.Services.AddScoped<IContext, HealthScannerContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// עדכון AutoMapper לצורה המומלצת שסורקת את ה-Assembly
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(MapperProfile).Assembly);
});

var app = builder.Build();

// --- הגדרת הצינור (Middleware Pipeline) ---

// חייב להופיע כמה שיותר מוקדם כדי לתפוס שגיאות מכל מה שבא אחריו
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// אם את מוסיפה אימות בעתיד, הוא יבוא כאן
app.UseAuthorization();

app.MapControllers();

app.Run();