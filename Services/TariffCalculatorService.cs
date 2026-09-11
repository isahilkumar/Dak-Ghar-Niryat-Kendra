using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Services
{
    public class TariffCalculatorService : ITariffCalculatorService
    {
        private readonly ApplicationDbContext _context;

        public TariffCalculatorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public decimal CalculateVolumetricWeight(decimal lengthCm, decimal widthCm, decimal heightCm)
        {
            // Standard IATA/UPU postal volumetric formula: (L x W x H in cm) / 5000
            if (lengthCm <= 0 || widthCm <= 0 || heightCm <= 0)
                return 0;

            decimal volumetricKg = (lengthCm * widthCm * heightCm) / 5000m;
            return Math.Round(volumetricKg, 2);
        }

        public async Task<List<ServiceRateComparisonResult>> CalculateRatesAsync(
            string destinationCountryCode,
            decimal actualWeightKg,
            decimal lengthCm,
            decimal widthCm,
            decimal heightCm,
            decimal declaredValueINR,
            bool includeInsurance = true)
        {
            decimal volumetricWeight = CalculateVolumetricWeight(lengthCm, widthCm, heightCm);
            decimal chargeableWeight = Math.Max(actualWeightKg, volumetricWeight);
            int weightGrams = (int)Math.Ceiling(chargeableWeight * 1000m);

            var availableTariffs = await _context.TariffZones
                .Where(t => t.CountryCode.ToUpper() == destinationCountryCode.ToUpper())
                .ToListAsync();

            var results = new List<ServiceRateComparisonResult>();

            // All possible services
            var allServices = new[]
            {
                (ServiceType.EMS_SpeedPost, "International EMS (Speed Post)", "Fastest Delivery", "Premium express service with end-to-end priority handling & customs express corridor. Weight up to 30 Kg."),
                (ServiceType.InternationalAirParcel, "International Air Parcel", "Heavy Cargo / Economical", "Standard air conveyance for parcels up to 20 Kg. Best for bulk MSME shipments."),
                (ServiceType.ITPS_TrackedPacket, "International Tracked Packet Service (ITPS)", "Best for E-Commerce", "Designed specifically for cross-border e-commerce sellers with affordable tracking. Max weight 2.0 Kg."),
                (ServiceType.RegisteredAirMail, "International Registered Air Mail", "Documents / Small Articles", "Economical registered mail service for small packets & lightweight products up to 2.0 Kg.")
            };

            foreach (var (service, name, badge, desc) in allServices)
            {
                var tariff = availableTariffs.FirstOrDefault(t => t.ServiceType == service);
                var result = new ServiceRateComparisonResult
                {
                    ServiceType = service,
                    ServiceName = name,
                    ServiceBadge = badge,
                    Description = desc,
                    ChargeableWeightKg = chargeableWeight
                };

                if (tariff == null)
                {
                    result.IsEligible = false;
                    result.IneligibilityReason = "Service not operating to this destination country.";
                    results.Add(result);
                    continue;
                }

                if (chargeableWeight > tariff.MaxAllowedWeightKg)
                {
                    result.IsEligible = false;
                    result.IneligibilityReason = $"Weight exceeds maximum limit of {tariff.MaxAllowedWeightKg} Kg for this service.";
                    results.Add(result);
                    continue;
                }

                // Compute base postage
                decimal basePostage = tariff.BaseRateINR;
                if (weightGrams > tariff.BaseWeightGrams)
                {
                    int extraGrams = weightGrams - tariff.BaseWeightGrams;
                    int extraSteps = (int)Math.Ceiling((double)extraGrams / tariff.StepWeightGrams);
                    basePostage += extraSteps * tariff.StepRateINR;
                }

                decimal fuelSurcharge = Math.Round(basePostage * (tariff.FuelSurchargePercent / 100m), 2);
                decimal insurance = 0.0m;
                if (includeInsurance && declaredValueINR > 0)
                {
                    // 1% of declared value or min Rs 50
                    insurance = Math.Max(50.0m, Math.Round(declaredValueINR * 0.01m, 2));
                }

                decimal subTotal = basePostage + fuelSurcharge + insurance;
                decimal gst = Math.Round(subTotal * (tariff.GstPercent / 100m), 2);
                decimal total = subTotal + gst;

                result.BasePostageINR = basePostage;
                result.FuelSurchargeINR = fuelSurcharge;
                result.InsuranceINR = insurance;
                result.GstINR = gst;
                result.TotalCostINR = total;
                result.EstimatedDaysMin = tariff.MinDeliveryDays;
                result.EstimatedDaysMax = tariff.MaxDeliveryDays;
                result.IsEligible = true;

                results.Add(result);
            }

            return results;
        }

        public async Task<(decimal postage, decimal fuelSurcharge, decimal insurance, decimal gst, decimal total, decimal chargeableWeight)> CalculateSpecificServiceRateAsync(
            ServiceType serviceType,
            string destinationCountryCode,
            decimal actualWeightKg,
            decimal lengthCm,
            decimal widthCm,
            decimal heightCm,
            decimal declaredValueINR)
        {
            var rates = await CalculateRatesAsync(destinationCountryCode, actualWeightKg, lengthCm, widthCm, heightCm, declaredValueINR, true);
            var selected = rates.FirstOrDefault(r => r.ServiceType == serviceType && r.IsEligible);

            if (selected != null)
            {
                return (selected.BasePostageINR, selected.FuelSurchargeINR, selected.InsuranceINR, selected.GstINR, selected.TotalCostINR, selected.ChargeableWeightKg);
            }

            // Fallback default calculation if tariff zone not found
            decimal volumetric = CalculateVolumetricWeight(lengthCm, widthCm, heightCm);
            decimal chargeable = Math.Max(actualWeightKg, volumetric);
            decimal baseRate = 1200m * Math.Max(1m, chargeable);
            decimal fuel = baseRate * 0.10m;
            decimal ins = declaredValueINR > 0 ? Math.Max(50m, declaredValueINR * 0.01m) : 0m;
            decimal gstAmt = (baseRate + fuel + ins) * 0.18m;
            decimal totalCost = baseRate + fuel + ins + gstAmt;

            return (baseRate, fuel, ins, gstAmt, totalCost, chargeable);
        }
    }
}
