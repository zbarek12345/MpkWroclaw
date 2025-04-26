using Microsoft.EntityFrameworkCore;
using MPKWrocław.Database;
using MPKWrocław;
using MPKWrocław.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5262);
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
String cpath = Path.Combine(Directory.GetCurrentDirectory(),"database","sql");

/// Existance of databases is verified
#region init

{
    if (!Directory.Exists(cpath))
        Directory.CreateDirectory(cpath);
    

    // Set up the database context options
    var dbBuilder2 = new DbContextOptionsBuilder<UserDataBaseContext>()
        .UseSqlite($"Data Source={Path.Combine(cpath, "user.sqlite")}");

    // Create the database context
    using (var user = new UserDataBaseContext(dbBuilder2.Options))
    {
        // Apply migrations to create the database schema
        user.Database.EnsureCreated();
        user.SaveChanges();
    }
}

#endregion

builder.Services.AddDbContext<MpkDatabaseContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(cpath, "mpk.sqlite")}"));



builder.Services.AddScoped<MpkSingleton>();
builder.Services.AddScoped<UserSingleton>();
builder.Services.AddSingleton<DatabaseUpdater>();
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
app.MapControllers();

var dbUpdater = app.Services.GetRequiredService<DatabaseUpdater>();
 
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}