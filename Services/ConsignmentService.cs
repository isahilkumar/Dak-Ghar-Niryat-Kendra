using DakGharNiryatKendra.Data;
using DakGharNiryatKendra.Models;
using DakGharNiryatKendra.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Services
{
    public class ConsignmentService : IConsignmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITariffCalculatorService _tariffService;
        private readonly IQrBarcodeService _qrBarcodeService;

        public ConsignmentService(
            ApplicationDbContext context,
            ITariffCalculatorService tariffService,
            IQrBarcodeService qrBarcodeService)
        {
            _context = context;
            _tariffService = tariffService;
            _qrBarcodeService = qrBarcodeService;
        }

        public async Task<ExportConsignment> BookConsignmentAsync(BookConsignmentViewModel model, int exporterId)
        {
            var exporter = await _context.Users.FindAsync(exporterId)
                ?? throw new InvalidOperationException("Exporter account not found.");

            var dnkCenter = await _context.DnkCenters.FindAsync(model.DnkCenterId)
                ?? throw new InvalidOperationException("Induction DNK Kendra not found.");

            // Generate tracking & PBE numbers
            string trackingNumber = _qrBarcodeService.GenerateS10TrackingNumber(model.ServiceType);
            string pbeNumber = _qrBarcodeService.GeneratePbeNumber(dnkCenter.CenterCode, model.PbeType);

            // Compute volumetric weight & chargeable weight
            decimal volumetricWeight = _tariffService.CalculateVolumetricWeight(model.LengthCm, model.WidthCm, model.HeightCm);
            decimal chargeableWeight = Math.Max(model.ActualWeightKg, volumetricWeight);

            // Calculate exact shipping cost
            var (postage, fuel, insurance, gst, totalCost, _) = await _tariffService.CalculateSpecificServiceRateAsync(
                model.ServiceType,
                model.DestinationCountryCode,
                model.ActualWeightKg,
                model.LengthCm,
                model.WidthCm,
                model.HeightCm,
                model.DeclaredValueINR);

            var consignment = new ExportConsignment
            {
                TrackingNumber = trackingNumber,
                PbeNumber = pbeNumber,
                PbeType = model.PbeType,
                ServiceType = model.ServiceType,
                Status = model.RequestPickup ? ConsignmentStatus.PickupScheduled : ConsignmentStatus.Booked,
                CustomsStatus = CustomsStatus.PendingVerification,
                ExporterId = exporterId,
                DnkCenterId = model.DnkCenterId,
                RecipientName = model.RecipientName,
                RecipientCompany = model.RecipientCompany,
                RecipientEmail = model.RecipientEmail,
                RecipientPhone = model.RecipientPhone,
                RecipientAddress = model.RecipientAddress,
                RecipientCity = model.RecipientCity,
                RecipientState = model.RecipientState,
                RecipientPostalCode = model.RecipientPostalCode,
                RecipientCountry = model.RecipientCountry,
                DestinationCountryCode = model.DestinationCountryCode.ToUpperInvariant(),
                ActualWeightKg = model.ActualWeightKg,
                LengthCm = model.LengthCm,
                WidthCm = model.WidthCm,
                HeightCm = model.HeightCm,
                VolumetricWeightKg = volumetricWeight,
                ChargeableWeightKg = chargeableWeight,
                InvoiceNumber = model.InvoiceNumber ?? $"INV-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(100, 999)}",
                InvoiceDate = model.InvoiceDate ?? DateTime.UtcNow,
                LutNumber = !string.IsNullOrWhiteSpace(model.LutNumber) ? model.LutNumber : exporter.LutNumber,
                NatureOfExport = model.NatureOfExport,
                CurrencyCode = model.CurrencyCode,
                DeclaredValueForeign = model.DeclaredValueForeign,
                ExchangeRateToINR = model.ExchangeRateToINR,
                DeclaredValueINR = model.DeclaredValueINR,
                PostageAmountINR = postage,
                FuelSurchargeINR = fuel,
                InsuranceAmountINR = insurance,
                GstAmountINR = gst,
                TotalShippingCostINR = totalCost,
                IsPickupRequested = model.RequestPickup,
                PickupStatus = model.RequestPickup ? PickupStatus.Requested : PickupStatus.NotRequested,
                ScheduledPickupDate = model.RequestPickup ? model.ScheduledPickupDate : null,
                PickupTimeSlot = model.RequestPickup ? model.PickupTimeSlot : null,
                CreatedAt = DateTime.UtcNow
            };

            // Add line items
            if (model.Items != null && model.Items.Count > 0)
            {
                foreach (var item in model.Items)
                {
                    consignment.Items.Add(new ConsignmentItem
                    {
                        ItemDescription = item.ItemDescription,
                        HsCode = item.HsCode,
                        Quantity = item.Quantity,
                        UnitOfMeasurement = item.UnitOfMeasurement,
                        UnitPriceINR = item.UnitPriceINR,
                        TotalPriceINR = item.TotalPriceINR > 0 ? item.TotalPriceINR : (item.UnitPriceINR * item.Quantity),
                        NetWeightKg = item.NetWeightKg,
                        CountryOfOrigin = item.CountryOfOrigin ?? "India"
                    });
                }
            }
            else
            {
                // Default item if none passed
                consignment.Items.Add(new ConsignmentItem
                {
                    ItemDescription = "General Export Merchandise",
                    HsCode = "6204.42",
                    Quantity = 1,
                    UnitOfMeasurement = "PCS",
                    UnitPriceINR = model.DeclaredValueINR,
                    TotalPriceINR = model.DeclaredValueINR,
                    NetWeightKg = model.ActualWeightKg,
                    CountryOfOrigin = "India"
                });
            }

            // Initial tracking milestone
            consignment.Milestones.Add(new TrackingMilestone
            {
                TrackingNumber = trackingNumber,
                EventTime = DateTime.UtcNow,
                FacilityLocation = $"{dnkCenter.CenterName} Digital Portal",
                StatusHeadline = "Postal Bill of Export Filed & Article Booked",
                StatusCategory = model.RequestPickup ? ConsignmentStatus.PickupScheduled : ConsignmentStatus.Booked,
                DetailedDescription = $"Article booked under {model.ServiceType} to {model.RecipientCountry}. PBE reference: {pbeNumber}.",
                PerformedBy = exporter.FullName
            });

            _context.ExportConsignments.Add(consignment);
            await _context.SaveChangesAsync();

            // Handle Doorstep Pickup Request
            if (model.RequestPickup)
            {
                var pickup = new PickupRequest
                {
                    ExportConsignmentId = consignment.Id,
                    ExporterId = exporterId,
                    ScheduledDate = model.ScheduledPickupDate ?? DateTime.UtcNow.Date.AddDays(1),
                    TimeSlot = model.PickupTimeSlot,
                    PickupAddress = $"{exporter.Address}, {exporter.City}, {exporter.State} - {exporter.Pincode}",
                    Pincode = exporter.Pincode ?? dnkCenter.Pincode,
                    Status = PickupStatus.Requested,
                    SpecialInstructions = model.PickupSpecialInstructions,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PickupRequests.Add(pickup);
                await _context.SaveChangesAsync();
            }

            return consignment;
        }

        public async Task<ExportConsignment?> GetConsignmentByIdAsync(int id)
        {
            return await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Include(c => c.DnkCenter)
                .Include(c => c.Items)
                .Include(c => c.Milestones.OrderBy(m => m.EventTime))
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ExportConsignment?> GetConsignmentByTrackingNumberAsync(string trackingNumber)
        {
            return await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Include(c => c.DnkCenter)
                .Include(c => c.Items)
                .Include(c => c.Milestones.OrderBy(m => m.EventTime))
                .FirstOrDefaultAsync(c => c.TrackingNumber.ToUpper() == trackingNumber.Trim().ToUpper());
        }

        public async Task<List<ExportConsignment>> GetExporterConsignmentsAsync(int exporterId)
        {
            return await _context.ExportConsignments
                .Include(c => c.DnkCenter)
                .Include(c => c.Items)
                .Where(c => c.ExporterId == exporterId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ExportConsignment>> GetAllConsignmentsAsync()
        {
            return await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Include(c => c.DnkCenter)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ExportConsignment>> GetPendingInductionsAsync(int? dnkCenterId = null)
        {
            var query = _context.ExportConsignments
                .Include(c => c.Exporter)
                .Include(c => c.DnkCenter)
                .Where(c => c.Status == ConsignmentStatus.Booked || c.Status == ConsignmentStatus.PickupScheduled || c.Status == ConsignmentStatus.PickedUp);

            if (dnkCenterId.HasValue)
            {
                query = query.Where(c => c.DnkCenterId == dnkCenterId.Value);
            }

            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<List<ExportConsignment>> GetPendingCustomsScrutinyAsync()
        {
            return await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Include(c => c.DnkCenter)
                .Include(c => c.Items)
                .Where(c => c.Status == ConsignmentStatus.InductedAtDNK || c.Status == ConsignmentStatus.ReceivedAtFPO || c.Status == ConsignmentStatus.CustomsHold || c.CustomsStatus != CustomsStatus.Approved)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, ExportConsignment? Consignment)> InductConsignmentAsync(string trackingNumber, decimal verifiedWeightKg, string staffName)
        {
            var consignment = await _context.ExportConsignments
                .Include(c => c.DnkCenter)
                .FirstOrDefaultAsync(c => c.TrackingNumber.ToUpper() == trackingNumber.Trim().ToUpper());

            if (consignment == null)
            {
                return (false, $"Consignment with Tracking Number '{trackingNumber}' not found in the DNK network.", null);
            }

            consignment.Status = ConsignmentStatus.InductedAtDNK;
            consignment.InductedAt = DateTime.UtcNow;
            consignment.InductedByStaffName = staffName;
            consignment.VerifiedWeightKg = verifiedWeightKg;

            var milestone = new TrackingMilestone
            {
                ExportConsignmentId = consignment.Id,
                TrackingNumber = consignment.TrackingNumber,
                EventTime = DateTime.UtcNow,
                FacilityLocation = consignment.DnkCenter != null ? consignment.DnkCenter.CenterName : "Dak Ghar Niryat Kendra Counter",
                StatusHeadline = "Article Inducted at DNK Counter",
                StatusCategory = ConsignmentStatus.InductedAtDNK,
                DetailedDescription = $"Article physically received and verified. Verified Gross Weight: {verifiedWeightKg:F2} Kg. Dispatched to Foreign Post Office.",
                PerformedBy = staffName
            };

            _context.TrackingMilestones.Add(milestone);
            await _context.SaveChangesAsync();

            return (true, $"Article {consignment.TrackingNumber} successfully inducted and verified at {verifiedWeightKg:F2} Kg.", consignment);
        }

        public async Task<(bool Success, string Message)> UpdateCustomsStatusAsync(
            string trackingNumber,
            CustomsStatus status,
            string officerName,
            string remarks,
            decimal dutyAmount = 0)
        {
            var consignment = await _context.ExportConsignments
                .Include(c => c.DnkCenter)
                .FirstOrDefaultAsync(c => c.TrackingNumber.ToUpper() == trackingNumber.Trim().ToUpper());

            if (consignment == null)
            {
                return (false, $"Consignment with Tracking Number '{trackingNumber}' not found.");
            }

            consignment.CustomsStatus = status;
            consignment.ScrutinyOfficerName = officerName;
            consignment.CustomsOfficerRemarks = remarks;
            consignment.CustomsAssessedDutyINR = dutyAmount;

            string headline;
            string details;
            ConsignmentStatus category;

            switch (status)
            {
                case CustomsStatus.Approved:
                    consignment.Status = ConsignmentStatus.CustomsCleared;
                    consignment.CustomsClearedAt = DateTime.UtcNow;
                    headline = "Customs Out of Charge (OOC) Granted";
                    category = ConsignmentStatus.CustomsCleared;
                    details = $"Electronic PBE verified and cleared by Customs Officer {officerName}. Consignment released for international air dispatch. Remarks: {remarks}";
                    break;

                case CustomsStatus.HoldForQuery:
                    consignment.Status = ConsignmentStatus.CustomsHold;
                    headline = "Customs Scrutiny Query Raised";
                    category = ConsignmentStatus.CustomsHold;
                    details = $"Customs query issued by {officerName}: {remarks}. Exporter is advised to upload requested compliance document.";
                    break;

                case CustomsStatus.DutyAssessed:
                    headline = "Customs Export Duty Assessed";
                    category = ConsignmentStatus.CustomsHold;
                    details = $"Customs duty of INR {dutyAmount:N2} assessed by {officerName}. Remarks: {remarks}";
                    break;

                case CustomsStatus.PhysicalInspectionRequired:
                    consignment.Status = ConsignmentStatus.CustomsHold;
                    headline = "Scheduled for Non-Intrusive / Physical Inspection";
                    category = ConsignmentStatus.CustomsHold;
                    details = $"Consignment selected for physical examination at FPO inspection bay. Remarks: {remarks}";
                    break;

                default:
                    headline = "Customs Status Updated";
                    category = ConsignmentStatus.ReceivedAtFPO;
                    details = $"Customs status set to {status}. Remarks: {remarks}";
                    break;
            }

            var milestone = new TrackingMilestone
            {
                ExportConsignmentId = consignment.Id,
                TrackingNumber = consignment.TrackingNumber,
                EventTime = DateTime.UtcNow,
                FacilityLocation = "Delhi Foreign Post Office (FPO) Customs Wing",
                StatusHeadline = headline,
                StatusCategory = category,
                DetailedDescription = details,
                PerformedBy = officerName
            };

            _context.TrackingMilestones.Add(milestone);
            await _context.SaveChangesAsync();

            return (true, $"Customs status for {consignment.TrackingNumber} updated to {status}.");
        }

        public async Task<DispatchManifest> CreateDispatchManifestAsync(int originCenterId, string destinationFpo, List<int> consignmentIds, string staffName)
        {
            var center = await _context.DnkCenters.FindAsync(originCenterId)
                ?? throw new InvalidOperationException("Origin Kendra not found.");

            string centerSlug = center.CenterCode.Replace("DNK-", "");
            string manifestNum = $"MNF/{centerSlug}/{DateTime.UtcNow:yyyyMMdd}/{new Random().Next(10, 99)}";
            string bagBarcode = $"BAG-IND-{centerSlug}-{new Random().Next(100000, 999999)}";
            string sealNumber = $"SL-{centerSlug}-{new Random().Next(100000, 999999)}";

            var consignments = await _context.ExportConsignments
                .Where(c => consignmentIds.Contains(c.Id))
                .ToListAsync();

            decimal totalWeight = consignments.Sum(c => c.VerifiedWeightKg ?? c.ActualWeightKg);

            var manifest = new DispatchManifest
            {
                ManifestNumber = manifestNum,
                OriginDnkCenterId = originCenterId,
                DestinationFpo = destinationFpo,
                DispatchDate = DateTime.UtcNow,
                BagBarcodeNumber = bagBarcode,
                SealNumber = sealNumber,
                TotalArticles = consignments.Count,
                TotalWeightKg = totalWeight,
                CreatedByStaff = staffName,
                Status = "Dispatched"
            };

            _context.DispatchManifests.Add(manifest);
            await _context.SaveChangesAsync();

            // Link consignments and add milestones
            foreach (var c in consignments)
            {
                c.DispatchManifestId = manifest.Id;
                c.Status = ConsignmentStatus.BaggedAndManifested;

                _context.TrackingMilestones.Add(new TrackingMilestone
                {
                    ExportConsignmentId = c.Id,
                    TrackingNumber = c.TrackingNumber,
                    EventTime = DateTime.UtcNow,
                    FacilityLocation = center.CenterName,
                    StatusHeadline = "Bagged & Dispatched in Secured Mail Conveyance",
                    StatusCategory = ConsignmentStatus.BaggedAndManifested,
                    DetailedDescription = $"Consignment sealed in Bag {bagBarcode} (Seal: {sealNumber}) on Manifest {manifestNum} to {destinationFpo}.",
                    PerformedBy = staffName
                });
            }

            await _context.SaveChangesAsync();
            return manifest;
        }

        public async Task<(bool Success, string Message)> SchedulePickupAsync(int consignmentId, int exporterId, DateTime scheduledDate, TimeSlot timeSlot, string? instructions)
        {
            var consignment = await _context.ExportConsignments
                .Include(c => c.Exporter)
                .Include(c => c.DnkCenter)
                .FirstOrDefaultAsync(c => c.Id == consignmentId && c.ExporterId == exporterId);

            if (consignment == null)
            {
                return (false, "Consignment not found or unauthorized.");
            }

            consignment.IsPickupRequested = true;
            consignment.PickupStatus = PickupStatus.Requested;
            consignment.ScheduledPickupDate = scheduledDate;
            consignment.PickupTimeSlot = timeSlot;
            consignment.Status = ConsignmentStatus.PickupScheduled;

            var pickup = new PickupRequest
            {
                ExportConsignmentId = consignment.Id,
                ExporterId = exporterId,
                ScheduledDate = scheduledDate,
                TimeSlot = timeSlot,
                PickupAddress = $"{consignment.Exporter!.Address}, {consignment.Exporter.City} - {consignment.Exporter.Pincode}",
                Pincode = consignment.Exporter.Pincode ?? consignment.DnkCenter?.Pincode ?? "110001",
                Status = PickupStatus.Requested,
                SpecialInstructions = instructions,
                CreatedAt = DateTime.UtcNow
            };

            _context.PickupRequests.Add(pickup);

            _context.TrackingMilestones.Add(new TrackingMilestone
            {
                ExportConsignmentId = consignment.Id,
                TrackingNumber = consignment.TrackingNumber,
                EventTime = DateTime.UtcNow,
                FacilityLocation = $"{consignment.DnkCenter?.CenterName ?? "Post Office"} Local Desk",
                StatusHeadline = "Doorstep Pickup Requested",
                StatusCategory = ConsignmentStatus.PickupScheduled,
                DetailedDescription = $"Doorstep pickup requested for {scheduledDate:dd MMM yyyy} ({timeSlot.ToString().Replace('_', ' ')}). Special instructions: {instructions ?? "None"}.",
                PerformedBy = consignment.Exporter.FullName
            });

            await _context.SaveChangesAsync();
            return (true, $"Doorstep pickup scheduled for {scheduledDate:dd MMM yyyy}. Postman will arrive at your premises.");
        }

        public async Task<List<PickupRequest>> GetPendingPickupsAsync(int? dnkCenterId = null)
        {
            return await _context.PickupRequests
                .Include(p => p.ExportConsignment)
                .Include(p => p.Exporter)
                .OrderByDescending(p => p.ScheduledDate)
                .ToListAsync();
        }

        public async Task<List<DispatchManifest>> GetRecentManifestsAsync()
        {
            return await _context.DispatchManifests
                .Include(m => m.OriginDnkCenter)
                .Include(m => m.Consignments)
                .OrderByDescending(m => m.DispatchDate)
                .ToListAsync();
        }
    }
}
