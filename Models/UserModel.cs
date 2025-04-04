using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MPKWrocław.Models;

public class UserModel
{
    [Key]
    public Guid UserID { get; set; } // Primary key

    public DateTime CreationDate { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Username { get; set; }
    
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(256)] // For hashed passwords
    public string Password { get; set; }
    
    [Required]
    [StringLength(256)]
    public string Email { get; set; }

    public virtual IList<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
    public virtual IList<UserFavourite> UserFavourites { get; set; } = new List<UserFavourite>();
}

public class UserLogin
{
    [Key] // Composite key will be defined in OnModelCreating
    public Guid UserID { get; set; }
    
    [Key]
    public Guid Token { get; set; } // Part of composite key
    
    [StringLength(100)]
    public string LogInDevice { get; set; }
    
    [StringLength(45)] // Enough for IPv6
    public string LogInIp { get; set; }
    
    public DateTime LogInDate { get; set; }
    public DateTime? LogOutTime { get; set; } // Nullable since user might not log out

    [ForeignKey("UserID")]
    public virtual UserModel User { get; set; }
}

public class UserFavourite
{
    [Key] // Composite key will be defined in OnModelCreating
    public Guid UserID { get; set; }
    
    [Key]
    [StringLength(50)]
    public string FavouriteID { get; set; } // Part of composite key

    [ForeignKey("UserID")]
    public virtual UserModel User { get; set; }
}