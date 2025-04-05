using System.Net.Mime;
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
      public DbSet<MpkDataModels.Shape> Shapes { get; set; }
      
      public DbSet<MpkDataModels.Shapes> ShapePoints { get; set; }
      public DbSet<MpkDataModels.Stop_Times> StopTimes { get; set; }
      public DbSet<MpkDataModels.Stops> Stops { get; set; }
      public DbSet<MpkDataModels.Trips> Trips { get; set; }
      public DbSet<MpkDataModels.Variants> Variants { get; set; }
      public DbSet<MpkDataModels.Vehicle_Types> VehicleTypes { get; set; }
      
      public static bool databaseLock =  false;
      public static int databaseInstances = 0;
          
    // ... Add other DBSets for each of your models
      public MpkDatabaseContext(DbContextOptions<MpkDatabaseContext> options)
        : base(options)
      {
            while (databaseLock)
            {
                  Thread.Sleep(1000);
            }

            databaseInstances++;
      }

      public override void Dispose()
      {
            base.Dispose();
            databaseInstances--;
      }
      
      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
        
      }
    
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          // Agency
          modelBuilder.Entity<MpkDataModels.Agency>(entity =>
          {
              entity.HasKey(a => a.agency_id);
              entity.Property(a => a.agency_id).HasColumnType("integer");
              entity.Property(a => a.agency_name).HasColumnType("varchar(256)");
              entity.Property(a => a.agency_url).HasColumnType("varchar(256)");
              entity.Property(a => a.agency_timezone).HasColumnType("varchar(64)");
              entity.Property(a => a.agency_phone).HasColumnType("varchar(32)");
              entity.Property(a => a.agency_lang).HasColumnType("varchar(5)");

              entity.HasMany(a => a.Routes)
                    .WithOne(r => r.Agency)
                    .HasForeignKey(r => r.agency_id);
          });

          // Calendar
          modelBuilder.Entity<MpkDataModels.Calendar>(entity =>
          {
              entity.HasKey(a => a.service_id);
              entity.Property(a => a.service_id).HasColumnType("integer");
              entity.Property(a => a.monday).HasColumnType("integer");
              entity.Property(a => a.tuesday).HasColumnType("integer");
              entity.Property(a => a.wednesday).HasColumnType("integer");
              entity.Property(a => a.thursday).HasColumnType("integer");
              entity.Property(a => a.friday).HasColumnType("integer");
              entity.Property(a => a.saturday).HasColumnType("integer");
              entity.Property(a => a.sunday).HasColumnType("integer");
              entity.Property(a => a.start_date)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));
              entity.Property(a => a.end_date)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));

              entity.HasMany(c => c.Trips)
                    .WithOne(t => t.Calendar)
                    .HasForeignKey(t => t.service_id);
              entity.HasMany(c => c.CalendarDates)
                    .WithOne(cd => cd.Calendar)
                    .HasForeignKey(cd => cd.service_id);
          });

          // Calendar_Dates
          modelBuilder.Entity<MpkDataModels.Calendar_Dates>(entity =>
          {
              entity.HasKey(cd => new { cd.service_id, cd.date }); // Removed exception_type from key
              entity.Property(cd => cd.service_id).HasColumnType("integer");
              entity.Property(cd => cd.date)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));
              entity.Property(cd => cd.exception_type).HasColumnType("integer");
          });

          // Contracts_Ext
          modelBuilder.Entity<MpkDataModels.Contracts_Ext>(entity =>
          {
              entity.HasKey(c => c.contract_id);
              entity.Property(c => c.contract_id).HasColumnType("varchar(64)");
              entity.Property(c => c.contract_conclusion_date)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));
              entity.Property(c => c.contract_start_date)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));
              entity.Property(c => c.contract_end_date)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));
              entity.Property(c => c.contract_number).HasColumnType("varchar(64)");
              entity.Property(c => c.contract_short_name).HasColumnType("varchar(64)");
              entity.Property(c => c.contract_operators_name).HasColumnType("varchar(64)");
              entity.Property(c => c.contract_desc).HasColumnType("varchar(64)");
              entity.Property(c => c.contract_op_id).HasColumnType("varchar(64)");
          });

          // Control_Stops
          modelBuilder.Entity<MpkDataModels.Control_Stops>(entity =>
          {
              entity.HasKey(cs => new { cs.stop_id, cs.variant_id });
              entity.Property(cs => cs.variant_id).HasColumnType("integer");
              entity.Property(cs => cs.stop_id).HasColumnType("integer");
          });

          // Feed_Info
          modelBuilder.Entity<MpkDataModels.Feed_Info>(entity =>
          {
              entity.HasKey(f => f.feed_publisher_url); // Consider if this is the best key
              entity.Property(f => f.feed_publisher_name).HasColumnType("varchar(64)");
              entity.Property(f => f.feed_publisher_url).HasColumnType("varchar(64)");
              entity.Property(f => f.feed_lang).HasColumnType("varchar(5)");
              entity.Property(f => f.feed_start_date)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));
              entity.Property(f => f.feed_end_date)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));
          });

          // Route_Types
          modelBuilder.Entity<MpkDataModels.Route_Types>(entity =>
          {
              entity.HasKey(rt => rt.route_type2_id);
              entity.Property(rt => rt.route_type2_id).HasColumnType("integer");
              entity.Property(rt => rt.route_type2_name).HasColumnType("varchar(32)");

              entity.HasMany(rt => rt.Routes)
                    .WithOne(r => r.RouteTypes)
                    .HasForeignKey(r => r.route_type2_id);
          });

          // Routes
          modelBuilder.Entity<MpkDataModels.Routes>(entity =>
          {
              entity.HasKey(r => r.route_id);
              entity.Property(r => r.route_id).HasColumnType("varchar(5)");
              entity.Property(r => r.agency_id).HasColumnType("integer");
              entity.Property(r => r.route_short_name).HasColumnType("varchar(32)");
              entity.Property(r => r.route_long_name).HasColumnType("varchar(1024)");
              entity.Property(r => r.route_desc).HasColumnType("varchar(128)");
              entity.Property(r => r.route_type).HasColumnType("integer");
              entity.Property(r => r.route_type2_id).HasColumnType("integer");
              entity.Property(r => r.valid_from)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));
              entity.Property(r => r.valid_until)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Date.FromString(v));

              entity.HasMany(r => r.Trips)
                    .WithOne(t => t.Routes)
                    .HasForeignKey(t => t.route_id);
          });

          modelBuilder.Entity<MpkDataModels.Shape>(entity =>
          {
                entity.HasKey(s => s.shape_id);
                entity.Property(s => s.shape_id).HasColumnType("integer");

                entity.HasMany(s => s.ShapePoints)
                      .WithOne(sp => sp.Shape)
                      .HasForeignKey(sp => sp.shape_id);
          });

          modelBuilder.Entity<MpkDataModels.Shapes>(entity =>
          {
                entity.HasKey(s => new { s.shape_id, s.shape_pt_sequence });
                entity.Property(s => s.shape_id).HasColumnType("integer");
                entity.Property(s => s.shape_pt_lat).HasColumnType("varchar(32)");
                entity.Property(s => s.shape_pt_lon).HasColumnType("varchar(32)");
                entity.Property(s => s.shape_pt_sequence).HasColumnType("integer");
          });
          
          // Stop_Times
          modelBuilder.Entity<MpkDataModels.Stop_Times>(entity =>
          {
              entity.HasKey(st => new { st.trip_id, st.stop_sequence });
              entity.Property(st => st.trip_id).HasColumnType("varchar(16)");
              entity.Property(st => st.arrival_time)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Hour.FromString(v));
              entity.Property(st => st.departure_time)
                    .HasColumnType("varchar(32)")
                    .HasConversion(v => v.ToString(), v => MpkDataModels.Hour.FromString(v));
              entity.Property(st => st.stop_id).HasColumnType("integer");
              entity.Property(st => st.stop_sequence).HasColumnType("integer");
              entity.Property(st => st.pickup_type).HasColumnType("integer");
              entity.Property(st => st.drop_off_type).HasColumnType("integer");

              entity.HasOne(st => st.Trips)
                    .WithMany(t => t.StopTimes)
                    .HasForeignKey(st => st.trip_id);
              entity.HasOne(st => st.Stops)
                    .WithMany(s => s.StopTimes)
                    .HasForeignKey(st => st.stop_id);
          });

          // Stops
          modelBuilder.Entity<MpkDataModels.Stops>(entity =>
          {
              entity.HasKey(s => s.stop_id);
              entity.Property(s => s.stop_id).HasColumnType("integer");
              entity.Property(s => s.stop_code).HasColumnType("varchar(32)");
              entity.Property(s => s.stop_name).HasColumnType("varchar(64)");
              entity.Property(s => s.stop_lat).HasColumnType("varchar(64)");
              entity.Property(s => s.stop_lon).HasColumnType("varchar(64)");
          });

          // Trips
          modelBuilder.Entity<MpkDataModels.Trips>(entity =>
          {
                entity.HasKey(t => t.trip_id);
                entity.Property(t => t.route_id).HasColumnType("varchar(5)");
                entity.Property(t => t.service_id).HasColumnType("integer");
                entity.Property(t => t.trip_id).HasColumnType("varchar(16)");
                entity.Property(t => t.trip_headsign).HasColumnType("varchar(64)");
                entity.Property(t => t.direction_id).HasColumnType("integer");
                entity.Property(t => t.shape_id).HasColumnType("integer");
                entity.Property(t => t.brigade_id).HasColumnType("integer");
                entity.Property(t => t.vehicle_id).HasColumnType("integer");
                entity.Property(t => t.variant_id).HasColumnType("integer");

                entity.HasOne(t => t.Routes)
                      .WithMany(r => r.Trips)
                      .HasForeignKey(t => t.route_id);
                entity.HasOne(t => t.Calendar)
                      .WithMany(c => c.Trips)
                      .HasForeignKey(t => t.service_id);
                entity.HasOne(t => t.Shape) // Updated to reference Shape
                      .WithMany(s => s.Trips)
                      .HasForeignKey(t => t.shape_id);
                entity.HasOne(t => t.Variants)
                      .WithMany(v => v.Trips)
                      .HasForeignKey(t => t.variant_id);
                entity.HasMany(t => t.StopTimes)
                      .WithOne(st => st.Trips)
                      .HasForeignKey(st => st.trip_id);
          });

          // Variants
          modelBuilder.Entity<MpkDataModels.Variants>(entity =>
          {
              entity.HasKey(v => v.variant_id);
              entity.Property(v => v.variant_id).HasColumnType("integer");
              entity.Property(v => v.is_main).HasColumnType("boolean");
              entity.Property(v => v.equiv_main_variant_id).HasColumnType("varchar(16)");
              entity.Property(v => v.join_stop_id).HasColumnType("varchar(16)");
              entity.Property(v => v.disjoin_stop_id).HasColumnType("varchar(16)");
          });

          // Vehicle_Types
          modelBuilder.Entity<MpkDataModels.Vehicle_Types>(entity =>
          {
              entity.HasKey(v => v.vehicle_type_id);
              entity.Property(v => v.vehicle_type_id).HasColumnType("integer");
              entity.Property(v => v.vehicle_type_name).HasColumnType("varchar(64)");
              entity.Property(v => v.vehicle_type_description).HasColumnType("varchar(64)");
              entity.Property(v => v.vehicle_type_symbol).HasColumnType("varchar(64)");
          });
      }
}