using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;

namespace DakGharNiryatKendra.Services
{
    public interface ITrackingService
    {
        Task<TrackingViewModel> TrackAsync(string trackingNumber);
        Task<bool> AddMilestoneAsync(string trackingNumber, string facilityLocation, string headline, ConsignmentStatus status, string description, string? performedBy = null);
        Task<(bool Success, string Message, ConsignmentStatus? NewStatus)> SimulateNextStepAsync(string trackingNumber);
    }
}
