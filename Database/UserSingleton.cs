using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MPKWrocław.Models;

namespace MPKWrocław.Database;

public class UserSingleton
{
    private UserDataBaseContext _databaseContext;

    private readonly DbContextOptions<UserDataBaseContext> _options;

    private readonly IDbContextFactory<UserDataBaseContext> _db;
    // Public constructor for DI to inject UserDataBaseContext
    public UserSingleton()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(),"database","sql", "user.sqlite");;
        var optionsBuilder = new DbContextOptionsBuilder<UserDataBaseContext>();
        optionsBuilder.UseSqlite($"Data Source={path}");
        _db = new DbContextFactory<UserDataBaseContext>(null, optionsBuilder.Options  ,new DbContextFactorySource<UserDataBaseContext>());
    }

    public void AddUser(UserModel userModel)
    {
        using var context = _db.CreateDbContext();
        context.UserModels.Add(userModel);
        context.SaveChanges();
    }

    public Guid LoginUser(string username, string password, string logInDevice, string logInIp)
    {   
        using var context = _db.CreateDbContext();
        var dataModel = context.UserModels
            .FirstOrDefault(um => um.Username == username && um.Password == password);
            
        if (dataModel != null)
        {
            var guid = Guid.NewGuid();
            context.UserLogins.Add(new UserLogin { UserID = dataModel.UserID, Token = guid, LogInDate = DateTime.Now, LogOutTime = DateTime.Now.AddDays(1),LogInDevice = logInDevice, LogInIp = logInIp });
            context.SaveChanges();
            
            
            return guid;
        }
        return Guid.Empty;
    }

    public bool VerifyToken(Guid token)
    {   
        using var context = _db.CreateDbContext();
        var loginData = context.UserLogins.FirstOrDefault(ul => ul.Token == token);
            
        if (loginData == null || loginData.LogOutTime < loginData.LogInDate)
            return false;
        loginData.LogOutTime = DateTime.Now.AddDays(1);
        context.SaveChanges();
        return true;
    }

    public String getUserData(Guid token)
    {
        var userData = _databaseContext.UserModels.Join(
            _databaseContext.UserLogins,
            user => user.UserID, // Matching user.UserID
            login => login.UserID, // To login.UserID
            (user, login) => new
            {
                user.Username,
                user.Email,
                login.LogInDate,
                login.LogInDevice
            }
        );
        return JsonSerializer.Serialize(userData);
    }

    public Boolean setUserData(Guid token, string username, string email)
    {
        var loginToken = _databaseContext.UserLogins.First(ul => ul.Token == token);
        var userModel = _databaseContext.UserModels.First(um => um.UserID == loginToken.UserID);
        userModel.Username = username;
        userModel.Email = email;
        _databaseContext.SaveChanges();
        return true;
    }

    public static void Main()
    {
        DbContextOptionsBuilder<UserDataBaseContext> options = new DbContextOptionsBuilder<UserDataBaseContext>();
        var cpath = "/Users/jakubwalica/RiderProjects/MpkWroclaw";
        if (!Directory.Exists(cpath + "/database/sql"))
            Directory.CreateDirectory(cpath + "/database/sql");
        cpath += "/database/sql";
        Console.WriteLine(cpath);
        if (!File.Exists(cpath + "/user.sqlite"))
            File.Create(cpath + "/user.sqlite");
        options.UseSqlite($"Data Source={cpath}/user.sqlite");
        UserSingleton userSingleton = new UserSingleton();
        
        for(int i = 0;i<1e5;i++)
        {
            var password = randomString(26);
            var username = randomString(10);
            userSingleton.AddUser(new UserModel
            {
                UserID = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                Username = username,
                Password = password,
                Name = randomString(4),
                Email = randomString(2)+"@"+randomString(10)+"."+randomString(3),
            });
            
            var guid = userSingleton.LoginUser(username, password, "mac", "192.168.6.232");
            
            var success = userSingleton.VerifyToken(guid);
    
            if (!success)
            {
                Console.WriteLine("Login Failed");
                throw new Exception("SHIIT");
            }
        }
    }

    private static String randomString(int size)
    {
        var s = "";
        for (int i = 0; i < size; i++)
            s += (char)new Random().Next('a', 'z');
        return s;
    }

    public bool setPassword(Guid token, string oldPassword, string newPassword)
    {
        using var context = _db.CreateDbContext();
        var userID = context.UserLogins.First(ul => ul.Token == token).UserID;
        var userData = context.UserModels.First(u => u.UserID == userID);

        if (userData.Password != oldPassword)
            return false;

        userData.Password = newPassword;
        context.SaveChanges();
        return true;
    }
}