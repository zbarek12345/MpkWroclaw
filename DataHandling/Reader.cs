using System.Data.Entity;
using System.Reflection;
using MPKWrocław.Models;

namespace MPKWrocław.DataHandling;
    

public class Reader
{
    private DbContext _context;
    public void ReadData(string filename)
    {
        // Code for reading data goes here
    }


    public void DownloadSource()
    {
        Uri mpkUrl= new Uri(
            "https://www.wroclaw.pl/open-data/87b09b32-f076-4475-8ec9-6020ed1f9ac0/OtwartyWroclaw_rozklad_jazdy_GTFS.zip");
    }

    public void ReadData<T>(string filePath) where T : new()
    {
        var fstream = File.ReadAllText(filePath);
        bool firstLine = true;

        foreach (var line in fstream.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (firstLine)
            {
                firstLine = false; // Skip the header line
                continue;
            }

            var data = line.Split(",");

            T instance = new T();
            PropertyInfo[] properties = typeof(T).GetProperties();

            for (int i = 0; i < properties.Length && i < data.Length; i++)
            {
                var property = properties[i];
                if (property.CanWrite)
                {
                    // Convert the data to the appropriate type and set the property
                    var convertedValue = Convert.ChangeType(data[i], property.PropertyType);
                    property.SetValue(instance, convertedValue);
                }
            }
        }
    }
    
    Reader(DbContext dbContext)
    {
        ReadData<MpkDataModels.Agency>("agency.txt");
    }
}