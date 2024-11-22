using Microsoft.EntityFrameworkCore;
using MPKWrocław.Models;

namespace MPKWrocław.Database;

public class MpkDatabaseContext:DbContext
{
    public DbSet<MpkDataModels.Agency> Agencies { get; set; }
    public DbSet<MpkDataModels.Calendar> Calendars { get; set; }
    public DbSet<MpkDataModels.Calendar_Dates> CalendarDatesEnumerable { get; set; }
    public DbSet<MpkDataModels.Contracts_Ext> ContractsExts { get; set; }
    public DbSet<MpkDataModels.Control_Stops> ControlStops { get; set; }
    public DbSet<MpkDataModels.Feed_Info> FeedInfos { get; set; }
    public DbSet<MpkDataModels.Route_Types> RouteTypes { get; set; }
    public DbSet<MpkDataModels.Routes> Routes { get; set; }
    public DbSet<MpkDataModels.Shapes> Shapes { get; set; }
    public DbSet<MpkDataModels.Stop_Times> StopTimes { get; set; }
    public DbSet<MpkDataModels.Stops> Stops { get; set; }
    public DbSet<MpkDataModels.Trips> Trips { get; set; }
    public DbSet<MpkDataModels.Variants> Variants { get; set; }
    public DbSet<MpkDataModels.Vehicle_Types> VehicleTypes { get; set; }
    
    // ... Add other DBSets for each of your models

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=C:\\Users\\Wiktor\\RiderProjects\\MPKWrocław\\Database\\mpk.sqlite");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MpkDataModels.Agency>(entity =>
        {
            entity.Property(a => a.agency_id)
                .HasColumnName("agency_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.agency_name)
                .HasColumnName("agency_name")
                .HasColumnType("varchar(256)"); // set data type to varchar(256)
            
            entity.Property(a => a.agency_url)
                .HasColumnName("agency_url")
                .HasColumnType("varchar(256)"); // set data type to varchar(256)
            
            entity.Property(a => a.agency_timezone)
                .HasColumnName("agency_timezone")
                .HasColumnType("varchar(64)"); // set data type to varchar(64)
            
            entity.Property(a => a.agency_phone)
                .HasColumnName("agency_phone")
                .HasColumnType("varchar(32)"); // set data type to varchar(32)
            
            entity.Property(a => a.agency_lang)
                .HasColumnName("agency_lang")
                .HasColumnType("varchar(5)");
            
            entity.HasKey(a => a.agency_id);
        });
        // set data type to varchar(5)

