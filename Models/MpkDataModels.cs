namespace MPKWrocław.Models;

public class MpkDataModels
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
        public DateTime end_date;
        public int friday;
        public int monday;
        public int saturday;
        public int service_id;
        public DateTime start_date;
        public int sunday;
        public int thursday;
        public int tuesday;
        public int wednesday;
    }

    public class Calendar_Dates
    {
        public string date;
        public int exception_type;
        public string service_id;
    }

    public class Contracts_Ext
    {
        public DateTime contract_conclusion_date;
        public string contract_desc;
        public DateTime contract_end_date;
        public string contract_id;
        public string contract_number;
        public string contract_op_id;
        public string contract_operators_name;
        public string contract_short_name;
        public DateTime contract_start_date;
    }

    public class Control_Stops
    {
        public int stop_id;
        public int variant_id;
    }

    public class Feed_Info
    {
        public DateTime feed_end_date;
        public string feed_lang;
        public string feed_publisher_name;
        public string feed_publisher_url;
        public DateTime feed_start_date;
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
        public DateTime valid_from;
        public DateTime valid_until;
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
        public DateTime arrival_time;
        public string departure_time;
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