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

        public override string ToString()
        {
            return Hours + ":" + Minutes + ":" + Seconds;
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

            var year = int.Parse(source.Substring(0, 4));
            var month = int.Parse(source.Substring(4, 2));
            var day = int.Parse(source.Substring(6, 2));

            return new Date { Year = year, Month = month, Day = day };
        }
        
        public override string ToString()
        {
            return Year +""+ Month +""+ Day;
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
    }

    public class Calendar_Dates
    {
        public Date date;
        public int exception_type;
        public string service_id;
    }

    public class Contracts_Ext
    {
        public Date contract_conclusion_date;
        public string contract_desc;
        public Date contract_end_date;
        public string contract_id;
        public string contract_number;
        public string contract_op_id;
        public string contract_operators_name;
        public string contract_short_name;
        public Date contract_start_date;
    }

    public class Control_Stops
    {
        public int stop_id;
        public int variant_id;
    }

    public class Feed_Info
    {
        public Date feed_end_date;
        public string feed_lang;
        public string feed_publisher_name;
        public string feed_publisher_url;
        public Date feed_start_date;
    }


    public class Route_Types
    {
        public int route_type2_id;
        public string route_type2_name;
    }

    public class Routes
    {
        public int agency_id;
        public string route_desc;
        public int route_id;
        public string route_long_name;
        public string route_short_name;
        public int route_type;
        public int route_type2_id;
        public Date valid_from;
        public Date valid_until;
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
        public Hour arrival_time;
        public Hour departure_time;
        public int drop_off_type;
        public int pickup_type;
        public int stop_id;
        public int stop_sequence;
        public int trip_id;
    }

    public class Stops
    {
        public string stop_code;
        public string stop_desc;
        public int stop_id;
        public string stop_lat;
        public string stop_lon;
        public string stop_name;
    }

    public class Trips
    {
        public int brigade_id;
        public int direction_id;
        public int route_id;
        public int service_id;
        public int shape_id;
        public string trip_headsign;
        public int trip_id;
        public int variant_id;
        public int vehicle_id;
    }

    public class Variants
    {
        public int? disjoin_stop_id;
        public int equiv_main_variant_id;
        public bool is_main;
        public int? join_stop_id;
        public int variant_id;
    }

    public class Vehicle_Types
    {
        public string vehicle_type_description;
        public int vehicle_type_id;
        public string vehicle_type_name;
        public string vehicle_type_symbol;
    }
}