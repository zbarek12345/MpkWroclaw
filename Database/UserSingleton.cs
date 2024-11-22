namespace MPKWrocław.Database;

public class UserSingleton
{
    private UserDataBaseContext _databaseContext;
    UserSingleton(UserDataBaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    

    public static void Main()
    {
        using var dbContext = new UserDataBaseContext();
        dbContext.Database.EnsureCreated();
        UserSingleton userSingleton = new UserSingleton(dbContext);
    }
}