        modelBuilder.Entity<MpkDataModels.Calendar>(entity =>
        {   
            entity.Property(a => a.service_id)
                .HasColumnName("service_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.monday)
                .HasColumnName("monday")
                .HasColumnType("integer");
            
            entity.Property(a => a.tuesday)
                .HasColumnName("tuesday")
                .HasColumnType("integer");
            
            entity.Property(a => a.wednesday)
                .HasColumnName("wednesday")
                .HasColumnType("integer");
            
            entity.Property(a => a.thursday)
                .HasColumnName("thursday")
                .HasColumnType("integer");
            
            entity.Property(a => a.friday)
                .HasColumnName("friday")
                .HasColumnType("integer");

            entity.Property(a => a.saturday)
                .HasColumnName("saturday")
                .HasColumnType("integer");
            
            entity.Property(a => a.sunday)
                .HasColumnName("sunday")
                .HasColumnType("integer");
            
            entity.Property(a => a.start_date)
                .HasColumnName("start_date")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));
            
            entity.Property(a => a.end_date)
                .HasColumnName("end_date")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));

            entity.HasKey(a => a.service_id);
        });

        modelBuilder.Entity<MpkDataModels.Calendar_Dates>(entity =>
        {
            entity.Property(a=> a.service_id)
                .HasColumnName("service_id")
                .HasColumnType("integer");

            entity.Property(a => a.date)
                .HasColumnName("date")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));;
            
            entity.Property(a => a.exception_type)
                .HasColumnName("exception_type")
                .HasColumnType("integer");

            entity.HasNoKey();
        });

        modelBuilder.Entity<MpkDataModels.Contracts_Ext>(entity =>
        {
            entity.Property(a => a.contract_id)
                .HasColumnName("contract_id")
                .HasColumnType("integer");

            entity.Property(a => a.contract_conclusion_date)
                .HasColumnName("contract_conclusion_date")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));

            entity.Property(a => a.contract_start_date)
                .HasColumnName("contract_start_date")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));
            
            entity.Property(a => a.contract_end_date)
                .HasColumnName("contract_end_date")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));
            
            entity.Property(a => a.contract_number)
                .HasColumnName("contract_number")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.contract_short_name)
                .HasColumnName("contract_short_name")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.contract_operators_name)
                .HasColumnName("contract_operators_name")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.contract_desc)
                .HasColumnName("contract_desc")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.contract_op_id)
                .HasColumnName("contract_op_id")
                .HasColumnType("varchar(64)");

            entity.HasKey(a => a.contract_id);
            
        });
        
        modelBuilder.Entity<MpkDataModels.Control_Stops>(entity =>
        {
            entity.Property(a => a.variant_id)
                .HasColumnName("variant_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.stop_id)
                .HasColumnName("stop_id")
                .HasColumnType("integer");

            entity.HasNoKey();
        });

        modelBuilder.Entity<MpkDataModels.Feed_Info>(entity =>
        {
            entity.Property(a => a.feed_publisher_name)
                .HasColumnName("feed_publisher_name")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.feed_publisher_url)
                .HasColumnName("feed_publisher_url")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.feed_lang)
                .HasColumnName("feed_lang")
                .HasColumnType("varchar(5)");
            
            entity.Property(a => a.feed_start_date)
                .HasColumnName("feed_start_date")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));
            
            entity.Property(a => a.feed_end_date)
                .HasColumnName("feed_end_date")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));

            entity.HasNoKey();
        });

        modelBuilder.Entity<MpkDataModels.Route_Types>(entity =>
        {
            entity.Property(a => a.route_type2_id)
                .HasColumnName("route_type2_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.route_type2_name)
                .HasColumnName("route_type2_name")
                .HasColumnType("varchar(32)");
            
            entity.HasKey(rt => rt.route_type2_id);
        });

        modelBuilder.Entity<MpkDataModels.Routes>(entity =>
        {
            entity.Property(a => a.route_id)
                .HasColumnName("route_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.agency_id)
                .HasColumnName("agency_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.route_short_name)
                .HasColumnName("route_short_name")
                .HasColumnType("varchar(32)");
            
            entity.Property(a => a.route_long_name)
                .HasColumnName("route_long_name")
                .HasColumnType("varchar(1024)");
            
            entity.Property(a => a.route_desc)
                .HasColumnName("route_desc")
                .HasColumnType("varchar(128)");
            
            entity.Property(a => a.route_type)
                .HasColumnName("route_type")
                .HasColumnType("integer");
            
            entity.Property(a => a.route_type2_id)
                .HasColumnName("route_type2_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.valid_from)
                .HasColumnName("valid_from")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));
            
            entity.Property(a => a.valid_until)
                .HasColumnName("valid_until")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Date.FromString(v));

            entity.HasKey(a => a.route_id);

        });

        modelBuilder.Entity<MpkDataModels.Shapes>(entity =>
        {
            entity.Property(a => a.shape_id)
                .HasColumnName("shape_id")
                .HasColumnType("integer");

            entity.Property(a => a.shape_pt_lat)
                .HasColumnName("shape_pt_lat")
                .HasColumnType("varchar(32)");

            entity.Property(a => a.shape_pt_lon)
                .HasColumnName("shape_pt_lon")
                .HasColumnType("varchar(32)");

            entity.Property(a => a.shape_pt_sequence)
                .HasColumnName("shape_pt_sequence")
                .HasColumnType("integer");

            entity.HasKey(a => a.shape_id);
        });
        
        modelBuilder.Entity<MpkDataModels.Stop_Times>(entity =>
        {
            entity.Property(a => a.trip_id)
                .HasColumnName("trip_id")
                .HasColumnType("integer");

            entity.Property(a => a.arrival_time)
                .HasColumnName("arrival_time")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Hour.FromString(v));

            entity.Property(a => a.departure_time)
                .HasColumnName("departure_time")
                .HasColumnType("varchar(32)")
                .HasConversion(v=>v.ToString(),
                    v=> MpkDataModels.Hour.FromString(v));

            entity.Property(a => a.stop_id)
                .HasColumnName("stop_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.stop_sequence)
                .HasColumnName("stop_sequence")
                .HasColumnType("integer");

            entity.Property(a => a.pickup_type)
                .HasColumnName("pickup_type")
                .HasColumnType("integer");

            entity.Property(a => a.drop_off_type)
                .HasColumnName("drop_off_type")
                .HasColumnType("integer");
            
            entity.HasKey(a => a.trip_id);
        });

        modelBuilder.Entity<MpkDataModels.Stops>(entity =>
        {
            entity.Property(a => a.stop_id)
                .HasColumnName("stop_id")
                .HasColumnType("integer");

            entity.Property(a => a.stop_code)
                .HasColumnName("stop_code")
                .HasColumnType("varchar(32)");

            entity.Property(a => a.stop_name)
                .HasColumnName("stop_name")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.stop_desc)
                .HasColumnName("stop_desc")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.stop_lat)
                .HasColumnName("stop_lat")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.stop_lon)
                .HasColumnName("stop_lon")
                .HasColumnType("varchar(64)");

            entity.HasKey(a => a.stop_id);
        });

        modelBuilder.Entity<MpkDataModels.Trips>(entity =>
        {
            entity.Property(a => a.route_id)
                .HasColumnName("route_id")
                .HasColumnType("integer");

            entity.Property(a => a.service_id)
                .HasColumnName("service_id")
                .HasColumnType("integer");

            entity.Property(a => a.trip_id)
                .HasColumnName("trip_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.trip_headsign)
                .HasColumnName("trip_headsign")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.direction_id)
                .HasColumnName("direction_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.shape_id)
                .HasColumnName("shape_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.brigade_id)
                .HasColumnName("brigade_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.vehicle_id)
                .HasColumnName("vehicle_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.variant_id)
                .HasColumnName("variant_id")
                .HasColumnType("integer");

            entity.HasKey(a => a.trip_id);
        });
        
        modelBuilder.Entity<MpkDataModels.Variants>(entity =>
        {
            entity.Property(a => a.variant_id)
                .HasColumnName("variant_id")
                .HasColumnType("integer");

            entity.Property(a => a.is_main)
                .HasColumnName("is_main")
                .HasColumnType("boolean");

            entity.Property(a => a.equiv_main_variant_id)
                .HasColumnName("equiv_main_variant_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.join_stop_id)
                .HasColumnName("join_stop_id")
                .HasColumnType("integer");
            
            entity.Property(a => a.disjoin_stop_id)
                .HasColumnName("disjoin_stop_id")
                .HasColumnType("integer");
            
            entity.HasKey(a => a.variant_id);
        });
        
        modelBuilder.Entity<MpkDataModels.Vehicle_Types>(entity =>
        {
            entity.Property(a => a.vehicle_type_id)
                .HasColumnName("vehicle_type_id")
                .HasColumnType("integer");

            entity.Property(a => a.vehicle_type_name)
                .HasColumnName("vehicle_type_name")
                .HasColumnType("varchar(64)");

            entity.Property(a => a.vehicle_type_description)
                .HasColumnName("vehicle_type_description")
                .HasColumnType("varchar(64)");
            
            entity.Property(a => a.vehicle_type_symbol)
                .HasColumnName("vehicle_type_symbol")
                .HasColumnType("varchar(64)");
            
            entity.HasKey(a => a.vehicle_type_id);
        });
    }
}