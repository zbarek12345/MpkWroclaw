
using System.IO.Compression;
using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using MPKWrocław.Models;

namespace MPKWrocław.Database
{   
    
    public static class CsvLoader
    {
        public static List<T> LoadCsv<T>(string filePath, bool hasHeader = true) where T : class, new()
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found at: {filePath}", filePath);

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = hasHeader,
                    MissingFieldFound = null,
                    HeaderValidated = null,
                    TrimOptions = TrimOptions.Trim
                };

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.TypeConverterCache.AddConverter<MpkDataModels.Hour>(new HourConverter());
                    csv.Context.TypeConverterCache.AddConverter<MpkDataModels.Date>(new DateConverter());
                    csv.Context.RegisterClassMap<AutoClassMap<T>>();
                    var records = csv.GetRecords<T>();
                    return new List<T>(records);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading CSV file {filePath}: {ex.Message}", ex);
            }
        }

        private sealed class AutoClassMap<T> : ClassMap<T> where T : class, new()
        {
            public AutoClassMap()
            {
                var type = typeof(T);

                // Map only scalar fields, ignore navigation properties
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!field.FieldType.IsClass || field.FieldType == typeof(string) || 
                        field.FieldType == typeof(MpkDataModels.Hour) || field.FieldType == typeof(MpkDataModels.Date))
                    {
                        var map = Map(type, field);
                        map.Name(field.Name);
                        map.Optional();
                    }
                }

                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.CanRead && property.CanWrite && 
                        (!property.PropertyType.IsClass || property.PropertyType == typeof(string) || 
                         property.PropertyType == typeof(MpkDataModels.Hour) || property.PropertyType == typeof(MpkDataModels.Date)))
                    {
                        var map = Map(type, property);
                        map.Name(property.Name);
                        map.Optional();
                    }
                }
            }
        }

        private class HourConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return string.IsNullOrWhiteSpace(text) 
                    ? new MpkDataModels.Hour { Hours = 0, Minutes = 0, Seconds = 0 } 
                    : MpkDataModels.Hour.FromString(text);
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return value is MpkDataModels.Hour hour ? hour.ToString() : base.ConvertToString(value, row, memberMapData);
            }
        }

        private class DateConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return string.IsNullOrWhiteSpace(text) 
                    ? new MpkDataModels.Date { Year = 0, Month = 0, Day = 0 } 
                    : MpkDataModels.Date.FromString(text);
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return value is MpkDataModels.Date date ? date.ToString() : base.ConvertToString(value, row, memberMapData);
            }
        }
    }
        
    public class DatabaseUpdater
    {
        private readonly string _url; // URL to fetch the data from
        private readonly string _tempFolderPath = Path.Combine(Path.GetTempPath(), "GTFS");
        private readonly Timer _timer;
        private readonly string _dbPath = Path.Combine(Directory.GetCurrentDirectory(),"database","sql");
        public DatabaseUpdater()
        {
            _url = "https://www.wroclaw.pl/open-data/87b09b32-f076-4475-8ec9-6020ed1f9ac0/OtwartyWroclaw_rozklad_jazdy_GTFS.zip";
            TimeSpan ttl;
            if (!File.Exists(Path.Combine(_dbPath, "mpk.sqlite")))
            {
                ttl = TimeSpan.Zero;
            }
            else
            {
                try
                {
                    var lastWriteTime = File.GetLastWriteTime(Path.Combine(_dbPath, "mpk.sqlite"));
                    var timeSinceLastUpdate = DateTime.Now - lastWriteTime;
                    ttl = timeSinceLastUpdate.TotalHours > 6 
                        ? TimeSpan.Zero 
                        : TimeSpan.FromHours(6) - timeSinceLastUpdate;
                }
                catch (IOException) // Handle file access errors
                {
                    ttl = TimeSpan.Zero; // Force refresh if file is inaccessible
                }
            }

            _timer = new Timer(async _ => 
            {
                try 
                {
                    await UpdateDatabase();
                }
                catch (Exception ex) 
                {
                    // Log the error (e.g., Serilog, Console, etc.)
                    Console.WriteLine($"Update failed: {ex.Message}");
                }
            }, null, ttl, TimeSpan.FromHours(6));
        }

        public async Task UpdateDatabase()
        {
            Console.WriteLine($"[{DateTime.Now}] Starting database update process...");

            try
            {
                // Ensure the temporary folder is clean
                if (Directory.Exists(_tempFolderPath))
                {
                    Directory.Delete(_tempFolderPath, true);
                }
                Directory.CreateDirectory(_tempFolderPath);

                // Step 1: Download the ZIP file
                Console.WriteLine($"Downloading ZIP file from {_url}...");
                using (HttpClient httpClient = new HttpClient())
                {
                    // Step 1: Download ZIP file as byte array into memory
                    byte[] zipData = await httpClient.GetByteArrayAsync(_url);
                    Console.WriteLine("Download complete.");

                    // Step 2: Extract ZIP contents from the in-memory byte array
                    Console.WriteLine($"Extracting contents to {_tempFolderPath}...");
                    using (var zipStream = new MemoryStream(zipData)) // Create MemoryStream from the byte array
                    {
                        // Extract ZIP contents to the temporary folder
                        ZipFile.ExtractToDirectory(zipStream, _tempFolderPath);
                    }
                    Console.WriteLine("Extraction completed.");
                }

                // Step 3: Process the extracted data
                Console.WriteLine("Processing extracted data...");
                ProcessExtractedData();
                Console.WriteLine("Data processing complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the update process: {ex.Message}");
            }
            finally
            {
                // Step 4: Clean up the temporary folder
                if (Directory.Exists(_tempFolderPath))
                {
                    Directory.Delete(_tempFolderPath, true);
                    Console.WriteLine($"Cleaned up temporary folder: {_tempFolderPath}");
                }
            }
        }

        private void ProcessExtractedData()
        {
            string directory = Path.GetDirectoryName(Path.Combine(_dbPath, "mpk.sqlite"));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Console.WriteLine($"Directory created at: {directory}");
            }
            
            var dbBuilder =
                new DbContextOptionsBuilder<MpkDatabaseContext>().UseSqlite(
                    $"Data Source={Path.Combine(_dbPath, "mpk.sqlite")}");
                    //.LogTo(Console.WriteLine, LogLevel.Information);
            var mpkDb = new MpkDatabaseContext(dbBuilder.Options);

            MpkDatabaseContext.databaseLock = true;
            //while (MpkDatabaseContext.databaseInstances!=1)
            //{
            //    Console.WriteLine($"Waiting for database lock... {MpkDatabaseContext.databaseInstances}");
            //    Thread.Sleep(1000);
            //}

            mpkDb.Database.EnsureDeleted();
            mpkDb.Database.EnsureCreated();
            
            var agencies = CsvLoader.LoadCsv<MpkDataModels.Agency>(Path.Combine(_tempFolderPath, "agency.txt"));
            var routeTypes =
                CsvLoader.LoadCsv<MpkDataModels.Route_Types>(Path.Combine(_tempFolderPath, "route_types.txt"));
            var calendars = CsvLoader.LoadCsv<MpkDataModels.Calendar>(Path.Combine(_tempFolderPath, "calendar.txt"));
            var shapesData =
                CsvLoader.LoadCsv<MpkDataModels.Shapes>(Path.Combine(_tempFolderPath, "shapes.txt")); // Raw Shapes data
            var variants = CsvLoader.LoadCsv<MpkDataModels.Variants>(Path.Combine(_tempFolderPath, "variants.txt"));
            var routes = CsvLoader.LoadCsv<MpkDataModels.Routes>(Path.Combine(_tempFolderPath, "routes.txt"));
            var trips = CsvLoader.LoadCsv<MpkDataModels.Trips>(Path.Combine(_tempFolderPath, "trips.txt"));
            var stopTimes =
                CsvLoader.LoadCsv<MpkDataModels.Stop_Times>(Path.Combine(_tempFolderPath, "stop_times.txt"));
            var stops = CsvLoader.LoadCsv<MpkDataModels.Stops>(Path.Combine(_tempFolderPath, "stops.txt"));

// Add to database context in order
            try
            {
                // Agencies (no dependencies)
                mpkDb.Agencies.AddRange(agencies);
                mpkDb.SaveChanges();
                Console.WriteLine("Agencies inserted.");

                // RouteTypes (no dependencies)
                mpkDb.RouteTypes.AddRange(routeTypes);
                mpkDb.SaveChanges();
                Console.WriteLine("RouteTypes inserted.");

                // Routes (depends on Agencies, RouteTypes)
                mpkDb.Routes.AddRange(routes);
                mpkDb.SaveChanges();
                Console.WriteLine("Routes inserted.");

                // Calendars (no dependencies)
                mpkDb.Calendars.AddRange(calendars);
                mpkDb.SaveChanges();
                Console.WriteLine("Calendars inserted.");

                // Shapes fix: Insert Shape first, then ShapePoints (Shapes)
                var uniqueShapeIds = shapesData.Select(s => s.shape_id).Distinct().ToList();
                var shapes = uniqueShapeIds.Select(id => new MpkDataModels.Shape { shape_id = id }).ToList();
                mpkDb.Shapes.AddRange(shapes); // Insert into Shape table
                mpkDb.SaveChanges();
                Console.WriteLine("Shape entities inserted.");

                mpkDb.ShapePoints.AddRange(shapesData); // Assuming Shapes renamed to ShapePoints
                mpkDb.SaveChanges();
                Console.WriteLine("ShapePoints inserted.");

                // Variants (no dependencies)
                mpkDb.Variants.AddRange(variants);
                mpkDb.SaveChanges();
                Console.WriteLine("Variants inserted.");

                // Trips (depends on Routes, Calendars, Shapes, Variants)
                mpkDb.Trips.AddRange(trips);
                mpkDb.SaveChanges();
                Console.WriteLine("Trips inserted.");

                // Stops (no dependencies)
                mpkDb.Stops.AddRange(stops);
                mpkDb.SaveChanges();
                Console.WriteLine("Stops inserted.");

                const int batchSize = 10000;
                for (int i = 0; i < stopTimes.Count; i += batchSize)
                {
                    var batch = stopTimes.Skip(i).Take(batchSize).ToList();
                    mpkDb.StopTimes.AddRange(batch);
                    mpkDb.SaveChanges();
                    Console.WriteLine($"Inserted batch {i / batchSize + 1}");
                }
                Console.WriteLine("StopTimes inserted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during update: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                throw; // Re-throw for further debugging if needed
            }
            finally
            {
                agencies = null;
                routeTypes = null;
                calendars = null;
                shapesData = null;
                variants = null;
                routes = null;
                trips = null;
                stopTimes = null;
                stops = null;

                mpkDb.Dispose();
                MpkDatabaseContext.databaseLock = false;
            }
        }

        // public static void Test()
        // {
        //     DatabaseUpdater updater = new DatabaseUpdater();
        //     updater.UpdateDatabase();
        // }
        
        public void StopUpdater()
        {
            _timer?.Dispose();
            Console.WriteLine("Database update process stopped.");
        }
    }
}
