using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;

namespace DakGharNiryatKendra.Services
{
    public interface IConsignmentService
    {
        Task<ExportConsignment> BookConsignmentAsync(BookConsignmentViewModel model, int exporterId);
        Task<ExportConsignment?> GetConsignmentByIdAsync(int id);
        Task<ExportConsignment?> GetConsignmentByTrackingNumberAsync(string trackingNumber);
        Task<List<ExportConsignment>> GetExporterConsignmentsAsync(int exporterId);
        Task<List<ExportConsignment>> GetAllConsignmentsAsync();
        Task<List<ExportConsignment>> GetPendingInductionsAsync(int? dnkCenterId = null);
        Task<List<ExportConsignment>> GetPendingCustomsScrutinyAsync();
        Task<(bool Success, string Message, ExportConsignment? Consignment)> InductConsignmentAsync(string trackingNumber, decimal verifiedWeightKg, string staffName);
        Task<(bool Success, string Message)> UpdateCustomsStatusAsync(string trackingNumber, CustomsStatus status, string officerName, string remarks, decimal dutyAmount = 0);
        Task<DispatchManifest> CreateDispatchManifestAsync(int originCenterId, string destinationFpo, List<int> consignmentIds, string staffName);
        Task<(bool Success, string Message)> SchedulePickupAsync(int consignmentId, int exporterId, DateTime scheduledDate, TimeSlot timeSlot, string? instructions);
        Task<List<PickupRequest>> GetPendingPickupsAsync(int? dnkCenterId = null);
        Task<List<DispatchManifest>> GetRecentManifestsAsync();
    }
}
