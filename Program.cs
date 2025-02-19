using AGROCHEM.Data;
using AGROCHEM.Models.Entities;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DotNetEnv;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

//var connectionString = builder.Configuration.GetConnectionString("AGROCHEMConn");
//System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
var connectionString = Env.GetString("CONNECTION_STRING");

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var frontendUrl = Env.GetString("FRONTEND_URL");

var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key), // U¿ycie klucza z appsettings.json
        ValidateIssuer = true, // Walidacja Issuer
        ValidateAudience = true, // Walidacja Audience
        ValidIssuer = jwtSettings["Issuer"], // Issuer z konfiguracji
        ValidAudience = jwtSettings["Issuer"], // Audience z konfiguracji
        RequireExpirationTime = true,
        ValidateLifetime = true // Walidacja, czy token nie wygas³
    };
});




builder.Services.AddDbContext<AgrochemContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins(frontendUrl) // React frontend
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    }); 
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserService, UserService>();
builder.Services.AddScoped<PlotService, PlotService>();
builder.Services.AddScoped<CultivationService, CultivationService>();
builder.Services.AddScoped<PlantService, PlantService>();
builder.Services.AddScoped<ChemicalAgentService, ChemicalAgentService>();
builder.Services.AddScoped<ChemicalUseService, ChemicalUseService>();
builder.Services.AddScoped<DiseaseService, DiseaseService>();
builder.Services.AddScoped<ChemicalTreatmentService, ChemicalTreatmentService>();
builder.Services.AddScoped<NotificationService, NotificationService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Wpisz token w formacie: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
