
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using P2_RaceDay.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Register controllers
builder.Services.AddControllers();

// 2. Configure SQL Server and Entity Framework Core
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "The DefaultConnection connection string is missing.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Configure JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT key is missing. Configure Jwt:Key in User Secrets.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT Issuer or Audience is missing from configuration.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// 4. Configure authorization for role-protected endpoints
builder.Services.AddAuthorization();

// Swagger / OpenAPI setup removed to avoid build-time dependency issues.
// Re-add Swagger when Microsoft.OpenApi package resolves correctly.

var app = builder.Build();

// Swagger UI disabled for now
// if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

// 7. Configure the HTTP request pipeline
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 8. Map controller routes
app.MapControllers();

app.Run();
