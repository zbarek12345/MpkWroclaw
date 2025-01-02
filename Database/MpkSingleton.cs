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
        var vehicles = _databaseContext.StopTimes
            .Where(st => st.stop_id == stop_id) // Filter StopTimes by stop_id
            .Join(
                _databaseContext.Trips,
                stopTime => stopTime.trip_id,   // Join StopTimes.trip_id to Trips.trip_id
                trip => trip.trip_id,
                (stopTime, trip) => new 
                {
                    RouteId = trip.route_id,
                    TripHeadsign = trip.trip_headsign
                }
            )
            .ToList();
        
        return JsonSerializer.Serialize(vehicles);
    }

    public string departuresForVehicle(string route_id, int stop_id)
    {
        var departures = _databaseContext.StopTimes
            .Where(st => st.stop_id == stop_id) // Filter StopTimes by stop_id
            .Join(
                _databaseContext.Trips,
                stopTime => stopTime.trip_id,   // Join StopTimes.trip_id to Trips.trip_id
                trip => trip.trip_id,
                (stopTime, trip) => new 
                {
                    stopTime.departure_time,
                    trip.route_id,
                    trip.trip_headsign
                }
            )
            .Where(result => result.route_id == route_id) // Filter by route_id
            .Select(result => new
            {
                DepartureTime = result.departure_time,
                RouteId = result.route_id,
                TripHeadsign = result.trip_headsign
            })
            .ToList();
        
        return JsonSerializer.Serialize(departures);
    }

    public string departuresClosestTen(int stop_id, int page = 1)
    {
        int pageSize = 10; // Number of departures per page
    
        var departures = _databaseContext.StopTimes
            .Where(st => st.stop_id == stop_id) // Filter StopTimes by stop_id
            .Join(
                _databaseContext.Trips,
                stopTime => stopTime.trip_id,   // Join StopTimes.trip_id to Trips.trip_id
                trip => trip.trip_id,
                (stopTime, trip) => new 
                {
                    stopTime.departure_time,
                    trip.route_id,
                    trip.trip_headsign
                }
            )
            .OrderBy(result => result.departure_time) // Sort by departure time
            .Skip((page - 1) * pageSize) // Skip previous pages
            .Take(pageSize) // Take only 10 results
            .Select(result => new
            {
                DepartureTime = result.departure_time,
                RouteId = result.route_id,
                TripHeadsign = result.trip_headsign
            })
            .ToList();
    
        return JsonSerializer.Serialize(departures);
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