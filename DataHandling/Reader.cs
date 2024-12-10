using System.IO.Compression;
using System.Reflection;
using MPKWrocław.Database;
using MPKWrocław.Models;

namespace MPKWrocław.DataHandling;
    

public class Reader
{
    private bool _updateLock = false;
    private static string _tmp = Directory.GetCurrentDirectory()+"/tmp";
    private static string _localString = _tmp+"/OtwartyWroclaw_rozklad_jazdy_GTFS";
    private MpkDatabaseContext _context;
    public void DownloadSource()
    {
        Uri mpkUrl= new Uri(
            "https://www.wroclaw.pl/open-data/87b09b32-f076-4475-8ec9-6020ed1f9ac0/OtwartyWroclaw_rozklad_jazdy_GTFS.zip");

        HttpClient httpClient = new HttpClient();

        var stream = httpClient.GetStreamAsync(mpkUrl).GetAwaiter().GetResult();

        var archive = new ZipArchive(stream);
        
        if(Directory.Exists(_localString))
        {
            Directory.Delete(_localString, true);
            Directory.CreateDirectory(_localString);
        }
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
        bool valid = true;

        for (int i = 0; i < fields.Length; i++)
        {
            if (i >= data.Length || string.IsNullOrWhiteSpace(data[i]))
            {
                // Jeśli brakuje danych, ustaw na null (dla typów Nullable) i przejdź dalej
                if (Nullable.GetUnderlyingType(fields[i].FieldType) != null || !fields[i].FieldType.IsValueType)
                {
                    fields[i].SetValue(instance, null);
                }
                else
                {
                    // Jeśli typ nie pozwala na null i dane są brakujące, oznacz obiekt jako nieprawidłowy
                    valid = false;
                    break;
                }
                continue;
            }

            try
            {
                var cleanValue = data[i].Replace("\"", "").Replace("\r", "").Trim();
                object convertedValue;

                if (fields[i].FieldType == typeof(MpkDataModels.Date))
                {
                    convertedValue = MpkDataModels.Date.FromString(cleanValue);
                }
                else if (fields[i].FieldType == typeof(MpkDataModels.Hour))
                {
                    convertedValue = MpkDataModels.Hour.FromString(cleanValue);
                }
                else if (fields[i].FieldType == typeof(bool))
                {
                    convertedValue = cleanValue != "0";
                }
                else
                {
                    convertedValue = Convert.ChangeType(cleanValue, fields[i].FieldType);
                }

                fields[i].SetValue(instance, convertedValue);
            }
            catch
            {
                valid = false; // Błąd konwersji - oznacz obiekt jako nieprawidłowy
                break;
            }
        }

        if (valid)
        {
            lst.Add(instance);
        }
    }

