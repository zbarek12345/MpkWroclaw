using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic.CompilerServices;

namespace MPKWrocław.Models;

public class MPkDataModelsSimplified
{   
    public class Agency
    {
        public int agency_id;
        public string agency_name;
        public string agency_url;
        public string agency_timezone;
        public string agency_phone;
        public string agency_lang;
    }

    public class Calendar
    {
        public int service_id;
        public int monday;
        public int tuesday;
        public int wednesday;
        public int thursday;
        public int friday;
        public int saturday;
        public int sunday;
        public MpkDataModels.Date start_date;
        public MpkDataModels.Date end_date;
    }
    
    public class Calendar_Dates
    {
        public int service_id;
        public MpkDataModels.Date date;
        public int exception_type;
    }
    
    public class Route_Types
    {
        public int route_type2_id;
        public string route_type2_name;
    }

    public class Routes
    {
        public string route_id;
        public int agency_id;
        public string route_short_name;
        public string route_long_name;
        public string route_desc;
        public int route_type;
        public int route_type2_id;
        public MpkDataModels.Date valid_from;
        public MpkDataModels.Date valid_until;
    }

    public class Shapes
    {
        public int shape_id;
        public string shape_pt_lat;
        public string shape_pt_lon;
        public int shape_pt_sequence;
    }

    public class Stop_Times
    {
        public string trip_id;
        public MpkDataModels.Hour arrival_time;
        public MpkDataModels.Hour departure_time;
        public int stop_id;
        public int stop_sequence;
        public int pickup_type;
        public int drop_off_type;
    }

    public class Stops
    {
        public int stop_id;
        public string stop_code;
        public string stop_name;
        public string stop_lat;
        public string stop_lon;
    }

    public class Trips
    {
        public string route_id;
        public int service_id;
        public string trip_id;
        public string trip_headsign;
        public int direction_id;
        public int shape_id;
        public int brigade_id;
        public int vehicle_id;
        public int variant_id;
    }

    public class Variants
    {
        public int variant_id;
        public bool is_main;
        public string equiv_main_variant_id;
        public string join_stop_id;
        public string disjoin_stop_id;
    }
}




public class MpkDataModels
{
    public class Hour
    {
        public int Hours;
        public int Minutes;
        public int Seconds;

    public static Hour FromString(string source)
    {
        var hour = int.Parse(source.Substring(0, 2));
        var minute = int.Parse(source.Substring(3, 2));
        var second = int.Parse(source.Substring(6, 2));

        return new Hour { Hours = hour, Minutes = minute, Seconds = second };
    }

    public static Hour Now()
    {
        var time = DateTime.Now;
        return new Hour
        {
            Hours = time.Hour,
            Minutes = time.Minute,
            Seconds = time.Second
        };
    }

    #region Arithmetic Operators
    
    public static bool operator<(Hour a, Hour b)
    {
        // Comparison prioritizes hours, then minutes, then seconds
        if (a.Hours > b.Hours)
            return true;
        if (a.Hours == b.Hours && a.Minutes > b.Minutes)
            return true;
        if (a.Hours == b.Hours && a.Minutes == b.Minutes && a.Seconds > b.Seconds)
            return true;

        return false;
    }
    
    public static bool operator>(Hour a, Hour b)
    {
        // Comparison prioritizes hours, then minutes, then seconds
        if (a.Hours < b.Hours)
            return true;
        if (a.Hours == b.Hours && a.Minutes < b.Minutes)
            return true;
        if (a.Hours == b.Hours && a.Minutes == b.Minutes && a.Seconds < b.Seconds)
            return true;

        return false;
    }
    
    public static bool operator ==(Hour a, Hour b)
    {
        if (ReferenceEquals(a, b)) return true; // Both are null or same instance
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false; // One is null
        // Compare actual properties
        return a.Minutes == b.Minutes && a.Hours == a.Hours && a.Seconds == a.Seconds; // Adjust based on your properties
    }

    public static bool operator !=(Hour a, Hour b)
    {
        return !(a == b); // Use the equality operator
    }

