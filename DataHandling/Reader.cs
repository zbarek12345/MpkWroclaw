using System.IO.Compression;
using System.Reflection;
using MPKWrocław.Database;
using MPKWrocław.Models;

namespace MPKWrocław.DataHandling;
    

public class Reader
{
    private bool _updateLock = false;
    private static string _tmp = Directory.GetCurrentDirectory()+"\\tmp";
    private static string _localString = _tmp+"\\OtwartyWroclaw_rozklad_jazdy_GTFS";
    private MpkDatabaseContext _context;
    public void DownloadSource()
    {
        Uri mpkUrl= new Uri(
            "https://www.wroclaw.pl/open-data/87b09b32-f076-4475-8ec9-6020ed1f9ac0/OtwartyWroclaw_rozklad_jazdy_GTFS.zip");

        HttpClient httpClient = new HttpClient();

        var stream = httpClient.GetStreamAsync(mpkUrl).GetAwaiter().GetResult();

        var archive = new ZipArchive(stream);
        
        archive.ExtractToDirectory(_localString);

    }

    public List<T> ReadData<T>(string filePath) where T : new()
    {
        var fstream = File.ReadAllText(filePath);
        bool firstLine = true;
        List<T> lst = new List<T>();
        foreach (var line in fstream.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (firstLine)
            {
                firstLine = false; // Skip the header line
                continue;
            }

            var data = line.Split(",");

            T instance = new T();
            FieldInfo[] fields = typeof(T).GetFields();

            for (int i = 0; i < fields.Length && i < data.Length; i++)
            {
                data[i] = data[i].Replace("\"", "").Replace("\r", "");
                var property = fields[i];
                Object convertedValue;
                if (property.FieldType == typeof(MpkDataModels.Date))
                {
                    convertedValue = MpkDataModels.Date.FromString(data[i]);
                    property.SetValue(instance, convertedValue);
                }
                else if (property.FieldType == typeof(MpkDataModels.Hour))
                {
                    convertedValue = MpkDataModels.Hour.FromString(data[i]);
                    property.SetValue(instance, convertedValue);
                }
                else if (property.FieldType == typeof(Boolean))
                {
                    convertedValue = data[i] != "0";
                    property.SetValue(instance, convertedValue);
                }
                else
                {
                    convertedValue = Convert.ChangeType(data[i], property.FieldType);
                    property.SetValue(instance, convertedValue);
                }
            }
            lst.Add(instance);
        }

        return lst;
    }

    public bool isLocked()
    {
        return _updateLock;
    }
    Reader(MpkDatabaseContext dbContext)
    {   
        
        if(!Directory.Exists(_localString) || (DateTime.Now - Directory.GetCreationTime(_localString)).Days>1)
            DownloadSource();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        _context = dbContext;
            
        #region working
        Object allRecords = _context.Agencies.ToList();
        _context.Agencies.RemoveRange( (List<MpkDataModels.Agency>)allRecords);
        foreach (var agency in ReadData<MpkDataModels.Agency>(_localString+"\\agency.txt"))
        {
            dbContext.Agencies.Add(agency);
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.Calendars.ToList();
        _context.Calendars.RemoveRange( (List<MpkDataModels.Calendar>)allRecords);
        foreach (var calendar in ReadData<MpkDataModels.Calendar>(_localString+"\\calendar.txt"))
        {
            dbContext.Calendars.Add(calendar);
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.CalendarDatesEnumerable.ToList();
        _context.CalendarDatesEnumerable.RemoveRange( (List<MpkDataModels.Calendar_Dates>)allRecords);
        foreach (var calendar_dates in ReadData<MpkDataModels.Calendar_Dates>(_localString+"\\calendar_dates.txt"))
        {
            dbContext.CalendarDatesEnumerable.Add(calendar_dates);
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.ContractsExts.ToList();
        _context.ContractsExts.RemoveRange( (List<MpkDataModels.Contracts_Ext>)allRecords);
        foreach (var contractsExt in ReadData<MpkDataModels.Contracts_Ext>(_localString+"\\contracts_ext.txt"))
        {
            dbContext.ContractsExts.Add(contractsExt);
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.ControlStops.ToList();
        _context.ControlStops.RemoveRange( (List<MpkDataModels.Control_Stops>)allRecords);
        foreach (var control_stop in ReadData<MpkDataModels.Control_Stops>(_localString + "\\control_stops.txt"))
        {
            dbContext.ControlStops.Add(control_stop);
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.FeedInfos.ToList();
        _context.FeedInfos.RemoveRange( (List<MpkDataModels.Feed_Info>)allRecords);
        foreach (var feed_info in ReadData<MpkDataModels.Feed_Info>(_localString + "\\feed_info.txt"))
        {
            dbContext.FeedInfos.Add(feed_info);
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.RouteTypes.ToList();
        _context.RouteTypes.RemoveRange( (List<MpkDataModels.Route_Types>)allRecords);
        foreach (var route_type in ReadData<MpkDataModels.Route_Types>(_localString + "\\route_types.txt"))
        {
            dbContext.RouteTypes.Add(route_type);
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.Routes.ToList();
        _context.Routes.RemoveRange( (List<MpkDataModels.Routes>)allRecords);
        foreach (var route in ReadData<MpkDataModels.Routes>(_localString + "\\routes.txt"))
        {
            dbContext.Routes.Add(route);
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.Shapes.ToList();
        _context.Shapes.RemoveRange( (List<MpkDataModels.Shapes>)allRecords);
        foreach (var shape in ReadData<MpkDataModels.Shapes>(_localString + "\\shapes.txt"))
        {
            dbContext.Shapes.Add(shape);
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.StopTimes.ToList();
        _context.StopTimes.RemoveRange( (List<MpkDataModels.Stop_Times>)allRecords);
        foreach (var stop_time in ReadData<MpkDataModels.Stop_Times>(_localString + "\\stop_times.txt"))
        {
            dbContext.StopTimes.Add(stop_time);
        }
        
        dbContext.SaveChanges();
        
        
        allRecords = _context.Stops.ToList();
        _context.Stops.RemoveRange( (List<MpkDataModels.Stops>)allRecords);
        foreach (var stop in ReadData<MpkDataModels.Stops>(_localString + "\\stops.txt"))
        {
            dbContext.Stops.Add(stop);
        }

        dbContext.SaveChanges();
        
        allRecords = _context.Trips.ToList();
        _context.Trips.RemoveRange( (List<MpkDataModels.Trips>)allRecords);
        foreach (var trip in ReadData<MpkDataModels.Trips>(_localString + "\\trips.txt"))
        {
            dbContext.Trips.Add(trip);
        }

        dbContext.SaveChanges();
        
        allRecords = _context.Variants.ToList();
        _context.Variants.RemoveRange( (List<MpkDataModels.Variants>)allRecords);
        foreach (var variant in ReadData<MpkDataModels.Variants>(_localString + "\\variants.txt"))
        {
            dbContext.Variants.Add(variant);
        }

        dbContext.SaveChanges();
    
        allRecords = _context.VehicleTypes.ToList();
        _context.VehicleTypes.RemoveRange( (List<MpkDataModels.Vehicle_Types>)allRecords);
        foreach (var vehicle_type in ReadData<MpkDataModels.Vehicle_Types>(_localString + "\\vehicle_types.txt"))
        {
            dbContext.VehicleTypes.Add(vehicle_type);
        }

        dbContext.SaveChanges();
        
        #endregion
        
    }
    
    
    

    public static void Main()
    {
        Reader r = new Reader(new MpkDatabaseContext());
    }
}