namespace MPKWrocław.Database;

public class MpkSingleton
{
    private MpkDatabaseContext _databaseContext;
    MpkSingleton(MpkDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public string vehiclesForStop(int stop_id)
    {
        var stopName = _databaseContext.Stops.First(e => e.stop_id == stop_id).stop_name;
        var trips = _databaseContext.StopTimes.Where(e => e.stop_id == stop_id);
        
        var tripIds = trips.Select(t => t.trip_id).ToList();
        
        var uniqueRouteIds = _databaseContext.Trips
            .Where(r => tripIds.Contains(r.trip_id))
            .Select(r => r.route_id)
            .Distinct()
            .ToList();
        var uniqueRouteHeadisgns = _databaseContext.Trips
            .Where(r => tripIds.Contains(r.trip_id))
            .Select(r => r.trip_headsign)
            .Distinct()
            .ToList();

        var retS = stopName + ":\n"; 
        
        for (int i = 0; i < uniqueRouteIds.Count; i++)
        {
            retS += uniqueRouteIds[i] + " -> " + uniqueRouteHeadisgns[i]+'\n';
        }

        return retS;
    }
    
    public List<string> getRoutesForStop(int stop_id)
    {
        var trips = _databaseContext.StopTimes.Where(e => e.stop_id == stop_id).Select(t => t.trip_id).ToList();

        var routes = _databaseContext.Trips.Where(t => trips.Contains(t.trip_id))
            .Select(r => r.route_id)
            .Distinct()
            .ToList();

        return routes;
    }

    public string getNextVehicles(int stop_id, int count)
    {
        var trips = _databaseContext.StopTimes.Where(e => e.stop_id == stop_id && e.arrival_time>=).Select(t => t.trip_id).ToList();
    }

    public static void Main()
    {
        MpkSingleton mpkSingleton = new MpkSingleton(new MpkDatabaseContext());
        Console.Write(mpkSingleton.vehiclesForStop(235));
    }
}