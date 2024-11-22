namespace MPKWrocław.Database;

public class MpkSingleton
{
    private MpkDatabaseContext _databaseContext;
    MpkSingleton(MpkDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public string[] vehiclesOnStop(int stop_id)
    {
        var vehicles = _databaseContext.Trips;
        return null;
    }
    
}