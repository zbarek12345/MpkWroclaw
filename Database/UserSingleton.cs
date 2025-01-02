using MPKWrocław.Models;

namespace MPKWrocław.Database;

public class UserSingleton
{
    private UserDataBaseContext _databaseContext;
    UserSingleton(UserDataBaseContext databaseContext)
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
            _databaseContext.UserLogins.Add(new UserLogins {UserID = dataModel.UserID, Token = guid, LogInDate = DateTime.Now, LogOutTime = DateTime.Now.AddDays(1)});
            return guid;
        }
        return Guid.Empty;
    }

    public Boolean VerifyToken(Guid token)
    {
        var loginData = _databaseContext.UserLogins.FirstOrDefault(ul => ul.Token == token);
        
        if(loginData == null || loginData.LogOutTime<loginData.LogInDate)
            return false;
        loginData.LogOutTime = DateTime.Now.AddDays(1);
        return true;
    }
    

    public static void Main()
    {
        using var dbContext = new UserDataBaseContext();
        dbContext.Database.EnsureCreated();
        UserSingleton userSingleton = new UserSingleton(dbContext);
    }
}