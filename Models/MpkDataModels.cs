using Microsoft.VisualBasic.CompilerServices;

namespace MPKWrocław.Models;

public class MPkDataModelsSimplified
{   
    public class Agency
    {
        public int agency_id;
        public string agency_lang;
        public string agency_name;
        public string agency_phone;
        public string agency_timezone;
        public string agency_url;
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
        public int agency_id;
        public string agency_lang;
        public string agency_name;
        public string agency_phone;
        public string agency_timezone;
        public string agency_url;
        
        public ICollection<Routes> Routes { get; set; }

        public static implicit operator Agency(MPkDataModelsSimplified.Agency a) => new Agency
            { agency_id = a.agency_id, agency_lang = a.agency_lang, agency_name = a.agency_name, agency_phone = a.agency_phone, agency_timezone = a.agency_timezone, agency_url = a.agency_url, Routes = null };

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
        public Date start_date;
        public Date end_date;
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
        
        public ICollection<Calendar_Dates> CalendarDates { get; set; }
        public ICollection<Trips> Trips { get; set; }

        public static implicit operator Calendar(MPkDataModelsSimplified.Calendar a) => new Calendar
        {
            service_id = a.service_id, monday = a.monday, tuesday = a.tuesday, wednesday = a.wednesday,
            thursday = a.thursday, friday = a.friday, saturday = a.saturday, sunday = a.sunday, start_date = a.start_date,
            end_date = a.end_date,
            CalendarDates = null, Trips = null
        };
    }

    public class Calendar_Dates
    {
        public int service_id;
        public Date date;
        public int exception_type;
        
        public Calendar Calendar { get; set; }

        public static implicit operator Calendar_Dates(MPkDataModelsSimplified.Calendar_Dates a) => new Calendar_Dates{
            date = a.date, exception_type = a.exception_type, service_id = a.service_id, Calendar = null
        };
    }

    public class Contracts_Ext
    {
        public string contract_id;
        public Date contract_conclusion_date;
        public Date contract_start_date;
        public Date contract_end_date;
        public string contract_number;
        public string contract_short_name;
        public string contract_operators_name;
        public string contract_desc;
        public string contract_op_id;
    }

    public class Control_Stops
    {
        public int variant_id;
        public int stop_id;
    }

    public class Feed_Info
    {   
        public string feed_publisher_name;
        public string feed_publisher_url;
        public string feed_lang;
        public Date feed_start_date;
        public Date feed_end_date;
    }
    
    public class Route_Types
    {
        public int route_type2_id;
        public string route_type2_name;
            
        public ICollection<Routes> Routes { get; set; }
        
        public static implicit operator Route_Types(MPkDataModelsSimplified.Route_Types a) => new Route_Types{
            route_type2_id = a.route_type2_id, route_type2_name = a.route_type2_name, Routes = null
        };
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
        public Date valid_from;
        public Date valid_until;

        public Agency Agency { get; set; }
        public Route_Types RouteTypes { get; set; }
        // public ICollection<Trips> Trips { get; set; }
        public static implicit operator Routes(MPkDataModelsSimplified.Routes a) => new Routes {
            route_id = a.route_id, agency_id = a.agency_id, route_short_name = a.route_short_name,
            route_long_name = a.route_long_name,
            route_desc = a.route_desc, route_type = a.route_type, route_type2_id = a.route_type2_id,
            valid_from = a.valid_from, valid_until = a.valid_until, Agency = null, RouteTypes = null
        };
    }

    public class Shapes
    {
        public int shape_id;
        public string shape_pt_lat;
        public string shape_pt_lon;
        public int shape_pt_sequence;
        
        // public virtual ICollection<Trips> Trips { get; set; }

        public static implicit operator Shapes(MPkDataModelsSimplified.Shapes a) => new Shapes{
            shape_id = a.shape_id, shape_pt_lat = a.shape_pt_lat, shape_pt_lon = a.shape_pt_lon,
            shape_pt_sequence = a.shape_pt_sequence
        };
    }
    
    public class Stop_Times
    {
        public string trip_id;
        public Hour arrival_time;
        public Hour departure_time;
        public int stop_id;
        public int stop_sequence;
        public int pickup_type;
        public int drop_off_type;
        
        // public Stops Stops { get; set; }
        // public ICollection<Trips> Trips { get; set; }

        public static implicit operator Stop_Times(MPkDataModelsSimplified.Stop_Times a) => new Stop_Times{
            trip_id = a.trip_id, arrival_time = a.arrival_time, departure_time = a.departure_time, stop_id = a.stop_id,
            stop_sequence = a.stop_sequence, pickup_type = a.pickup_type, drop_off_type = a.drop_off_type
        };
    }

    public class Stops
    {
        public int stop_id;
        public string stop_code;
        public string stop_name;
        public string stop_lat;
        public string stop_lon;
        
        // public ICollection<Stop_Times> StopTimes { get; set; }

        public static implicit operator Stops(MPkDataModelsSimplified.Stops a) => new Stops {
            stop_id = a.stop_id, stop_code = a.stop_code, stop_name = a.stop_name,
            stop_lat = a.stop_lat, stop_lon = a.stop_lon
        };
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
       
        // public Routes Routes { get; set; }
        public Calendar Calendar { get; set; }
        // public ICollection<Stop_Times> StopTimes { get; set; }
        // public virtual Shapes Shapes { get; set; }
        public Variants Variants { get; set; }

        public static implicit operator Trips(MPkDataModelsSimplified.Trips a) => new Trips {
            route_id = a.route_id, service_id = a.service_id, trip_id = a.trip_id,
            trip_headsign = a.trip_headsign, direction_id = a.direction_id, shape_id = a.shape_id,
            brigade_id = a.brigade_id, vehicle_id = a.vehicle_id, variant_id = a.variant_id, Calendar = null, Variants = null
        };
    }

    public class Variants
    {
        public int variant_id;
        public bool is_main;
        public string equiv_main_variant_id;
        public string join_stop_id;
        public string disjoin_stop_id;
        
        public ICollection<Trips> Trips { get; set; }

        public static implicit operator Variants(MPkDataModelsSimplified.Variants a) => new Variants
        {
            variant_id = a.variant_id, equiv_main_variant_id = a.equiv_main_variant_id ?? "DEFAULT_VALUE", join_stop_id = a.join_stop_id ?? "DEFAULT_VALUE",
            disjoin_stop_id = a.disjoin_stop_id ?? "DEFAULT_VALUE", is_main = a.is_main, Trips = null
        };
    }

    public class Vehicle_Types
    {
        public int vehicle_type_id;
        public string vehicle_type_name;
        public string vehicle_type_description;
        public string vehicle_type_symbol;
    }
}