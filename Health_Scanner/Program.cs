using DataContext;
using Health_Scanner.WebApi;
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

builder.Services.AddHttpClient<IProductService, ProductService>();

// הזרקת תלויות (DI)
builder.Services.AddScoped<IContext, HealthScannerContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAllergenRepository, AllergenRepository>();
builder.Services.AddScoped<IAllergenService, AllergenService>();
builder.Services.AddScoped<IUserAllergenRepository, UserAllergenRepository>();
builder.Services.AddScoped<IUserAllergenService, UserAllergenService>();
builder.Services.AddScoped<IScanHistoryRepository, ScanHistoryRepository>();
builder.Services.AddScoped<IScanHistoryService, ScanHistoryService>();

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