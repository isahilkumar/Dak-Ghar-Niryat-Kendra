using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;

namespace DakGharNiryatKendra.Services
{
    public interface ITariffCalculatorService
    {
        Task<List<ServiceRateComparisonResult>> CalculateRatesAsync(
            string destinationCountryCode,
            decimal actualWeightKg,
            decimal lengthCm,
            decimal widthCm,
            decimal heightCm,
            decimal declaredValueINR,
            bool includeInsurance = true);

        Task<(decimal postage, decimal fuelSurcharge, decimal insurance, decimal gst, decimal total, decimal chargeableWeight)> CalculateSpecificServiceRateAsync(
            ServiceType serviceType,
            string destinationCountryCode,
            decimal actualWeightKg,
            decimal lengthCm,
            decimal widthCm,
            decimal heightCm,
            decimal declaredValueINR);

        decimal CalculateVolumetricWeight(decimal lengthCm, decimal widthCm, decimal heightCm);
    }
}
