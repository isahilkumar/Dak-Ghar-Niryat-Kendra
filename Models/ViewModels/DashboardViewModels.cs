namespace DakGharNiryatKendra.Models.ViewModels
{
    public class ExporterDashboardViewModel
    {
        public User Exporter { get; set; } = null!;
        public int TotalShipments { get; set; }
        public int ActiveShipments { get; set; }
        public int InTransitShipments { get; set; }
        public int CustomsClearedShipments { get; set; }
        public int DeliveredShipments { get; set; }
        public decimal TotalExportValueINR { get; set; }
        public decimal TotalExportValueUSD { get; set; }
        public List<ExportConsignment> RecentConsignments { get; set; } = new List<ExportConsignment>();
        public List<PickupRequest> PendingPickups { get; set; } = new List<PickupRequest>();
    }

    public class PostalStaffDashboardViewModel
    {
        public User StaffUser { get; set; } = null!;
        public DnkCenter DnkCenter { get; set; } = null!;
        public int TodayInductions { get; set; }
        public int PendingInductions { get; set; }
        public int ReadyForDispatchCount { get; set; }
        public int ActivePickupsCount { get; set; }
        public List<ExportConsignment> PendingInductionList { get; set; } = new List<ExportConsignment>();
        public List<ExportConsignment> InductedTodayList { get; set; } = new List<ExportConsignment>();
        public List<DispatchManifest> RecentManifests { get; set; } = new List<DispatchManifest>();
        public List<PickupRequest> TodayPickups { get; set; } = new List<PickupRequest>();
    }

    public class CustomsDashboardViewModel
    {
        public User Officer { get; set; } = null!;
        public int TotalUnderScrutiny { get; set; }
        public int ClearedToday { get; set; }
        public int UnderHoldQuery { get; set; }
        public decimal TotalDutyAssessedINR { get; set; }
        public List<ExportConsignment> PendingScrutinyList { get; set; } = new List<ExportConsignment>();
        public List<ExportConsignment> HeldConsignments { get; set; } = new List<ExportConsignment>();
        public List<ExportConsignment> RecentlyCleared { get; set; } = new List<ExportConsignment>();
    }

    public class AdminDashboardViewModel
    {
        public int TotalExporters { get; set; }
        public int TotalDnkCenters { get; set; }
        public int TotalConsignmentsAllTime { get; set; }
        public decimal TotalExportValueAllTimeINR { get; set; }
        public decimal TotalPostageRevenueINR { get; set; }
        public List<DnkCenter> DnkCenters { get; set; } = new List<DnkCenter>();
        public List<ExportConsignment> AllRecentConsignments { get; set; } = new List<ExportConsignment>();
        public Dictionary<string, int> ExportsByCountry { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> ExportsByCategory { get; set; } = new Dictionary<string, decimal>();
    }
}