    public static bool operator >=(Hour a, Hour b)
    {
        return a > b || a == b;
    }
    
    public static bool operator <=(Hour a, Hour b)
    {
        return a < b || a == b;
    }

    
    #endregion
    public static string fillZeroes(int value, int desiredLength)
    {
        string s = value.ToString();

        while (s.Length < desiredLength)
        {
            s = '0' + s;
        }
        return s;
    }
    
    public override string ToString()
    {
        return fillZeroes(Hours, 2) + ":" + fillZeroes(Minutes, 2) + ":" + fillZeroes(Seconds, 2);
    }
}

    public class Date
    {
        public int Year;
        public int Month;
        public int Day;

        // Method to convert from String to a Date
        public static Date FromString(string source)
        {
            int year = 0, month = 0 , day = 0;
            if(source.Length == 8)
            {
                year = int.Parse(source.Substring(0, 4));
                month = int.Parse(source.Substring(4, 2));
                day = int.Parse(source.Substring(6, 2));
            }
            else if (source.Length == 10)
            {
                year = int.Parse(source.Substring(0, 4));
                month = int.Parse(source.Substring(5, 2));
                day = int.Parse(source.Substring(8, 2));
            }

            return new Date { Year = year, Month = month, Day = day };
        }
        
        public static string fillZeroes(int value, int desiredLength)
        {
            string s = value.ToString();

            while (s.Length < desiredLength)
            {
                s = '0' + s;
            }
            return s;
        }
        
        public override string ToString()
        {
            return fillZeroes(Year,4) + fillZeroes(Month,2) +fillZeroes(Day,2);
        }
        
    }


    public class Agency
    {
        [Key]
        public int agency_id { get; set; }
        public string agency_lang { get; set; }
        public string agency_name { get; set; }
        public string agency_phone { get; set; }
        public string agency_timezone { get; set; }
        public string agency_url { get; set; }

        public virtual ICollection<Routes> Routes { get; set; } = new List<Routes>();
    }

    public class Calendar
    {
        [Key]
        public int service_id { get; set; }
        public int monday { get; set; }
        public int tuesday { get; set; }
        public int wednesday { get; set; }
        public int thursday { get; set; }
        public int friday { get; set; }
        public int saturday { get; set; }
        public int sunday { get; set; }
        public Date start_date { get; set; }
        public Date end_date { get; set; }

        public virtual ICollection<Calendar_Dates> CalendarDates { get; set; } = new List<Calendar_Dates>();
        public virtual ICollection<Trips> Trips { get; set; } = new List<Trips>();

        public bool availableToday()
        {
            var day = DateTime.Now.DayOfWeek;

            switch (day)
            {
                case DayOfWeek.Monday:
                    return monday == 1;
                case DayOfWeek.Tuesday:
                    return tuesday == 1;
                case DayOfWeek.Wednesday:
                    return wednesday == 1;
                case DayOfWeek.Thursday:
                    return thursday == 1;
                case DayOfWeek.Friday:
                    return friday == 1;
                case DayOfWeek.Saturday:
                    return saturday == 1;
                case DayOfWeek.Sunday:
                    return sunday == 1;
            }
            return false;
        }
    }

    public class Calendar_Dates
    {
        [Key]
        [Column(Order = 0)]
        public int service_id { get; set; }
        [Key]
        [Column(Order = 1)]
        public Date date { get; set; }
        public int exception_type { get; set; }

        [ForeignKey("service_id")]
        public virtual Calendar Calendar { get; set; }
    }

    public class Contracts_Ext
    {
        [Key]
        public string contract_id { get; set; }
        public Date contract_conclusion_date { get; set; }
        public Date contract_start_date { get; set; }
        public Date contract_end_date { get; set; }
        public string contract_number { get; set; }
        public string contract_short_name { get; set; }
        public string contract_operators_name { get; set; }
        public string contract_desc { get; set; }
        public string contract_op_id { get; set; }
    }

    public class Control_Stops
    {
        [Key]
        [Column(Order = 0)]
        public int variant_id { get; set; }
        [Key]
        [Column(Order = 1)]
        public int stop_id { get; set; }
    }

