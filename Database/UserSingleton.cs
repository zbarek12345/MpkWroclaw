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
        var path = Path.Combine(Directory.GetCurrentDirectory(),"database","sql", "user.sqlite");
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
        using var context = _db.CreateDbContext();
        var userData = context.UserModels.Join(
            context.UserLogins.Where(u => u.Token == token),
            user => user.UserID, // Matching user.UserID
            login => login.UserID, // To login.UserID
            (user, login) => new
            {
                user.Username,
                user.Email,
                login.LogInDate,
                login.LogInDevice
            }
        ).FirstOrDefault();
        return JsonSerializer.Serialize(userData);
    }

    public Boolean setUserData(Guid token, string username, string email)
    {   
        using var context = _db.CreateDbContext();
        try
        {
            var loginToken = context.UserLogins.First(ul => ul.Token == token);
            var userModel = context.UserModels.First(um => um.UserID == loginToken.UserID);
            userModel.Username = username;
            userModel.Email = email;
            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public bool updateFavorites(Guid token, string favorites_str)
    {   
        using var context = _db.CreateDbContext();
        try
        {
            var favorites = JsonSerializer.Deserialize<List<string>>(favorites_str);

            // Retrieve the user based on the provided token
            var userID = context.UserLogins.First(ul => ul.Token == token);

            // Remove favorites that are not in the new favorites list
            var existingFavorites = context.UserFavourites
                .Where(u => u.UserID == userID.UserID) // Assuming UserID is a property of UserFavourites
                .ToList();

            var favoritesToRemove = existingFavorites
                .Where(u => !favorites.Contains(u.FavouriteID))
                .ToList();

            if (favoritesToRemove.Any())
            {
                context.UserFavourites.RemoveRange(favoritesToRemove);
            }

            // Add new favorites that are not already in the database
            var favoritesToAdd = favorites
                .Where(f => existingFavorites.All(ef => ef.FavouriteID != f))
                .Select(f => new UserFavourite { UserID = userID.UserID, FavouriteID = f }) // Assuming UserFavourite has UserID and FavouriteID properties
                .ToList();

            if (favoritesToAdd.Any())
            {
                context.UserFavourites.AddRange(favoritesToAdd);
            }

            // Save changes to the database
            context.SaveChanges();
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public string loadFavorites(Guid token)
    {   
        using var context = _db.CreateDbContext();
        var ret = context.UserFavourites.Where(u => u.UserID == context.UserLogins.First(ul => ul.Token == token).UserID).Select(u => u.FavouriteID).ToList();
            
        return JsonSerializer.Serialize(ret);
    }

    public bool addFavorite(Guid token, string favoriteId)
    {
        using var context = _db.CreateDbContext();
        try
        {
            var userID = context.UserLogins.First(ul => ul.Token == token).UserID;

            // Check if the favorite already exists for the user
            var existingFavorite = context.UserFavourites
                .FirstOrDefault(uf => uf.UserID == userID && uf.FavouriteID == favoriteId);

            if (existingFavorite == null)
            {
                context.UserFavourites.Add(new UserFavourite
                {
                    UserID = userID,
                    FavouriteID = favoriteId
                });
                context.SaveChanges();
                return true; // Successfully added
            }
            else
            {
                return false; // Favorite already exists
            }
        }
        catch (Exception e)
        {
            return false; // Error while adding favorite
        }
    }

    public bool deleteFavorite(Guid token, string favoriteId)
    {
        using var context = _db.CreateDbContext();
        try
        {
            var userID = context.UserLogins.First(ul => ul.Token == token).UserID;

            var favoriteToDelete = context.UserFavourites
                .FirstOrDefault(uf => uf.UserID == userID && uf.FavouriteID == favoriteId);

            if (favoriteToDelete != null)
            {
                context.UserFavourites.Remove(favoriteToDelete);
                context.SaveChanges();
                return true; // Successfully deleted
            }
            else
            {
                return false; // Favorite not found
            }
        }
        catch (Exception e)
        {
            return false; // Error while deleting favorite
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