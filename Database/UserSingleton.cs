using Microsoft.EntityFrameworkCore;
using MPKWrocław.Models;

namespace MPKWrocław.Database;

public class UserSingleton
{
    private readonly UserDataBaseContext _databaseContext;

    // Public constructor for DI to inject UserDataBaseContext
    public UserSingleton(UserDataBaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public void AddUser(UserModel userModel)
    {
        _databaseContext.UserModels.Add(userModel);
        _databaseContext.SaveChanges();
    }

    public Guid LoginUser(string username, string password)
    {
        var dataModel = _databaseContext.UserModels
            .FirstOrDefault(um => um.Username == username && um.Password == password);
            
        if (dataModel != null)
        {
            var guid = Guid.NewGuid();
            _databaseContext.UserLogins.Add(new UserLogins { UserID = dataModel.UserID, Token = guid, LogInDate = DateTime.Now, LogOutTime = DateTime.Now.AddDays(1) });
            _databaseContext.SaveChanges();
            return guid;
        }
        return Guid.Empty;
    }

    public bool VerifyToken(Guid token)
    {
        var loginData = _databaseContext.UserLogins.FirstOrDefault(ul => ul.Token == token);
            
        if (loginData == null || loginData.LogOutTime < loginData.LogInDate)
            return false;
        loginData.LogOutTime = DateTime.Now.AddDays(1);
        _databaseContext.SaveChanges();
        return true;
    }

    // public static void Main()
    // {
    //     DbContextOptionsBuilder<UserDataBaseContext> options = new DbContextOptionsBuilder<UserDataBaseContext>();
    //     // var cpath = "/Users/jakubwalica/RiderProjects/MpkWroclaw";
    //     // if (!Directory.Exists(cpath + "/database"))
    //     //     Directory.CreateDirectory(cpath + "/database");
    //     // cpath += "/database";
    //     // Console.WriteLine(cpath);
    //     // if (!File.Exists(cpath + "/mpk.sqlite"))
    //     //     File.Create(cpath + "/mpk.sqlite");
    //     // options.UseSqlite($"Data Source={cpath}/mpk.sqlite");
    //     using var dbContext = new UserDataBaseContext(options.Options);
    //     dbContext.Database.EnsureCreated();
    //     UserSingleton userSingleton = new UserSingleton(dbContext);
    // }
}