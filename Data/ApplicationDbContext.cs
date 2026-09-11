using Microsoft.EntityFrameworkCore;
using DakGharNiryatKendra.Models;

namespace DakGharNiryatKendra.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Schema: Identity
        public DbSet<User> Users => Set<User>();

        // Schema: Master
        public DbSet<DnkCenter> DnkCenters => Set<DnkCenter>();
        public DbSet<HsCodeDirectory> HsCodeDirectories => Set<HsCodeDirectory>();
        public DbSet<TariffZone> TariffZones => Set<TariffZone>();

        // Schema: Export
        public DbSet<ExportConsignment> ExportConsignments => Set<ExportConsignment>();
        public DbSet<ConsignmentItem> ConsignmentItems => Set<ConsignmentItem>();
        public DbSet<PickupRequest> PickupRequests => Set<PickupRequest>();

        // Schema: Postal & Manifest
        public DbSet<DispatchManifest> DispatchManifests => Set<DispatchManifest>();

        // Schema: Tracking
        public DbSet<TrackingMilestone> TrackingMilestones => Set<TrackingMilestone>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Table & Schema Mappings (Supports both SQL Server Schemas and standard naming)
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<DnkCenter>().ToTable("DnkCenters");
            modelBuilder.Entity<HsCodeDirectory>().ToTable("HsCodeDirectories");
            modelBuilder.Entity<TariffZone>().ToTable("TariffZones");
            modelBuilder.Entity<ExportConsignment>().ToTable("ExportConsignments");
            modelBuilder.Entity<ConsignmentItem>().ToTable("ConsignmentItems");
            modelBuilder.Entity<PickupRequest>().ToTable("PickupRequests");
            modelBuilder.Entity<DispatchManifest>().ToTable("DispatchManifests");
            modelBuilder.Entity<TrackingMilestone>().ToTable("TrackingMilestones");

            // 2. High-Performance Enterprise Indexes
            modelBuilder.Entity<ExportConsignment>(entity =>
            {
                // Unique S10 Barcode Index
                entity.HasIndex(c => c.TrackingNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_ExportConsignments_TrackingNumber");

                // PBE Registry Index
                entity.HasIndex(c => c.PbeNumber)
                    .HasDatabaseName("IX_ExportConsignments_PbeNumber");

                // Composite Filter Index for Exporter Dashboard & Querying
                entity.HasIndex(c => new { c.ExporterId, c.Status, c.CreatedAt })
                    .HasDatabaseName("IX_ExportConsignments_Exporter_Status_CreatedAt");

                // Customs Scrutiny Index
                entity.HasIndex(c => new { c.CustomsStatus, c.DnkCenterId })
                    .HasDatabaseName("IX_ExportConsignments_CustomsStatus_DnkCenter");

                // Destination Analytics Index
                entity.HasIndex(c => new { c.DestinationCountryCode, c.Status })
                    .HasDatabaseName("IX_ExportConsignments_Country_Status");
            });

            modelBuilder.Entity<TrackingMilestone>(entity =>
            {
                // Fast reverse-chronological milestone lookup
                entity.HasIndex(m => new { m.TrackingNumber, m.EventTime })
                    .HasDatabaseName("IX_TrackingMilestones_TrackingNumber_EventTime");

                entity.HasIndex(m => m.ExportConsignmentId)
                    .HasDatabaseName("IX_TrackingMilestones_ConsignmentId");
            });

            modelBuilder.Entity<DnkCenter>(entity =>
            {
                entity.HasIndex(d => d.CenterCode)
                    .IsUnique()
                    .HasDatabaseName("UQ_DnkCenters_CenterCode");

                entity.HasIndex(d => new { d.Pincode, d.State })
                    .HasDatabaseName("IX_DnkCenters_Pincode_State");
            });

            modelBuilder.Entity<HsCodeDirectory>(entity =>
            {
                entity.HasIndex(h => h.HsCode)
                    .HasDatabaseName("IX_HsCodeDirectories_HsCode");

                entity.HasIndex(h => h.Category)
                    .HasDatabaseName("IX_HsCodeDirectories_Category");
            });

            modelBuilder.Entity<TariffZone>(entity =>
            {
                entity.HasIndex(t => new { t.CountryCode, t.ServiceType })
                    .HasDatabaseName("IX_TariffZones_Country_Service");
            });

            // 3. Strict Decimal Precision for Financial & Customs Calculations
            modelBuilder.Entity<ExportConsignment>(entity =>
            {
                entity.Property(e => e.ActualWeightKg).HasPrecision(10, 3);
                entity.Property(e => e.LengthCm).HasPrecision(8, 2);
                entity.Property(e => e.WidthCm).HasPrecision(8, 2);
                entity.Property(e => e.HeightCm).HasPrecision(8, 2);
                entity.Property(e => e.VolumetricWeightKg).HasPrecision(10, 3);
                entity.Property(e => e.ChargeableWeightKg).HasPrecision(10, 3);
                entity.Property(e => e.VerifiedWeightKg).HasPrecision(10, 3);

                entity.Property(e => e.DeclaredValueForeign).HasPrecision(18, 2);
                entity.Property(e => e.ExchangeRateToINR).HasPrecision(10, 4);
                entity.Property(e => e.DeclaredValueINR).HasPrecision(18, 2);

                entity.Property(e => e.PostageAmountINR).HasPrecision(18, 2);
                entity.Property(e => e.FuelSurchargeINR).HasPrecision(18, 2);
                entity.Property(e => e.InsuranceAmountINR).HasPrecision(18, 2);
                entity.Property(e => e.GstAmountINR).HasPrecision(18, 2);
                entity.Property(e => e.TotalShippingCostINR).HasPrecision(18, 2);
                entity.Property(e => e.CustomsAssessedDutyINR).HasPrecision(18, 2);
            });

            modelBuilder.Entity<ConsignmentItem>(entity =>
            {
                entity.Property(e => e.UnitPriceINR).HasPrecision(18, 2);
                entity.Property(e => e.TotalPriceINR).HasPrecision(18, 2);
                entity.Property(e => e.NetWeightKg).HasPrecision(10, 3);
            });

            modelBuilder.Entity<TariffZone>(entity =>
            {
                entity.Property(e => e.BaseRateINR).HasPrecision(18, 2);
                entity.Property(e => e.StepRateINR).HasPrecision(18, 2);
                entity.Property(e => e.FuelSurchargePercent).HasPrecision(5, 2);
                entity.Property(e => e.GstPercent).HasPrecision(5, 2);
                entity.Property(e => e.MaxAllowedWeightKg).HasPrecision(8, 2);
            });

            modelBuilder.Entity<HsCodeDirectory>(entity =>
            {
                entity.Property(e => e.StandardGstPercent).HasPrecision(5, 2);
                entity.Property(e => e.ExportDutyPercent).HasPrecision(5, 2);
                entity.Property(e => e.RodtepRatePercent).HasPrecision(5, 2);
            });

            modelBuilder.Entity<DispatchManifest>(entity =>
            {
                entity.Property(e => e.TotalWeightKg).HasPrecision(10, 3);
            });

            // 4. Relational Constraints & Referential Integrity
            modelBuilder.Entity<ExportConsignment>()
                .HasOne(c => c.Exporter)
                .WithMany()
                .HasForeignKey(c => c.ExporterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExportConsignment>()
                .HasOne(c => c.DnkCenter)
                .WithMany()
                .HasForeignKey(c => c.DnkCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExportConsignment>()
                .HasMany(c => c.Items)
                .WithOne(i => i.ExportConsignment)
                .HasForeignKey(i => i.ExportConsignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExportConsignment>()
                .HasMany(c => c.Milestones)
                .WithOne(m => m.ExportConsignment)
                .HasForeignKey(m => m.ExportConsignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PickupRequest>()
                .HasOne(p => p.ExportConsignment)
                .WithMany()
                .HasForeignKey(p => p.ExportConsignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DispatchManifest>()
                .HasOne(m => m.OriginDnkCenter)
                .WithMany()
                .HasForeignKey(m => m.OriginDnkCenterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
