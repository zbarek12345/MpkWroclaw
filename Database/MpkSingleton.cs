using System.Text.Json;

namespace MPKWrocław.Database;

public class MpkSingleton
{
    private MpkDatabaseContext _databaseContext;
    public MpkSingleton()
    {
        _databaseContext = new MpkDatabaseContext();
    }
    public string GetStopList()
    {
        return JsonSerializer.Serialize(_databaseContext.Stops.Select(p => new { p.stop_id, p.stop_lat, p.stop_lon, p.stop_name}));
    }
    public string vehiclesForStop(int stop_id)
    {
        var vehicles = "";
        return null;
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
        MpkSingleton mpkSingleton = new MpkSingleton();
        // Console.Write(mpkSingleton.vehiclesForStop(235));
        Console.WriteLine(mpkSingleton.GetStopList());
    }
}