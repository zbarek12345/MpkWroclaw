namespace MPKWrocław.Database;

public class MpkSingleton
{
    private MpkDatabaseContext _databaseContext;
    MpkSingleton(MpkDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
}