    return lst;
}


    public bool isLocked()
    {
        return _updateLock;
    }

    private bool IsValid<T>(T obj)
    {
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(obj);

            // If a property is nullable and has no value, continue
            if (value == null && Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                continue;
            }

            // If a property is of a reference type (excluding string) and has no value, mark invalid
            if (value == null && !property.PropertyType.IsPrimitive && typeof(string) != property.PropertyType)
            {
                return false;
            }

            // Optionally, further validation logic can be added here for specific properties
        }

        return true;
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
        foreach (var agency in ReadData<MPkDataModelsSimplified.Agency>(_localString+"/agency.txt"))
        {
            if (IsValid(agency))
            {
                dbContext.Agencies.Add(agency);
            }
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.Calendars.ToList();
        _context.Calendars.RemoveRange( (List<MpkDataModels.Calendar>)allRecords);
        foreach (var calendar in ReadData<MPkDataModelsSimplified.Calendar>(_localString+"/calendar.txt"))
        {
            if (IsValid(calendar))
            {
                dbContext.Calendars.Add(calendar);
            }
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.CalendarDatesEnumerable.ToList();
        _context.CalendarDatesEnumerable.RemoveRange( (List<MpkDataModels.Calendar_Dates>)allRecords);
        foreach (var calendar_dates in ReadData<MPkDataModelsSimplified.Calendar_Dates>(_localString+"/calendar_dates.txt"))
        {
            if (IsValid(calendar_dates))
            {
                dbContext.CalendarDatesEnumerable.Add(calendar_dates);
            }
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.ContractsExts.ToList();
        _context.ContractsExts.RemoveRange( (List<MpkDataModels.Contracts_Ext>)allRecords);
        foreach (var contractsExt in ReadData<MpkDataModels.Contracts_Ext>(_localString+"/contracts_ext.txt"))
        {
            if (IsValid(contractsExt))
            {
                dbContext.ContractsExts.Add(contractsExt);
            }
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.ControlStops.ToList();
        _context.ControlStops.RemoveRange( (List<MpkDataModels.Control_Stops>)allRecords);
        foreach (var control_stop in ReadData<MpkDataModels.Control_Stops>(_localString + "/control_stops.txt"))
        {
            if (IsValid(control_stop))
            {
                dbContext.ControlStops.Add(control_stop);
            }
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.FeedInfos.ToList();
        _context.FeedInfos.RemoveRange( (List<MpkDataModels.Feed_Info>)allRecords);
        foreach (var feed_info in ReadData<MpkDataModels.Feed_Info>(_localString + "/feed_info.txt"))
        {
            if (IsValid(feed_info))
            {
                dbContext.FeedInfos.Add(feed_info);
            }
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.RouteTypes.ToList();
        _context.RouteTypes.RemoveRange( (List<MpkDataModels.Route_Types>)allRecords);
        foreach (var route_type in ReadData<MPkDataModelsSimplified.Route_Types>(_localString + "/route_types.txt"))
        {
            if (IsValid(route_type))
            {
                dbContext.RouteTypes.Add(route_type); 
            }
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.Routes.ToList();
        _context.Routes.RemoveRange( (List<MpkDataModels.Routes>)allRecords);
        foreach (var route in ReadData<MPkDataModelsSimplified.Routes>(_localString + "/routes.txt"))
        {
            if (IsValid(route))
            {
                dbContext.Routes.Add(route);
            }
        }
        
        dbContext.SaveChanges();
        
        allRecords = _context.Shapes.ToList();
        _context.Shapes.RemoveRange( (List<MpkDataModels.Shapes>)allRecords);
        foreach (var shape in ReadData<MPkDataModelsSimplified.Shapes>(_localString + "/shapes.txt"))
        {
            if (IsValid(shape))
            {
               dbContext.Shapes.Add(shape);
            }
        }
        
        dbContext.SaveChanges();
        //
        // allRecords = _context.StopTimes.ToList();
        // _context.StopTimes.RemoveRange( (List<MpkDataModels.Stop_Times>)allRecords);
        // foreach (var stop_time in ReadData<MPkDataModelsSimplified.Stop_Times>(_localString + "/stop_times.txt"))
        // {
        //     if (IsValid(stop_time))
        //     {
        //         dbContext.StopTimes.Add(stop_time);
        //     }
        // }
        //
        // dbContext.SaveChanges();
        
        
        allRecords = _context.Stops.ToList();
        _context.Stops.RemoveRange( (List<MpkDataModels.Stops>)allRecords);
        foreach (var stop in ReadData<MPkDataModelsSimplified.Stops>(_localString + "/stops.txt"))
        {
            if (IsValid(stop))
            {
                dbContext.Stops.Add(stop); 
            }
        }

        dbContext.SaveChanges();
        
        allRecords = _context.Variants.ToList();
        _context.Variants.RemoveRange( (List<MpkDataModels.Variants>)allRecords);
        foreach (var variant in ReadData<MPkDataModelsSimplified.Variants>(_localString + "/variants.txt"))
        {
            if (IsValid(variant))
            {
                dbContext.Variants.Add(variant);
            }
            
        }

        dbContext.SaveChanges();
        
        allRecords = _context.Trips.ToList();
        _context.Trips.RemoveRange( (List<MpkDataModels.Trips>)allRecords);
        foreach (var trip in ReadData<MPkDataModelsSimplified.Trips>(_localString + "/trips.txt"))
        {
            if (IsValid(trip))
            {
                dbContext.Trips.Add(trip); 
            }
        }

        dbContext.SaveChanges();
    
        allRecords = _context.VehicleTypes.ToList();
        _context.VehicleTypes.RemoveRange( (List<MpkDataModels.Vehicle_Types>)allRecords);
        foreach (var vehicle_type in ReadData<MpkDataModels.Vehicle_Types>(_localString + "/vehicle_types.txt"))
        {
            if (IsValid(vehicle_type))
            {
                dbContext.VehicleTypes.Add(vehicle_type);
            }
            
        }

        dbContext.SaveChanges();
        
        #endregion
        
    }
    
    
    

    public static void Main()
    {
        Reader r = new Reader(new MpkDatabaseContext());
    }
}