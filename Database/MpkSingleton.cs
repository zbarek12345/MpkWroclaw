using System.Text.Json;
using MPKWrocław.Models;

namespace MPKWrocław.Database;

public class MpkSingleton
{
    private readonly MpkDatabaseContext _databaseContext;

    // Inject MpkDatabaseContext via constructor
    public MpkSingleton(MpkDatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
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
            .Distinct()
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

        var availableServices = _databaseContext.Calendars.ToList().Where(r => r.availableToday()).Select(s => s.service_id);
        
        var departures = _databaseContext.StopTimes
            .Where(st => st.stop_id == stop_id && st.departure_time > MpkDataModels.Hour.Now()) // Filter StopTimes by stop_id
            .Join(
                _databaseContext.Trips,
                stopTime => stopTime.trip_id,   // Join StopTimes.trip_id to Trips.trip_id
                trip => trip.trip_id,
                (stopTime, trip) => new 
                {
                    stopTime.departure_time,
                    trip.route_id,
                    trip.trip_headsign,
                    trip.trip_id,
                    trip.service_id
                }
            )
            .OrderBy(result => result.departure_time) // Sort by departure time
            .Skip((page - 1) * pageSize) // Skip previous pages
            .Distinct()
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

    public string getShape(string trip_id)
    {
        var shapeID = _databaseContext.Trips.First(trip => trip.trip_id == trip_id).shape_id; 
        var shape = _databaseContext.Shapes.Where( s => s.shape_id == shapeID) 
            .Select(result => new
            {
                Latitude = result.shape_pt_lat.ToString(),
                Longitude = result.shape_pt_lon.ToString(),
            })
            .ToList();
        return JsonSerializer.Serialize(shape);
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
    public string getStopsInfo()
    {
        var stops = _databaseContext.Stops.ToList();
        List<StopInfo> infoList = new List<StopInfo>();
        foreach (var stop in stops)
        {
            StopInfo stopInfo = new StopInfo(stop.stop_id, stop.stop_name, stop.stop_lat, stop.stop_lon);
            infoList.Add(stopInfo);
        }

        return JsonSerializer.Serialize(infoList);
    }

    // public string getNextVehicles(int stop_id, int count)
    // {
    //     var trips = _databaseContext.StopTimes.Where(e => e.stop_id == stop_id && e.arrival_time>=).Select(t => t.trip_id).ToList();
    // }

    // public static void Main()
    // {
    //     MpkSingleton mpkSingleton = new MpkSingleton();
    //     // Console.Write(mpkSingleton.vehiclesForStop(235));
    //     Console.WriteLine(mpkSingleton.GetStopList());
    // }
}