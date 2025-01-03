using Microsoft.EntityFrameworkCore;
using MPKWrocław.Database;
using MPKWrocław.DataHandling;
using MPKWrocław.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var cpath = Path.Combine(Directory.GetCurrentDirectory(),"database","sql");

/// Existance of databases is verified
#region init

{
    if (!Directory.Exists(cpath))
        Directory.CreateDirectory(cpath);

    if (!File.Exists(Path.Combine(cpath, "mpk.sqlite")))
    {
        using (FileStream fs = File.Create(Path.Combine(cpath, "mpk.sqlite")))
        {
            // Ensure the file is created and closed properly
        }

        var dbBuilder =
            new DbContextOptionsBuilder<MpkDatabaseContext>().UseSqlite($"Data Source={Path.Combine(cpath, "mpk.sqlite")}");
        var mpk = new MpkDatabaseContext(dbBuilder.Options);
        mpk.Database.Migrate();
        var reader = new Reader(mpk);
        mpk.Dispose();
    }
    
    if (!File.Exists(Path.Combine(cpath, "user.sqlite")))
    {
        // Create the SQLite file
        using (FileStream fs = File.Create(Path.Combine(cpath, "user.sqlite")))
        {
            // Ensure the file is created and closed properly
        }

        // Set up the database context options
        var dbBuilder2 = new DbContextOptionsBuilder<UserDataBaseContext>()
            .UseSqlite($"Data Source={Path.Combine(cpath, "user.sqlite")}");

        // Create the database context
        using (var user = new UserDataBaseContext(dbBuilder2.Options))
        {
            // Apply migrations to create the database schema
            user.Database.EnsureCreated();

            UserModel a = new UserModel
            {
                CreationDate = DateTime.Now,
                Email = "aaa@aaa.com",
                Name = "aaa",
                Username = "a",
                Password = "aaa",
                UserID = Guid.NewGuid()
            };
            user.UserModels.Add(a);
        }
    }
    
}

#endregion

builder.Services.AddDbContext<MpkDatabaseContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(cpath, "mpk.sqlite")}"));

builder.Services.AddDbContext<UserDataBaseContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(cpath, "user.sqlite")}"));



builder.Services.AddScoped<MpkSingleton>();
builder.Services.AddScoped<UserSingleton>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();


app.UseCors("AllowAllOrigins");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}