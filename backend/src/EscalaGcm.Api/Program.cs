using System.Text;
using System.Text.Json.Serialization;
using EscalaGcm.Application.Services.Interfaces;
using EscalaGcm.Infrastructure.Services;
using EscalaGcm.Domain.Interfaces;
using EscalaGcm.Infrastructure.Data;
using EscalaGcm.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=escala_gcm.db"));

// Auth
// REVIEW: Hardcoded fallback JWT secret. If config is missing in production, this weak key silently applies.
// Consider throwing if Jwt:Key is absent in non-Development environments.
var jwtKey = builder.Configuration["Jwt:Key"] ?? "EscalaGcmSuperSecretKeyForDevelopment2024!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "EscalaGcm",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "EscalaGcm",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISetorService, SetorService>();
builder.Services.AddScoped<IPosicaoService, PosicaoService>();
builder.Services.AddScoped<ITurnoService, TurnoService>();
builder.Services.AddScoped<IHorarioService, HorarioService>();
builder.Services.AddScoped<IGuardaService, GuardaService>();
builder.Services.AddScoped<IViaturaService, ViaturaService>();
builder.Services.AddScoped<IEquipeService, EquipeService>();
builder.Services.AddScoped<IFeriasService, FeriasService>();
builder.Services.AddScoped<IAusenciaService, AusenciaService>();
builder.Services.AddScoped<IConflictValidationService, ConflictValidationService>();
builder.Services.AddScoped<ISectorRuleService, SectorRuleService>();
builder.Services.AddScoped<IEscalaService, EscalaService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IRetService, RetService>();
builder.Services.AddScoped<IGuardaAvailabilityService, GuardaAvailabilityService>();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
// REVIEW: CORS origin is hardcoded to localhost:5173. Use appsettings to configure per environment.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Auto-migrate and seed
// REVIEW: Auto-migrate on every startup is risky in production. Migrations should be a deliberate deployment step.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await SeedData.InitializeAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
