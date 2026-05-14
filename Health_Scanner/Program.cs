using DataContext;
using Health_Scanner.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Repository.Interfaces;
using Repository.Repositories;
using Service;
using Service.Interfaces;
using Service.Services;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// --- רישום שירותים (Services) ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Health Scanner API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

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
builder.Services.AddScoped<IAuthService, AuthService>();

// עדכון AutoMapper לצורה המומלצת שסורקת את ה-Assembly
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(MapperProfile).Assembly);
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll",
        p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
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


// ... בהמשך הקוד לפני UseAuthorization
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();