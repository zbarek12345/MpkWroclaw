using Microsoft.EntityFrameworkCore;
using MPKWrocław.Models;

namespace MPKWrocław.Database;

public class UserDataBaseContext : DbContext
{
    public DbSet<UserModel> UserModels { get; set; }
    public DbSet<UserLogin> UserLogins { get; set; }

    public UserDataBaseContext(DbContextOptions<UserDataBaseContext> options)
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.Property(um => um.UserID)
                .HasColumnName("UserID")
                .HasColumnType("varchar(32)");

            entity.Property(um => um.CreationDate)
                .HasColumnName("CreationDate")
                .HasColumnType("datetime");

            entity.Property(um => um.Username)
                .HasColumnName("Username")
                .HasColumnType("varchar(32)");

            entity.Property(um => um.Name)
                .HasColumnName("Name")
                .HasColumnType("varchar(32)");

            entity.Property(um => um.Password)
                .HasColumnName("Password")
                .HasColumnType("varchar(32)");

            entity.Property(um => um.Email)
                .HasColumnName("email")
                .HasColumnType("varchar(32)");

            entity.HasKey(um => um.UserID);
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.Property(ul => ul.UserID)
                .HasColumnName("UserID")
                .HasColumnType("varchar(32)");

            entity.Property(ul => ul.Token)
                .HasColumnName("Token")
                .HasColumnType("varchar(32)");

            entity.Property(ul => ul.LogInDevice)
                .HasColumnName("LogInDevice")
                .HasColumnType("varchar(32)");

            entity.Property(ul => ul.LogInIp)
                .HasColumnName("LogInIp")
                .HasColumnType("varchar(32)");

            entity.Property(ul => ul.LogInDate)
                .HasColumnName("LogInDate")
                .HasColumnType("datetime");

            entity.Property(ul => ul.LogOutTime)
                .HasColumnName("LogOutTime")
                .HasColumnType("datetime");
            
            entity.HasKey(ul => ul.Token);
        });
    }
}
