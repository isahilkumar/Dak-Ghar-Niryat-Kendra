using DakGharNiryatKendra.Models;
using Microsoft.EntityFrameworkCore;

namespace DakGharNiryatKendra.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return; // DB has already been seeded
            }

            // 1. Seed DNK Centers across India
            var dnkCenters = new List<DnkCenter>
            {
                new DnkCenter
                {
                    CenterCode = "DNK-DEL-01",
                    CenterName = "New Delhi GPO Dak Ghar Niryat Kendra",
                    Address = "GPO Complex, Ashoka Road, Connaught Place",
                    City = "New Delhi",
                    State = "Delhi",
                    Pincode = "110001",
                    ContactPerson = "Rajiv Ranjan (Superintendent)",
                    Phone = "+91 11 2336 4111",
                    Email = "dnk.delhigpo@indiapost.gov.in",
                    Latitude = 28.6253,
                    Longitude = 77.2090,
                    HasCustomsDesk = true,
                    HasPackagingFacility = true,
                    OperatingHours = "08:30 AM - 07:00 PM (Mon-Sat)",
                    ServicesOffered = "International EMS, Air Parcel, ITPS, Customs Scrutiny, Packaging Support, Export Helpdesk"
                },
                new DnkCenter
                {
                    CenterCode = "DNK-RAJ-01",
                    CenterName = "Jaipur Head Post Office DNK",
                    Address = "MI Road, Near Panch Batti",
                    City = "Jaipur",
                    State = "Rajasthan",
                    Pincode = "302001",
                    ContactPerson = "Anil Singhal (Nodal Officer)",
                    Phone = "+91 141 236 8920",
                    Email = "dnk.jaipurhpo@indiapost.gov.in",
                    Latitude = 26.9196,
                    Longitude = 75.8080,
                    HasCustomsDesk = true,
                    HasPackagingFacility = true,
                    OperatingHours = "09:00 AM - 06:00 PM (Mon-Sat)",
                    ServicesOffered = "Textiles & Handicrafts Special Hub, EMS, Air Parcel, ITPS, On-site PBE Assistance"
                },
                new DnkCenter
                {
                    CenterCode = "DNK-MUM-01",
                    CenterName = "Mumbai GPO Dak Ghar Niryat Kendra",
                    Address = "GPO Building, CST Area, Fort",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Pincode = "400001",
                    ContactPerson = "Pooja Kulkarni (Asst. Director)",
                    Phone = "+91 22 2262 0524",
                    Email = "dnk.mumbaigpo@indiapost.gov.in",
                    Latitude = 18.9402,
                    Longitude = 72.8356,
                    HasCustomsDesk = true,
                    HasPackagingFacility = true,
                    OperatingHours = "08:00 AM - 08:00 PM (Mon-Sat)",
                    ServicesOffered = "Gems & Jewellery Hub, EMS, Air Parcel, ITPS, Fast-track FPO linkage"
                },
                new DnkCenter
                {
                    CenterCode = "DNK-KER-01",
                    CenterName = "Kochi Head Post Office DNK",
                    Address = "Park Avenue, Marine Drive",
                    City = "Kochi",
                    State = "Kerala",
                    Pincode = "682011",
                    ContactPerson = "K. Sreekumar (Nodal Officer)",
                    Phone = "+91 484 235 2200",
                    Email = "dnk.kochihpo@indiapost.gov.in",
                    Latitude = 9.9723,
                    Longitude = 76.2783,
                    HasCustomsDesk = true,
                    HasPackagingFacility = true,
                    OperatingHours = "09:00 AM - 05:30 PM (Mon-Sat)",
                    ServicesOffered = "Spices & Organic Exports Nodal Kendra, EMS, Air Parcel, ITPS"
                },
                new DnkCenter
                {
                    CenterCode = "DNK-BLR-01",
                    CenterName = "Bengaluru GPO Dak Ghar Niryat Kendra",
                    Address = "Raj Bhavan Road, Ambedkar Veedhi",
                    City = "Bengaluru",
                    State = "Karnataka",
                    Pincode = "560001",
                    ContactPerson = "Venkatesh Murthy (Senior Postmaster)",
                    Phone = "+91 80 2286 7890",
                    Email = "dnk.blr.gpo@indiapost.gov.in",
                    Latitude = 12.9815,
                    Longitude = 77.5921,
                    HasCustomsDesk = true,
                    HasPackagingFacility = true,
                    OperatingHours = "08:30 AM - 06:30 PM (Mon-Sat)",
                    ServicesOffered = "Silk & E-Commerce Hub, ITPS, EMS, Air Parcel"
                },
                new DnkCenter
                {
                    CenterCode = "DNK-UP-01",
                    CenterName = "Moradabad Head Post Office DNK",
                    Address = "Civil Lines, Near Company Bagh",
                    City = "Moradabad",
                    State = "Uttar Pradesh",
                    Pincode = "244001",
                    ContactPerson = "Mohd. Tariq (Export Facilitator)",
                    Phone = "+91 591 241 1234",
                    Email = "dnk.moradabad@indiapost.gov.in",
                    Latitude = 28.8386,
                    Longitude = 78.7733,
                    HasCustomsDesk = true,
                    HasPackagingFacility = true,
                    OperatingHours = "09:00 AM - 05:30 PM (Mon-Sat)",
                    ServicesOffered = "Metal Art & Brassware Direct Export Corridor, Air Parcel, EMS"
                },
                new DnkCenter
                {
                    CenterCode = "DNK-JK-01",
                    CenterName = "Srinagar GPO Dak Ghar Niryat Kendra",
                    Address = "The Bund, Residency Road",
                    City = "Srinagar",
                    State = "Jammu and Kashmir",
                    Pincode = "190001",
                    ContactPerson = "Farooq Ahmad Mir (Nodal Officer)",
                    Phone = "+91 194 245 2820",
                    Email = "dnk.srinagar@indiapost.gov.in",
                    Latitude = 34.0754,
                    Longitude = 74.8142,
                    HasCustomsDesk = true,
                    HasPackagingFacility = true,
                    OperatingHours = "09:30 AM - 05:00 PM (Mon-Sat)",
                    ServicesOffered = "Pashmina & Carpet GI Export Facilitation, EMS, Air Parcel"
                },
                new DnkCenter
                {
                    CenterCode = "DNK-CHN-01",
                    CenterName = "Chennai GPO Dak Ghar Niryat Kendra",
                    Address = "Rajaji Salai, George Town",
                    City = "Chennai",
                    State = "Tamil Nadu",
                    Pincode = "600001",
                    ContactPerson = "S. Soundararajan",
                    Phone = "+91 44 2534 1289",
                    Email = "dnk.chennaigpo@indiapost.gov.in",
                    Latitude = 13.0878,
                    Longitude = 80.2885,
                    HasCustomsDesk = true,
                    HasPackagingFacility = true,
                    OperatingHours = "08:30 AM - 06:30 PM (Mon-Sat)",
                    ServicesOffered = "Leather & Handloom Center, EMS, ITPS, Air Parcel"
                }
            };
            context.DnkCenters.AddRange(dnkCenters);
            context.SaveChanges();

            // 2. Seed Users
            var delhiCenter = dnkCenters.First(c => c.CenterCode == "DNK-DEL-01");
            var jaipurCenter = dnkCenters.First(c => c.CenterCode == "DNK-RAJ-01");

            var users = new List<User>
            {
                new User
                {
                    Username = "exporter1",
                    FullName = "Vikram Aditya (Jaipur Heritage Crafts)",
                    Email = "exports@jaipurheritagecrafts.in",
                    PhoneNumber = "+91 98290 12345",
                    Role = UserRole.Exporter,
                    BusinessName = "Jaipur Heritage Crafts & Textiles Pvt. Ltd.",
                    IecNumber = "0518920192",
                    Gstin = "08AABCT1334M1Z4",
                    AdCode = "0293841002",
                    LutNumber = "AD080326001923K",
                    Address = "Plot 42, Sitapura Industrial Area, Tonk Road",
                    City = "Jaipur",
                    State = "Rajasthan",
                    Pincode = "302022",
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new User
                {
                    Username = "exporter2",
                    FullName = "Ananya Nair (Malabar Spice Roots)",
                    Email = "contact@malabarspiceroots.com",
                    PhoneNumber = "+91 94470 98765",
                    Role = UserRole.Exporter,
                    BusinessName = "Malabar Spice Roots Exporters LLP",
                    IecNumber = "0920183921",
                    Gstin = "32AABCM9921D1ZB",
                    AdCode = "0482910394",
                    LutNumber = "AD320126004819M",
                    Address = "Building 12, Spice Park, Puthencruz",
                    City = "Kochi",
                    State = "Kerala",
                    Pincode = "682310",
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new User
                {
                    Username = "exporter3",
                    FullName = "Bashir Ahmad (Kashmir Silk & Pashmina)",
                    Email = "bashir@kashmirpashminaart.com",
                    PhoneNumber = "+91 97970 45678",
                    Role = UserRole.Exporter,
                    BusinessName = "Kashmir Royale Pashmina Guild",
                    IecNumber = "0719284729",
                    Gstin = "01AABCK4821N1Z0",
                    AdCode = "0194820193",
                    LutNumber = "AD010226008129L",
                    Address = "Near Jama Masjid, Nowhatta",
                    City = "Srinagar",
                    State = "Jammu and Kashmir",
                    Pincode = "190002",
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new User
                {
                    Username = "postal_staff",
                    FullName = "Ramesh Sharma",
                    Email = "ramesh.sharma@indiapost.gov.in",
                    PhoneNumber = "+91 98110 54321",
                    Role = UserRole.PostalStaff,
                    Designation = "Senior Postal Assistant / DNK Nodal Officer",
                    AssignedDnkCenterId = delhiCenter.Id,
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-2)
                },
                new User
                {
                    Username = "customs_officer",
                    FullName = "Vikramaditya Rathore, IRS",
                    Email = "v.rathore.customs@gov.in",
                    PhoneNumber = "+91 99100 87654",
                    Role = UserRole.CustomsOfficer,
                    Designation = "Superintendent of Customs (Postal Appraising)",
                    Address = "Delhi Foreign Post Office, Kotla Road",
                    City = "New Delhi",
                    State = "Delhi",
                    Pincode = "110002",
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-3)
                },
                new User
                {
                    Username = "admin",
                    FullName = "Director (International Mail & Exports)",
                    Email = "admin.dnk@indiapost.gov.in",
                    PhoneNumber = "+91 11 2309 6000",
                    Role = UserRole.Admin,
                    Designation = "Directorate of Posts, Dak Bhawan, Sansad Marg",
                    City = "New Delhi",
                    State = "Delhi",
                    Pincode = "110001",
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-5)
                }
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            // 3. Seed HS Code Directory
            var hsCodes = new List<HsCodeDirectory>
            {
                new HsCodeDirectory
                {
                    HsCode = "6204.42",
                    Description = "Women's or girls' dresses of cotton, handloom block printed",
                    Category = "Textiles & Garments",
                    Chapter = "Chapter 62",
                    StandardGstPercent = 5.0m,
                    ExportDutyPercent = 0.0m,
                    RodtepRatePercent = 2.5m,
                    IsRestricted = false,
                    RequiredDocuments = "Commercial Invoice, Packing List, Self Declaration Form"
                },
                new HsCodeDirectory
                {
                    HsCode = "7113.11",
                    Description = "Silver jewellery, whether or not plated or clad with other precious metal",
                    Category = "Gems & Jewellery",
                    Chapter = "Chapter 71",
                    StandardGstPercent = 3.0m,
                    ExportDutyPercent = 0.0m,
                    RodtepRatePercent = 1.0m,
                    IsRestricted = false,
                    RequiredDocuments = "Hallmarking Certificate, Valuation Certificate, GJEPC Registration"
                },
                new HsCodeDirectory
                {
                    HsCode = "0902.30",
                    Description = "Black tea (fermented) and partly fermented tea in immediate packings <= 3 kg",
                    Category = "Agricultural & Tea",
                    Chapter = "Chapter 09",
                    StandardGstPercent = 5.0m,
                    ExportDutyPercent = 0.0m,
                    RodtepRatePercent = 1.7m,
                    IsRestricted = false,
                    RequiredDocuments = "Tea Board Non-preferential Certificate, Phytosanitary Certificate"
                },
                new HsCodeDirectory
                {
                    HsCode = "0908.31",
                    Description = "Cardamom, neither crushed nor ground (Malabar / Alleppey Green)",
                    Category = "Spices",
                    Chapter = "Chapter 09",
                    StandardGstPercent = 5.0m,
                    ExportDutyPercent = 0.0m,
                    RodtepRatePercent = 2.0m,
                    IsRestricted = false,
                    RequiredDocuments = "Spices Board Certificate of Registration, Phytosanitary Certificate"
                },
                new HsCodeDirectory
                {
                    HsCode = "7419.80",
                    Description = "Handicrafts of brass, ornamental artware, vases, statues, candle holders",
                    Category = "Handicrafts & Metalware",
                    Chapter = "Chapter 74",
                    StandardGstPercent = 12.0m,
                    ExportDutyPercent = 0.0m,
                    RodtepRatePercent = 1.8m,
                    IsRestricted = false,
                    RequiredDocuments = "Commercial Invoice, EPCH Certificate of Origin"
                },
                new HsCodeDirectory
                {
                    HsCode = "5701.10",
                    Description = "Carpets and other textile floor coverings, knotted, of wool or fine animal hair",
                    Category = "Handicrafts & Carpets",
                    Chapter = "Chapter 57",
                    StandardGstPercent = 12.0m,
                    ExportDutyPercent = 0.0m,
                    RodtepRatePercent = 3.1m,
                    IsRestricted = false,
                    RequiredDocuments = "CEPC Registration, GI Certification (if Kashmiri)"
                },
                new HsCodeDirectory
                {
                    HsCode = "3304.99",
                    Description = "Ayurvedic herbal cosmetic preparations for the care of the skin (creams/lotions)",
                    Category = "Ayush & Cosmetics",
                    Chapter = "Chapter 33",
                    StandardGstPercent = 18.0m,
                    ExportDutyPercent = 0.0m,
                    RodtepRatePercent = 1.2m,
                    IsRestricted = false,
                    RequiredDocuments = "Ayush Drug Manufacturing License, Certificate of Analysis (COA)"
                },
                new HsCodeDirectory
                {
                    HsCode = "4202.21",
                    Description = "Handbags with outer surface of leather, handcrafted",
                    Category = "Leather Goods",
                    Chapter = "Chapter 42",
                    StandardGstPercent = 18.0m,
                    ExportDutyPercent = 0.0m,
                    RodtepRatePercent = 2.4m,
                    IsRestricted = false,
                    RequiredDocuments = "CLE Registration, Non-CITES Animal Leather Declaration"
                },
                new HsCodeDirectory
                {
                    HsCode = "9503.00",
                    Description = "Traditional handcrafted non-mechanized wooden toys (Channapatna / Kondapalli)",
                    Category = "Toys & Crafts",
                    Chapter = "Chapter 95",
                    StandardGstPercent = 12.0m,
                    ExportDutyPercent = 0.0m,
                    RodtepRatePercent = 1.5m,
                    IsRestricted = false,
                    RequiredDocuments = "Non-toxic vegetable lacquer test report"
                }
            };
            context.HsCodeDirectories.AddRange(hsCodes);
            context.SaveChanges();

            // 4. Seed Tariff Zones for EMS, Air Parcel, ITPS, Registered Mail
            var tariffZones = new List<TariffZone>
            {
                // India (Domestic & Inland SEZ)
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "IN", CountryName = "India", PostalZone = "Domestic & SEZ", BaseWeightGrams = 250, BaseRateINR = 200m, StepWeightGrams = 250, StepRateINR = 40m, MinDeliveryDays = 1, MaxDeliveryDays = 3, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 5m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.InternationalAirParcel, CountryCode = "IN", CountryName = "India", PostalZone = "Domestic & SEZ", BaseWeightGrams = 500, BaseRateINR = 150m, StepWeightGrams = 500, StepRateINR = 50m, MinDeliveryDays = 2, MaxDeliveryDays = 4, MaxAllowedWeightKg = 20m, FuelSurchargePercent = 5m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "IN", CountryName = "India", PostalZone = "Domestic & SEZ", BaseWeightGrams = 100, BaseRateINR = 80m, StepWeightGrams = 100, StepRateINR = 20m, MinDeliveryDays = 1, MaxDeliveryDays = 3, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 5m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.RegisteredAirMail, CountryCode = "IN", CountryName = "India", PostalZone = "Domestic & SEZ", BaseWeightGrams = 50, BaseRateINR = 50m, StepWeightGrams = 50, StepRateINR = 15m, MinDeliveryDays = 2, MaxDeliveryDays = 5, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 5m, GstPercent = 18m },

                // USA
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "US", CountryName = "United States", PostalZone = "Zone 4 (Americas)", BaseWeightGrams = 250, BaseRateINR = 1250m, StepWeightGrams = 250, StepRateINR = 280m, MinDeliveryDays = 4, MaxDeliveryDays = 7, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.InternationalAirParcel, CountryCode = "US", CountryName = "United States", PostalZone = "Zone 4 (Americas)", BaseWeightGrams = 500, BaseRateINR = 1100m, StepWeightGrams = 500, StepRateINR = 380m, MinDeliveryDays = 7, MaxDeliveryDays = 12, MaxAllowedWeightKg = 20m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "US", CountryName = "United States", PostalZone = "Zone 4 (Americas)", BaseWeightGrams = 100, BaseRateINR = 450m, StepWeightGrams = 100, StepRateINR = 95m, MinDeliveryDays = 6, MaxDeliveryDays = 10, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 8m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.RegisteredAirMail, CountryCode = "US", CountryName = "United States", PostalZone = "Zone 4 (Americas)", BaseWeightGrams = 50, BaseRateINR = 280m, StepWeightGrams = 50, StepRateINR = 60m, MinDeliveryDays = 10, MaxDeliveryDays = 18, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 5m, GstPercent = 18m },

                // United Kingdom
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "GB", CountryName = "United Kingdom", PostalZone = "Zone 3 (Europe)", BaseWeightGrams = 250, BaseRateINR = 1100m, StepWeightGrams = 250, StepRateINR = 240m, MinDeliveryDays = 3, MaxDeliveryDays = 6, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.InternationalAirParcel, CountryCode = "GB", CountryName = "United Kingdom", PostalZone = "Zone 3 (Europe)", BaseWeightGrams = 500, BaseRateINR = 950m, StepWeightGrams = 500, StepRateINR = 320m, MinDeliveryDays = 6, MaxDeliveryDays = 10, MaxAllowedWeightKg = 20m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "GB", CountryName = "United Kingdom", PostalZone = "Zone 3 (Europe)", BaseWeightGrams = 100, BaseRateINR = 400m, StepWeightGrams = 100, StepRateINR = 80m, MinDeliveryDays = 5, MaxDeliveryDays = 9, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 8m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.RegisteredAirMail, CountryCode = "GB", CountryName = "United Kingdom", PostalZone = "Zone 3 (Europe)", BaseWeightGrams = 50, BaseRateINR = 250m, StepWeightGrams = 50, StepRateINR = 50m, MinDeliveryDays = 8, MaxDeliveryDays = 15, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 5m, GstPercent = 18m },

                // UAE (Middle East)
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "AE", CountryName = "United Arab Emirates", PostalZone = "Zone 1 (Gulf & ME)", BaseWeightGrams = 250, BaseRateINR = 750m, StepWeightGrams = 250, StepRateINR = 160m, MinDeliveryDays = 2, MaxDeliveryDays = 5, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.InternationalAirParcel, CountryCode = "AE", CountryName = "United Arab Emirates", PostalZone = "Zone 1 (Gulf & ME)", BaseWeightGrams = 500, BaseRateINR = 680m, StepWeightGrams = 500, StepRateINR = 210m, MinDeliveryDays = 5, MaxDeliveryDays = 8, MaxAllowedWeightKg = 20m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "AE", CountryName = "United Arab Emirates", PostalZone = "Zone 1 (Gulf & ME)", BaseWeightGrams = 100, BaseRateINR = 320m, StepWeightGrams = 100, StepRateINR = 60m, MinDeliveryDays = 4, MaxDeliveryDays = 7, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 8m, GstPercent = 18m },

                // Germany
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "DE", CountryName = "Germany", PostalZone = "Zone 3 (Europe)", BaseWeightGrams = 250, BaseRateINR = 1150m, StepWeightGrams = 250, StepRateINR = 250m, MinDeliveryDays = 4, MaxDeliveryDays = 7, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "DE", CountryName = "Germany", PostalZone = "Zone 3 (Europe)", BaseWeightGrams = 100, BaseRateINR = 420m, StepWeightGrams = 100, StepRateINR = 85m, MinDeliveryDays = 5, MaxDeliveryDays = 9, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 8m, GstPercent = 18m },

                // Australia
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "AU", CountryName = "Australia", PostalZone = "Zone 2 (Asia-Pacific)", BaseWeightGrams = 250, BaseRateINR = 1200m, StepWeightGrams = 250, StepRateINR = 260m, MinDeliveryDays = 4, MaxDeliveryDays = 8, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "AU", CountryName = "Australia", PostalZone = "Zone 2 (Asia-Pacific)", BaseWeightGrams = 100, BaseRateINR = 440m, StepWeightGrams = 100, StepRateINR = 90m, MinDeliveryDays = 6, MaxDeliveryDays = 10, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 8m, GstPercent = 18m },

                // Canada
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "CA", CountryName = "Canada", PostalZone = "Zone 4 (Americas)", BaseWeightGrams = 250, BaseRateINR = 1300m, StepWeightGrams = 250, StepRateINR = 290m, MinDeliveryDays = 5, MaxDeliveryDays = 9, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "CA", CountryName = "Canada", PostalZone = "Zone 4 (Americas)", BaseWeightGrams = 100, BaseRateINR = 460m, StepWeightGrams = 100, StepRateINR = 95m, MinDeliveryDays = 6, MaxDeliveryDays = 11, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 8m, GstPercent = 18m },

                // Singapore
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "SG", CountryName = "Singapore", PostalZone = "Zone 2 (Asia-Pacific)", BaseWeightGrams = 250, BaseRateINR = 800m, StepWeightGrams = 250, StepRateINR = 170m, MinDeliveryDays = 3, MaxDeliveryDays = 5, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "SG", CountryName = "Singapore", PostalZone = "Zone 2 (Asia-Pacific)", BaseWeightGrams = 100, BaseRateINR = 340m, StepWeightGrams = 100, StepRateINR = 65m, MinDeliveryDays = 4, MaxDeliveryDays = 7, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 8m, GstPercent = 18m },

                // Japan
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "JP", CountryName = "Japan", PostalZone = "Zone 2 (Asia-Pacific)", BaseWeightGrams = 250, BaseRateINR = 950m, StepWeightGrams = 250, StepRateINR = 200m, MinDeliveryDays = 3, MaxDeliveryDays = 6, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "JP", CountryName = "Japan", PostalZone = "Zone 2 (Asia-Pacific)", BaseWeightGrams = 100, BaseRateINR = 380m, StepWeightGrams = 100, StepRateINR = 75m, MinDeliveryDays = 5, MaxDeliveryDays = 8, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 8m, GstPercent = 18m },

                // France
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "FR", CountryName = "France", PostalZone = "Zone 3 (Europe)", BaseWeightGrams = 250, BaseRateINR = 1120m, StepWeightGrams = 250, StepRateINR = 245m, MinDeliveryDays = 4, MaxDeliveryDays = 7, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 10m, GstPercent = 18m },

                // Others / Rest of the World
                new TariffZone { ServiceType = ServiceType.EMS_SpeedPost, CountryCode = "OTH", CountryName = "Others (Rest of the World)", PostalZone = "Zone 5 (Global Rest)", BaseWeightGrams = 250, BaseRateINR = 1500m, StepWeightGrams = 250, StepRateINR = 300m, MinDeliveryDays = 5, MaxDeliveryDays = 12, MaxAllowedWeightKg = 30m, FuelSurchargePercent = 12m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.InternationalAirParcel, CountryCode = "OTH", CountryName = "Others (Rest of the World)", PostalZone = "Zone 5 (Global Rest)", BaseWeightGrams = 500, BaseRateINR = 1250m, StepWeightGrams = 500, StepRateINR = 400m, MinDeliveryDays = 8, MaxDeliveryDays = 15, MaxAllowedWeightKg = 20m, FuelSurchargePercent = 12m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.ITPS_TrackedPacket, CountryCode = "OTH", CountryName = "Others (Rest of the World)", PostalZone = "Zone 5 (Global Rest)", BaseWeightGrams = 100, BaseRateINR = 500m, StepWeightGrams = 100, StepRateINR = 100m, MinDeliveryDays = 7, MaxDeliveryDays = 14, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 10m, GstPercent = 18m },
                new TariffZone { ServiceType = ServiceType.RegisteredAirMail, CountryCode = "OTH", CountryName = "Others (Rest of the World)", PostalZone = "Zone 5 (Global Rest)", BaseWeightGrams = 50, BaseRateINR = 300m, StepWeightGrams = 50, StepRateINR = 70m, MinDeliveryDays = 10, MaxDeliveryDays = 20, MaxAllowedWeightKg = 2m, FuelSurchargePercent = 8m, GstPercent = 18m }
            };
            context.TariffZones.AddRange(tariffZones);
            context.SaveChanges();

            // 5. Seed Consignments & Tracking Milestones
            var exp1 = users.First(u => u.Username == "exporter1");
            var exp2 = users.First(u => u.Username == "exporter2");
            var exp3 = users.First(u => u.Username == "exporter3");

            // Consignment 1: Delivered (Handcrafted Brass Idol to Dubai)
            var c1 = new ExportConsignment
            {
                TrackingNumber = "LP928410293IN",
                PbeNumber = "PBE-I/2026/DEL/000142",
                PbeType = PbeType.PBE_I,
                ServiceType = ServiceType.ITPS_TrackedPacket,
                Status = ConsignmentStatus.Delivered,
                CustomsStatus = CustomsStatus.Approved,
                ExporterId = exp1.Id,
                DnkCenterId = jaipurCenter.Id,
                RecipientName = "Omar Al-Mansoor",
                RecipientCompany = "Gulf Heritage Arts LLC",
                RecipientEmail = "o.mansoor@gulfheritage.ae",
                RecipientPhone = "+971 50 123 4567",
                RecipientAddress = "Villa 14, Al Safa 2, Jumeirah",
                RecipientCity = "Dubai",
                RecipientState = "Dubai",
                RecipientPostalCode = "337-1500",
                RecipientCountry = "United Arab Emirates",
                DestinationCountryCode = "AE",
                ActualWeightKg = 1.20m,
                LengthCm = 24.0m,
                WidthCm = 18.0m,
                HeightCm = 12.0m,
                VolumetricWeightKg = 1.04m,
                ChargeableWeightKg = 1.20m,
                InvoiceNumber = "EXP-JHC-2026-088",
                InvoiceDate = DateTime.UtcNow.AddDays(-10),
                LutNumber = exp1.LutNumber,
                NatureOfExport = "Sale of Goods (E-Commerce)",
                CurrencyCode = "AED",
                DeclaredValueForeign = 650.0m,
                ExchangeRateToINR = 23.50m,
                DeclaredValueINR = 15275.0m,
                PostageAmountINR = 1040.0m,
                FuelSurchargeINR = 83.20m,
                InsuranceAmountINR = 152.75m,
                GstAmountINR = 229.67m,
                TotalShippingCostINR = 1505.62m,
                CustomsAssessedDutyINR = 0.0m,
                CustomsOfficerRemarks = "Verified under PBE-I. Non-restricted handicraft artware.",
                ScrutinyOfficerName = "Vikramaditya Rathore, IRS",
                CustomsClearedAt = DateTime.UtcNow.AddDays(-7),
                InductedAt = DateTime.UtcNow.AddDays(-9),
                InductedByStaffName = "Sunita Verma",
                VerifiedWeightKg = 1.20m,
                DeliveredAt = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            };

            c1.Items.Add(new ConsignmentItem
            {
                ItemDescription = "Handcrafted Brass Peacock Decorative Lamp",
                HsCode = "7419.80",
                Quantity = 2,
                UnitOfMeasurement = "PCS",
                UnitPriceINR = 7637.50m,
                TotalPriceINR = 15275.0m,
                NetWeightKg = 1.15m,
                CountryOfOrigin = "India"
            });

            c1.Milestones.AddRange(new[]
            {
                new TrackingMilestone { TrackingNumber = "LP928410293IN", EventTime = DateTime.UtcNow.AddDays(-10), FacilityLocation = "Jaipur HPO DNK Portal", StatusHeadline = "Electronic PBE Filed & Article Booked", StatusCategory = ConsignmentStatus.Booked, DetailedDescription = "Consignment data and Customs Declaration e-PBE uploaded successfully." },
                new TrackingMilestone { TrackingNumber = "LP928410293IN", EventTime = DateTime.UtcNow.AddDays(-9), FacilityLocation = "Jaipur HPO Dak Ghar Niryat Kendra", StatusHeadline = "Article Inducted at DNK Counter", StatusCategory = ConsignmentStatus.InductedAtDNK, DetailedDescription = "Physical package received, weighment verified (1.20 Kg), barcode scanned.", PerformedBy = "Sunita Verma" },
                new TrackingMilestone { TrackingNumber = "LP928410293IN", EventTime = DateTime.UtcNow.AddDays(-8), FacilityLocation = "Delhi Foreign Post Office (FPO Kotla Road)", StatusHeadline = "Received at Office of Exchange / FPO", StatusCategory = ConsignmentStatus.ReceivedAtFPO, DetailedDescription = "Bag opened, submitted to Customs Postal Appraising Department for clearance." },
                new TrackingMilestone { TrackingNumber = "LP928410293IN", EventTime = DateTime.UtcNow.AddDays(-7), FacilityLocation = "Delhi FPO Customs Wing", StatusHeadline = "Customs Scrutiny Cleared", StatusCategory = ConsignmentStatus.CustomsCleared, DetailedDescription = "PBE-I verified against invoice. Electronic Out of Charge (OOC) granted.", PerformedBy = "Vikramaditya Rathore, IRS" },
                new TrackingMilestone { TrackingNumber = "LP928410293IN", EventTime = DateTime.UtcNow.AddDays(-6), FacilityLocation = "Indira Gandhi International Airport Hub (DEL)", StatusHeadline = "Departed Origin Country (India)", StatusCategory = ConsignmentStatus.DepartedOriginCountry, DetailedDescription = "Air dispatch on Flight EK511 to Dubai International Airport (DXB)." },
                new TrackingMilestone { TrackingNumber = "LP928410293IN", EventTime = DateTime.UtcNow.AddDays(-4), FacilityLocation = "Emirates Post Central Sorting Hub, Dubai", StatusHeadline = "Arrived at Destination Hub & Inward Customs Cleared", StatusCategory = ConsignmentStatus.CustomsAtDestination, DetailedDescription = "Received at Dubai Hub, inbound customs cleared without duty." },
                new TrackingMilestone { TrackingNumber = "LP928410293IN", EventTime = DateTime.UtcNow.AddDays(-2), FacilityLocation = "Dubai Jumeirah Delivery Depot", StatusHeadline = "Out for Delivery & Item Delivered", StatusCategory = ConsignmentStatus.Delivered, DetailedDescription = "Delivered to recipient Omar Al-Mansoor. Recipient signature recorded." }
            });

            // Consignment 2: In Transit / Cleared (Jaipur Cotton Block Printed Dresses to USA)
            var c2 = new ExportConsignment
            {
                TrackingNumber = "EM892104928IN",
                PbeNumber = "PBE-I/2026/DEL/000204",
                PbeType = PbeType.PBE_I,
                ServiceType = ServiceType.EMS_SpeedPost,
                Status = ConsignmentStatus.DepartedOriginCountry,
                CustomsStatus = CustomsStatus.Approved,
                ExporterId = exp1.Id,
                DnkCenterId = jaipurCenter.Id,
                RecipientName = "Sophia Anderson",
                RecipientCompany = "Boho Chic Boutiques",
                RecipientEmail = "sophia@bohochic.com",
                RecipientPhone = "+1 415 889 0192",
                RecipientAddress = "842 Market Street, Suite 400",
                RecipientCity = "San Francisco",
                RecipientState = "California",
                RecipientPostalCode = "94102",
                RecipientCountry = "United States",
                DestinationCountryCode = "US",
                ActualWeightKg = 2.40m,
                LengthCm = 35.0m,
                WidthCm = 28.0m,
                HeightCm = 15.0m,
                VolumetricWeightKg = 2.94m,
                ChargeableWeightKg = 2.94m,
                InvoiceNumber = "EXP-JHC-2026-095",
                InvoiceDate = DateTime.UtcNow.AddDays(-4),
                LutNumber = exp1.LutNumber,
                NatureOfExport = "Sale of Goods (Commercial)",
                CurrencyCode = "USD",
                DeclaredValueForeign = 480.0m,
                ExchangeRateToINR = 86.50m,
                DeclaredValueINR = 41520.0m,
                PostageAmountINR = 3770.0m,
                FuelSurchargeINR = 377.0m,
                InsuranceAmountINR = 415.20m,
                GstAmountINR = 821.19m,
                TotalShippingCostINR = 5383.39m,
                CustomsAssessedDutyINR = 0.0m,
                CustomsOfficerRemarks = "Handloom textiles verified with Self Declaration. RoDTEP claimed.",
                ScrutinyOfficerName = "Vikramaditya Rathore, IRS",
                CustomsClearedAt = DateTime.UtcNow.AddDays(-2),
                InductedAt = DateTime.UtcNow.AddDays(-3),
                InductedByStaffName = "Sunita Verma",
                VerifiedWeightKg = 2.40m,
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            };

            c2.Items.Add(new ConsignmentItem
            {
                ItemDescription = "Pure Cotton Hand Block Printed Maxi Dresses",
                HsCode = "6204.42",
                Quantity = 8,
                UnitOfMeasurement = "PCS",
                UnitPriceINR = 5190.0m,
                TotalPriceINR = 41520.0m,
                NetWeightKg = 2.30m,
                CountryOfOrigin = "India"
            });

            c2.Milestones.AddRange(new[]
            {
                new TrackingMilestone { TrackingNumber = "EM892104928IN", EventTime = DateTime.UtcNow.AddDays(-4), FacilityLocation = "Jaipur DNK System", StatusHeadline = "Postal Bill of Export e-Filed", StatusCategory = ConsignmentStatus.Booked, DetailedDescription = "Consignment booked under International EMS with LUT zero-rated IGST." },
                new TrackingMilestone { TrackingNumber = "EM892104928IN", EventTime = DateTime.UtcNow.AddDays(-3), FacilityLocation = "Jaipur HPO Dak Ghar Niryat Kendra", StatusHeadline = "Article Inducted & Weighed", StatusCategory = ConsignmentStatus.InductedAtDNK, DetailedDescription = "Article verified. Dimensions: 35x28x15 cm. Dispatched in secured mail van to Delhi FPO." },
                new TrackingMilestone { TrackingNumber = "EM892104928IN", EventTime = DateTime.UtcNow.AddDays(-2), FacilityLocation = "Delhi Foreign Post Office", StatusHeadline = "Customs Out of Charge (OOC) Granted", StatusCategory = ConsignmentStatus.CustomsCleared, DetailedDescription = "PBE-I scrutinized and cleared. Forwarded to Air Mail Sorting Division." },
                new TrackingMilestone { TrackingNumber = "EM892104928IN", EventTime = DateTime.UtcNow.AddHours(-18), FacilityLocation = "IGIA Air Cargo Hub, New Delhi", StatusHeadline = "Departed from Origin Country", StatusCategory = ConsignmentStatus.DepartedOriginCountry, DetailedDescription = "Dispatched on United Airlines Flight UA83 to Newark / SFO Hub." }
            });

            // Consignment 3: Customs Hold / Scrutiny Demo (Channapatna Wooden Toys to Canada)
            var c3 = new ExportConsignment
            {
                TrackingNumber = "EM109283746IN",
                PbeNumber = "PBE-II/2026/DEL/000318",
                PbeType = PbeType.PBE_II,
                ServiceType = ServiceType.EMS_SpeedPost,
                Status = ConsignmentStatus.CustomsHold,
                CustomsStatus = CustomsStatus.HoldForQuery,
                ExporterId = exp3.Id,
                DnkCenterId = delhiCenter.Id,
                RecipientName = "Liam Tremblay",
                RecipientCompany = "Maple Crafts Imports",
                RecipientEmail = "liam@maplecrafts.ca",
                RecipientPhone = "+1 416 902 4432",
                RecipientAddress = "220 Bay Street, Floor 14",
                RecipientCity = "Toronto",
                RecipientState = "Ontario",
                RecipientPostalCode = "M5J 2W4",
                RecipientCountry = "Canada",
                DestinationCountryCode = "CA",
                ActualWeightKg = 3.50m,
                LengthCm = 40.0m,
                WidthCm = 30.0m,
                HeightCm = 20.0m,
                VolumetricWeightKg = 4.80m,
                ChargeableWeightKg = 4.80m,
                InvoiceNumber = "EXP-KS-2026-041",
                InvoiceDate = DateTime.UtcNow.AddDays(-2),
                LutNumber = exp3.LutNumber,
                NatureOfExport = "Commercial Sample / Gift",
                CurrencyCode = "CAD",
                DeclaredValueForeign = 850.0m,
                ExchangeRateToINR = 63.20m,
                DeclaredValueINR = 53720.0m,
                PostageAmountINR = 5940.0m,
                FuelSurchargeINR = 594.0m,
                InsuranceAmountINR = 537.20m,
                GstAmountINR = 1272.81m,
                TotalShippingCostINR = 8344.01m,
                CustomsAssessedDutyINR = 0.0m,
                CustomsOfficerRemarks = "Query raised: Non-toxic natural dye certification and itemized valuation required for commercial samples.",
                ScrutinyOfficerName = "Vikramaditya Rathore, IRS",
                InductedAt = DateTime.UtcNow.AddDays(-1),
                InductedByStaffName = "Ramesh Sharma",
                VerifiedWeightKg = 3.50m,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            };

            c3.Items.Add(new ConsignmentItem
            {
                ItemDescription = "Traditional Channapatna Lacquer-ware Wooden Stacking Toys & Figurines",
                HsCode = "9503.00",
                Quantity = 15,
                UnitOfMeasurement = "SET",
                UnitPriceINR = 3581.33m,
                TotalPriceINR = 53720.0m,
                NetWeightKg = 3.40m,
                CountryOfOrigin = "India"
            });

            c3.Milestones.AddRange(new[]
            {
                new TrackingMilestone { TrackingNumber = "EM109283746IN", EventTime = DateTime.UtcNow.AddDays(-2), FacilityLocation = "New Delhi GPO DNK Portal", StatusHeadline = "PBE-II Filed & Consignment Booked", StatusCategory = ConsignmentStatus.Booked, DetailedDescription = "Article booked with declared sample value." },
                new TrackingMilestone { TrackingNumber = "EM109283746IN", EventTime = DateTime.UtcNow.AddDays(-1), FacilityLocation = "New Delhi GPO DNK Counter", StatusHeadline = "Article Inducted at Post Office", StatusCategory = ConsignmentStatus.InductedAtDNK, DetailedDescription = "Package handed over to nodal officer Ramesh Sharma. Transferred to Delhi FPO.", PerformedBy = "Ramesh Sharma" },
                new TrackingMilestone { TrackingNumber = "EM109283746IN", EventTime = DateTime.UtcNow.AddHours(-10), FacilityLocation = "Delhi FPO Customs Appraising Wing", StatusHeadline = "Customs Query / Hold Placed", StatusCategory = ConsignmentStatus.CustomsHold, DetailedDescription = "Customs official requested material safety certification. Exporter notified via SMS/Email.", PerformedBy = "Vikramaditya Rathore, IRS" }
            });

            // Consignment 4: Inducted at DNK, ready for dispatch (Kashmiri Pashmina to France)
            var c4 = new ExportConsignment
            {
                TrackingNumber = "EM419208374IN",
                PbeNumber = "PBE-I/2026/DEL/000455",
                PbeType = PbeType.PBE_I,
                ServiceType = ServiceType.EMS_SpeedPost,
                Status = ConsignmentStatus.InductedAtDNK,
                CustomsStatus = CustomsStatus.PendingVerification,
                ExporterId = exp3.Id,
                DnkCenterId = delhiCenter.Id,
                RecipientName = "Camille Laurent",
                RecipientCompany = "Maison de Cachemire",
                RecipientEmail = "c.laurent@maisoncachemire.fr",
                RecipientPhone = "+33 1 42 68 55 00",
                RecipientAddress = "18 Rue du Faubourg Saint-Honore",
                RecipientCity = "Paris",
                RecipientState = "Ile-de-France",
                RecipientPostalCode = "75008",
                RecipientCountry = "France",
                DestinationCountryCode = "FR",
                ActualWeightKg = 0.85m,
                LengthCm = 30.0m,
                WidthCm = 22.0m,
                HeightCm = 8.0m,
                VolumetricWeightKg = 1.05m,
                ChargeableWeightKg = 1.05m,
                InvoiceNumber = "EXP-KS-2026-052",
                InvoiceDate = DateTime.UtcNow.AddDays(-1),
                LutNumber = exp3.LutNumber,
                NatureOfExport = "Sale of Goods (Commercial)",
                CurrencyCode = "EUR",
                DeclaredValueForeign = 720.0m,
                ExchangeRateToINR = 92.40m,
                DeclaredValueINR = 66528.0m,
                PostageAmountINR = 2100.0m,
                FuelSurchargeINR = 210.0m,
                InsuranceAmountINR = 665.28m,
                GstAmountINR = 535.55m,
                TotalShippingCostINR = 3510.83m,
                InductedAt = DateTime.UtcNow.AddHours(-4),
                InductedByStaffName = "Ramesh Sharma",
                VerifiedWeightKg = 0.85m,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            c4.Items.Add(new ConsignmentItem
            {
                ItemDescription = "GI Tagged Authentic Pure Kashmiri Pashmina Handspun Shawl",
                HsCode = "5701.10",
                Quantity = 2,
                UnitOfMeasurement = "PCS",
                UnitPriceINR = 33264.0m,
                TotalPriceINR = 66528.0m,
                NetWeightKg = 0.80m,
                CountryOfOrigin = "India"
            });

            c4.Milestones.AddRange(new[]
            {
                new TrackingMilestone { TrackingNumber = "EM419208374IN", EventTime = DateTime.UtcNow.AddDays(-1), FacilityLocation = "Delhi DNK Online Portal", StatusHeadline = "e-PBE Filed & Label Generated", StatusCategory = ConsignmentStatus.Booked, DetailedDescription = "Consignment registered with GI Pashmina details." },
                new TrackingMilestone { TrackingNumber = "EM419208374IN", EventTime = DateTime.UtcNow.AddHours(-4), FacilityLocation = "New Delhi GPO DNK Counter", StatusHeadline = "Article Inducted & Scanned", StatusCategory = ConsignmentStatus.InductedAtDNK, DetailedDescription = "Article verified by staff Ramesh Sharma. Staged for FPO Bagging.", PerformedBy = "Ramesh Sharma" }
            });

            // Consignment 5: Booked / Pickup Requested (Organic Spices to UK)
            var c5 = new ExportConsignment
            {
                TrackingNumber = "CP771209384IN",
                PbeNumber = "PBE-I/2026/DEL/000512",
                PbeType = PbeType.PBE_I,
                ServiceType = ServiceType.InternationalAirParcel,
                Status = ConsignmentStatus.PickupScheduled,
                CustomsStatus = CustomsStatus.PendingVerification,
                ExporterId = exp2.Id,
                DnkCenterId = delhiCenter.Id,
                RecipientName = "David Macallister",
                RecipientCompany = "Highland Organic Stores",
                RecipientEmail = "david@highlandorganics.co.uk",
                RecipientPhone = "+44 20 7946 0912",
                RecipientAddress = "72 Victoria Street, Westminster",
                RecipientCity = "London",
                RecipientState = "Greater London",
                RecipientPostalCode = "SW1E 6QP",
                RecipientCountry = "United Kingdom",
                DestinationCountryCode = "GB",
                ActualWeightKg = 5.00m,
                LengthCm = 35.0m,
                WidthCm = 30.0m,
                HeightCm = 25.0m,
                VolumetricWeightKg = 5.25m,
                ChargeableWeightKg = 5.25m,
                InvoiceNumber = "EXP-MSR-2026-103",
                InvoiceDate = DateTime.UtcNow,
                LutNumber = exp2.LutNumber,
                NatureOfExport = "Sale of Goods (Commercial)",
                CurrencyCode = "GBP",
                DeclaredValueForeign = 350.0m,
                ExchangeRateToINR = 111.20m,
                DeclaredValueINR = 38920.0m,
                PostageAmountINR = 3990.0m,
                FuelSurchargeINR = 399.0m,
                InsuranceAmountINR = 389.20m,
                GstAmountINR = 860.07m,
                TotalShippingCostINR = 5638.27m,
                IsPickupRequested = true,
                PickupStatus = PickupStatus.Assigned,
                ScheduledPickupDate = DateTime.UtcNow.Date.AddDays(1),
                PickupTimeSlot = TimeSlot.Morning_09_12,
                CreatedAt = DateTime.UtcNow
            };

            c5.Items.Add(new ConsignmentItem
            {
                ItemDescription = "Organic Malabar Green Cardamom Grade A (1kg vacuum pouches)",
                HsCode = "0908.31",
                Quantity = 5,
                UnitOfMeasurement = "KGS",
                UnitPriceINR = 7784.0m,
                TotalPriceINR = 38920.0m,
                NetWeightKg = 5.0m,
                CountryOfOrigin = "India"
            });

            c5.Milestones.AddRange(new[]
            {
                new TrackingMilestone { TrackingNumber = "CP771209384IN", EventTime = DateTime.UtcNow, FacilityLocation = "Malabar DNK Exporter Desk", StatusHeadline = "Postal Bill of Export Registered & Pickup Scheduled", StatusCategory = ConsignmentStatus.PickupScheduled, DetailedDescription = "Doorstep pickup assigned to postman for tomorrow 09:00 AM - 12:00 PM." }
            });

            context.ExportConsignments.AddRange(new[] { c1, c2, c3, c4, c5 });
            context.SaveChanges();

            // Seed Pickup Requests
            var pickup1 = new PickupRequest
            {
                ExportConsignmentId = c5.Id,
                ExporterId = exp2.Id,
                ScheduledDate = DateTime.UtcNow.Date.AddDays(1),
                TimeSlot = TimeSlot.Morning_09_12,
                PickupAddress = exp2.Address + ", " + exp2.City + " - " + exp2.Pincode,
                Pincode = exp2.Pincode ?? "682310",
                Status = PickupStatus.Assigned,
                AssignedPostmanName = "K. Manoj Kumar (Speed Post Courier)",
                AssignedPostmanPhone = "+91 94460 11223",
                SpecialInstructions = "Heavy spice carton with fragile/aroma-tight seal.",
                CreatedAt = DateTime.UtcNow
            };
            context.PickupRequests.Add(pickup1);

            // Seed a Dispatch Manifest
            var manifest1 = new DispatchManifest
            {
                ManifestNumber = "MNF/DNK-DEL/20260910/01",
                OriginDnkCenterId = delhiCenter.Id,
                DestinationFpo = "Delhi Foreign Post Office (FPO Kotla Road)",
                DispatchDate = DateTime.UtcNow.AddHours(-3),
                BagBarcodeNumber = "BAG-IND-DEL-984210",
                SealNumber = "SL-DEL-482918",
                TotalArticles = 3,
                TotalWeightKg = 6.75m,
                CreatedByStaff = "Ramesh Sharma",
                Status = "Dispatched"
            };
            context.DispatchManifests.Add(manifest1);
            context.SaveChanges();
        }
    }
}
