namespace MPKWrocław.Models;

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
        
        public ICollection<Calendar_Dates> CalendarDates { get; set; }
        public Trips Trips { get; set; }
    }

    public class Calendar_Dates
    {
        public Date date;
        public int exception_type;
        public int service_id;
        
        public Calendar Calendar { get; set; }
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
        public int stop_id;
        public string variant_id;
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

        public Routes Routes { get; set; }
    }

    public class Routes
    {
        public string route_id;
        public int agency_id;
        public string route_desc;
        public string route_long_name;
        public string route_short_name;
        public int route_type;
        public int route_type2_id;
        public Date valid_from;
        public Date valid_until;

        public Agency Agency { get; set; }
        public Route_Types RouteTypes { get; set; }
        public Trips Trips { get; set; }
    }

    public class Shapes
    {
        public string shape_id;
        public string shape_pt_lat;
        public string shape_pt_lon;
        public int shape_pt_sequence;
        
        public Trips Trips { get; set; }
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
        
        public Stops Stops { get; set; }
        public Trips Trips { get; set; }
    }

    public class Stops
    {
        public int stop_id;
        public string stop_code;
        public string stop_name;
        public string stop_lat;
        public string stop_lon;
        
        public ICollection<Stop_Times> StopTimes { get; set; }
        
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
        public string variant_id;
       
        public Routes Routes { get; set; }
        public Calendar Calendar { get; set; }
        public ICollection<Stop_Times> StopTimes { get; set; }
        public ICollection<Shapes> Shapes { get; set; }
        public ICollection<Variants> Variants { get; set; }
    }

    public class Variants
    {
        public string variant_id;
        public bool is_main;
        public string equiv_main_variant_id;
        public string join_stop_id;
        public string disjoin_stop_id;
        
        public Trips Trips { get; set; }
    }

    public class Vehicle_Types
    {
        public int vehicle_type_id;
        public string vehicle_type_name;
        public string vehicle_type_description;
        public string vehicle_type_symbol;
    }
}