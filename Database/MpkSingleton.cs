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

    public struct StopInfo
    {
        public int Stop_id { get; }
        public string Stop_name { get; }
        public string Stop_lat { get; }
        public string Stop_lon { get; }

        public StopInfo(int stop_id, string stop_name, string stop_lat, string stop_lon)
        {
            Stop_id = stop_id;
            Stop_name = stop_name;
            Stop_lat = stop_lat;
            Stop_lon = stop_lon;
        }
        
    }
    public List<StopInfo> getStopsInfo()
    {
        var stops = _databaseContext.Stops.ToList();
        List<StopInfo> infoList = new List<StopInfo>();
        foreach (var stop in stops)
        {
            StopInfo stopInfo = new StopInfo(stop.stop_id, stop.stop_name, stop.stop_lat, stop.stop_lon);
            infoList.Add(stopInfo);
        }

        return infoList;
    }

    // public string getNextVehicles(int stop_id, int count)
    // {
    //     var trips = _databaseContext.StopTimes.Where(e => e.stop_id == stop_id && e.arrival_time>=).Select(t => t.trip_id).ToList();
    // }

    public static void Main()
    {
        MpkSingleton mpkSingleton = new MpkSingleton(new MpkDatabaseContext());
        // Console.Write(mpkSingleton.vehiclesForStop(235));
        List<StopInfo> infoList = mpkSingleton.getStopsInfo();
        foreach (var x in infoList)
        {
            Console.WriteLine(x.Stop_id);
            Console.WriteLine(x.Stop_name);
            Console.WriteLine(x.Stop_lon);
            Console.WriteLine(x.Stop_lat);
        }
        
    }
}