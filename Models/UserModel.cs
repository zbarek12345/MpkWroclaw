namespace MPKWrocław.Models;

public class UserModel
{
    public Guid UserID;
    public DateTime CreationDate;
    public string Username;
    public string Name;
    public string Password;
}

public class UserLogins
{
    public Guid UserID;
    public string LogInDevice;
    public string LogInIp;
    public DateTime LogInDate;
    public DateTime LogOutTime;
}