    public class Feed_Info
    {
        [Key]
        public string feed_publisher_name { get; set; }
        public string feed_publisher_url { get; set; }
        public string feed_lang { get; set; }
        public Date feed_start_date { get; set; }
        public Date feed_end_date { get; set; }
    }

    public class Route_Types
    {
        [Key]
        public int route_type2_id { get; set; }
        public string route_type2_name { get; set; }

        public virtual ICollection<Routes> Routes { get; set; } = new List<Routes>();
    }

    public class Routes
    {
        [Key]
        public string route_id { get; set; }
        public int agency_id { get; set; }
        public string route_short_name { get; set; }
        public string route_long_name { get; set; }
        public string route_desc { get; set; }
        public int route_type { get; set; }
        public int route_type2_id { get; set; }
        public Date valid_from { get; set; }
        public Date valid_until { get; set; }

        [ForeignKey("agency_id")]
        public virtual Agency Agency { get; set; }
        [ForeignKey("route_type2_id")]
        public virtual Route_Types RouteTypes { get; set; }
        public virtual ICollection<Trips> Trips { get; set; } = new List<Trips>();
    }
    
    public class Shape
    {
        [Key]
        public int shape_id { get; set; }

        public virtual ICollection<Shapes> ShapePoints { get; set; } = new List<Shapes>();
        public virtual ICollection<Trips> Trips { get; set; } = new List<Trips>();
    }
    
    public class Shapes
    {
        public int shape_id { get; set; }
        public string shape_pt_lat { get; set; }
        public string shape_pt_lon { get; set; }
        public int shape_pt_sequence { get; set; }

        [ForeignKey("shape_id")]
        public virtual Shape Shape { get; set; }
    }

    public class Stop_Times
    {
        [Key]
        [Column(Order = 0)]
        public string trip_id { get; set; }
        [Key]
        [Column(Order = 1)]
        public int stop_sequence { get; set; }
        public Hour arrival_time { get; set; }
        public Hour departure_time { get; set; }
        public int stop_id { get; set; }
        public int pickup_type { get; set; }
        public int drop_off_type { get; set; }

        [ForeignKey("trip_id")]
        public virtual Trips Trips { get; set; }
        [ForeignKey("stop_id")]
        public virtual Stops Stops { get; set; }
    }

    public class Stops
    {
        [Key]
        public int stop_id { get; set; }
        public string stop_code { get; set; }
        public string stop_name { get; set; }
        public string stop_lat { get; set; }
        public string stop_lon { get; set; }

        public virtual ICollection<Stop_Times> StopTimes { get; set; } = new List<Stop_Times>();
    }

    public class Trips
    {
        [Key]
        public string trip_id { get; set; }
        public string route_id { get; set; }
        public int service_id { get; set; }
        public string trip_headsign { get; set; }
        public int direction_id { get; set; }
        public int shape_id { get; set; } // References Shape.shape_id
        public int brigade_id { get; set; }
        public int vehicle_id { get; set; }
        public int variant_id { get; set; }

        [ForeignKey("route_id")]
        public virtual Routes Routes { get; set; }
        [ForeignKey("service_id")]
        public virtual Calendar Calendar { get; set; }
        [ForeignKey("shape_id")]
        public virtual Shape Shape { get; set; } // Updated to reference Shape
        [ForeignKey("variant_id")]
        public virtual Variants Variants { get; set; }
        public virtual ICollection<Stop_Times> StopTimes { get; set; } = new List<Stop_Times>();
    }


    public class Variants
    {
        [Key]
        public int variant_id { get; set; }
        public bool is_main { get; set; }
        public string equiv_main_variant_id { get; set; }
        public string join_stop_id { get; set; }
        public string disjoin_stop_id { get; set; }

        public virtual ICollection<Trips> Trips { get; set; } = new List<Trips>();
    }

    public class Vehicle_Types
    {
        [Key]
        public int vehicle_type_id { get; set; }
        public string vehicle_type_name { get; set; }
        public string vehicle_type_description { get; set; }
        public string vehicle_type_symbol { get; set; }
    }
}