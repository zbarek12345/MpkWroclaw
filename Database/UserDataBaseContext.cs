using Microsoft.EntityFrameworkCore;
using MPKWrocław.Models;

namespace MPKWrocław.Database;

public class UserDataBaseContext : DbContext
{
    public DbSet<UserModel> UserModels { get; set; }
    public DbSet<UserLogin> UserLogins { get; set; }
    public DbSet<UserFavourite>  UserFavourites { get; set; }

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
          // UserModel
          modelBuilder.Entity<UserModel>(entity =>
          {
              entity.HasKey(u => u.UserID); // Primary key
              entity.Property(u => u.UserID)
                    .HasColumnType("uuid") // For PostgreSQL; use "uniqueidentifier" for SQL Server
                    .ValueGeneratedOnAdd(); // Auto-generate GUID
              
              entity.Property(u => u.CreationDate)
                    .HasColumnType("timestamp")
                    .IsRequired();
              
              entity.Property(u => u.Username)
                    .HasColumnType("varchar(50)")
                    .IsRequired();
              
              entity.Property(u => u.Name)
                    .HasColumnType("varchar(100)");
              
              entity.Property(u => u.Password)
                    .HasColumnType("varchar(256)") // For hashed passwords
                    .IsRequired();
              
              entity.Property(u => u.Email)
                    .HasColumnType("varchar(256)")
                    .IsRequired();

              // Relationships
              entity.HasMany(u => u.UserLogins)
                    .WithOne(ul => ul.User)
                    .HasForeignKey(ul => ul.UserID)
                    .OnDelete(DeleteBehavior.Cascade); // Delete logins when user is deleted
              
              entity.HasMany(u => u.UserFavourites)
                    .WithOne(uf => uf.User)
                    .HasForeignKey(uf => uf.UserID)
                    .OnDelete(DeleteBehavior.Cascade); // Delete favourites when user is deleted
          });

          // UserLogin
          modelBuilder.Entity<UserLogin>(entity =>
          {
              entity.HasKey(ul => new { ul.UserID, ul.Token }); // Composite key
              entity.Property(ul => ul.UserID)
                    .HasColumnType("uuid");
              
              entity.Property(ul => ul.Token)
                    .HasColumnType("uuid")
                    .ValueGeneratedOnAdd(); // Auto-generate GUID for Token
              
              entity.Property(ul => ul.LogInDevice)
                    .HasColumnType("varchar(100)");
              
              entity.Property(ul => ul.LogInIp)
                    .HasColumnType("varchar(45)"); // Enough for IPv6
              
              entity.Property(ul => ul.LogInDate)
                    .HasColumnType("timestamp")
                    .IsRequired();
              
              entity.Property(ul => ul.LogOutTime)
                    .HasColumnType("timestamp")
                    .IsRequired(false); // Nullable
          });

          // UserFavourite
          modelBuilder.Entity<UserFavourite>(entity =>
          {
              entity.HasKey(uf => new { uf.UserID, uf.FavouriteID }); // Composite key
              entity.Property(uf => uf.UserID)
                    .HasColumnType("uuid");
              
              entity.Property(uf => uf.FavouriteID)
                    .HasColumnType("varchar(50)");
          });
      }
}
