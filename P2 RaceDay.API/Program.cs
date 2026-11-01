
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using P2_RaceDay.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Register controllers
builder.Services.AddControllers();

// 2. Configure SQL Server and Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    // No SQL connection configured — use an in-memory database for development/testing
    Console.WriteLine("Warning: DefaultConnection missing. Using in-memory database for development.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("DevDb"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// 3. Configure JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "RaceDayIssuer";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "RaceDayAudience";

if (string.IsNullOrWhiteSpace(jwtKey))
{
    // Use a development key when none is configured so the app can run locally.
    jwtKey = "dev_secret_key_change_me_please_1234567890";
    Console.WriteLine("Warning: Jwt:Key missing. Using development JWT key. Do not use in production.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// 4. Configure authorization for role-protected endpoints
builder.Services.AddAuthorization();

// 5. Configure Swagger / OpenAPI (basic)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed development data only when using the in-memory provider so endpoints and Swagger show examples
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
        try
        {
            var db = services.GetRequiredService<ApplicationDbContext>();

            if (db.Database.IsInMemory())
            {
                db.Database.EnsureCreated();

                // Seed only when empty
                if (!db.Events.Any())
                {
                    var ev = new P2_RaceDay.API.Models.RaceEvent
                    {
                        EventName = "City Marathon",
                        EventDate = DateTime.UtcNow.AddDays(30),
                        Venue = "Central Park",
                        Location = "Downtown",
                        Description = "Annual city marathon"
                    };

                    db.Events.Add(ev);

                    var cat = new P2_RaceDay.API.Models.Category
                    {
                        Name = "Adult",
                        Description = "Adult category"
                    };

                    db.Categories.Add(cat);

                    // Save to get generated keys
                    db.SaveChanges();

                    var race = new P2_RaceDay.API.Models.Race
                    {
                        RaceName = "10K",
                        EventID = ev.EventID
                    };

                    db.Races.Add(race);
                    db.SaveChanges();
                }
            }
            else
            {
                // When using real SQL Server, ensure database schema exists (creates database/tables if needed)
                try
                {
                    db.Database.EnsureCreated();
                    Console.WriteLine("Database EnsureCreated executed for SQL provider.");
                }
                catch (Exception exSql)
                {
                    Console.WriteLine($"Could not create SQL database/schema at startup: {exSql.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding database: {ex.Message}");
        }
}

// 6. Enable Swagger / Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RaceDay API v1");
    options.RoutePrefix = "swagger";
});

// 7. Configure the HTTP request pipeline
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 8. Map controller routes
app.MapControllers();

// Add simple root endpoint so visiting / shows a message
app.MapGet("/", () => Results.Ok(new { status = "API running", now = DateTime.UtcNow }));

app.Run();
