namespace DakGharNiryatKendra.Models.ViewModels
{
    public class TrackingViewModel
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public ExportConsignment? Consignment { get; set; }
        public bool IsFound { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public List<TrackingMilestone> Milestones { get; set; } = new List<TrackingMilestone>();
    }
}
