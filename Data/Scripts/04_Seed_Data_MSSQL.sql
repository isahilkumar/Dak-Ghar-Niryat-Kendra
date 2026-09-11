-- ====================================================================================
-- DAK GHAR NIRYAT KENDRA (DNK) - ENTERPRISE SEED DATA (T-SQL / MSSQL)
-- Department of Posts, Ministry of Communications, Government of India
-- ====================================================================================

USE [DakGharNiryatKendraDB];
GO

SET NOCOUNT ON;

-- 1. Seed Master.DnkCenters
SET IDENTITY_INSERT [Master].[DnkCenters] ON;

INSERT INTO [Master].[DnkCenters] ([Id], [CenterCode], [CenterName], [Address], [City], [State], [Pincode], [ContactPerson], [Phone], [Email], [Latitude], [Longitude], [IsActive], [HasCustomsDesk], [HasPackagingFacility], [OperatingHours], [ServicesOffered])
VALUES
(1, N'DNK-DEL-01', N'New Delhi GPO Dak Ghar Niryat Kendra', N'GPO Complex, Ashoka Road, Connaught Place', N'New Delhi', N'Delhi', N'110001', N'Rajiv Ranjan (Superintendent)', N'+91 11 2336 4111', N'dnk.delhigpo@indiapost.gov.in', 28.6253, 77.2090, 1, 1, 1, N'08:30 AM - 07:00 PM (Mon-Sat)', N'International EMS, Air Parcel, ITPS, Customs Scrutiny, Packaging Support'),
(2, N'DNK-RAJ-01', N'Jaipur Head Post Office DNK', N'MI Road, Near Panch Batti', N'Jaipur', N'Rajasthan', N'302001', N'Anil Singhal (Nodal Officer)', N'+91 141 236 8920', N'dnk.jaipurhpo@indiapost.gov.in', 26.9196, 75.8080, 1, 1, 1, N'09:00 AM - 06:00 PM (Mon-Sat)', N'Textiles & Handicrafts Special Hub, EMS, Air Parcel, ITPS'),
(3, N'DNK-MUM-01', N'Mumbai GPO Dak Ghar Niryat Kendra', N'GPO Building, CST Area, Fort', N'Mumbai', N'Maharashtra', N'400001', N'Pooja Kulkarni (Asst. Director)', N'+91 22 2262 0524', N'dnk.mumbaigpo@indiapost.gov.in', 18.9402, 72.8356, 1, 1, 1, N'08:00 AM - 08:00 PM (Mon-Sat)', N'Gems & Jewellery Hub, EMS, Air Parcel, ITPS'),
(4, N'DNK-KER-01', N'Kochi Head Post Office DNK', N'Park Avenue, Marine Drive', N'Kochi', N'Kerala', N'682011', N'K. Sreekumar (Nodal Officer)', N'+91 484 235 2200', N'dnk.kochihpo@indiapost.gov.in', 9.9723, 76.2783, 1, 1, 1, N'09:00 AM - 05:30 PM (Mon-Sat)', N'Spices & Organic Exports Nodal Kendra, EMS, Air Parcel, ITPS'),
(5, N'DNK-BLR-01', N'Bengaluru GPO Dak Ghar Niryat Kendra', N'Raj Bhavan Road, Ambedkar Veedhi', N'Bengaluru', N'Karnataka', N'560001', N'Venkatesh Murthy (Senior Postmaster)', N'+91 80 2286 7890', N'dnk.blr.gpo@indiapost.gov.in', 12.9815, 77.5921, 1, 1, 1, N'08:30 AM - 06:30 PM (Mon-Sat)', N'Silk & E-Commerce Hub, ITPS, EMS, Air Parcel'),
(6, N'DNK-UP-01', N'Moradabad Head Post Office DNK', N'Civil Lines, Near Company Bagh', N'Moradabad', N'Uttar Pradesh', N'244001', N'Mohd. Tariq (Export Facilitator)', N'+91 591 241 1234', N'dnk.moradabad@indiapost.gov.in', 28.8386, 78.7733, 1, 1, 1, N'09:00 AM - 05:30 PM (Mon-Sat)', N'Metal Art & Brassware Direct Export Corridor, Air Parcel, EMS'),
(7, N'DNK-JK-01', N'Srinagar GPO Dak Ghar Niryat Kendra', N'The Bund, Residency Road', N'Srinagar', N'Jammu and Kashmir', N'190001', N'Farooq Ahmad Mir (Nodal Officer)', N'+91 194 245 2820', N'dnk.srinagar@indiapost.gov.in', 34.0754, 74.8142, 1, 1, 1, N'09:30 AM - 05:00 PM (Mon-Sat)', N'Pashmina & Carpet GI Export Facilitation, EMS, Air Parcel'),
(8, N'DNK-CHN-01', N'Chennai GPO Dak Ghar Niryat Kendra', N'Rajaji Salai, George Town', N'Chennai', N'Tamil Nadu', N'600001', N'S. Soundararajan', N'+91 44 2534 1289', N'dnk.chennaigpo@indiapost.gov.in', 13.0878, 80.2885, 1, 1, 1, N'08:30 AM - 06:30 PM (Mon-Sat)', N'Leather & Handloom Center, EMS, ITPS, Air Parcel');

SET IDENTITY_INSERT [Master].[DnkCenters] OFF;

-- 2. Seed Identity.Users
SET IDENTITY_INSERT [Identity].[Users] ON;

INSERT INTO [Identity].[Users] ([Id], [Username], [FullName], [Email], [PhoneNumber], [Role], [BusinessName], [IecNumber], [Gstin], [AdCode], [LutNumber], [Address], [City], [State], [Pincode], [IsVerified], [AssignedDnkCenterId], [Designation])
VALUES
(1, N'exporter1', N'Vikram Aditya (Jaipur Heritage Crafts)', N'exports@jaipurheritagecrafts.in', N'+91 98290 12345', 1, N'Jaipur Heritage Crafts & Textiles Pvt. Ltd.', N'0518920192', N'08AABCT1334M1Z4', N'0293841002', N'AD080326001923K', N'Plot 42, Sitapura Industrial Area, Tonk Road', N'Jaipur', N'Rajasthan', N'302022', 1, NULL, NULL),
(2, N'exporter2', N'Ananya Nair (Malabar Spice Roots)', N'contact@malabarspiceroots.com', N'+91 94470 98765', 1, N'Malabar Spice Roots Exporters LLP', N'0920183921', N'32AABCM9921D1ZB', N'0482910394', N'AD320126004819M', N'Building 12, Spice Park, Puthencruz', N'Kochi', N'Kerala', N'682310', 1, NULL, NULL),
(3, N'exporter3', N'Bashir Ahmad (Kashmir Silk & Pashmina)', N'bashir@kashmirpashminaart.com', N'+91 97970 45678', 1, N'Kashmir Royale Pashmina Guild', N'0719284729', N'01AABCK4821N1Z0', N'0194820193', N'AD010226008129L', N'Near Jama Masjid, Nowhatta', N'Srinagar', N'Jammu and Kashmir', N'190002', 1, NULL, NULL),
(4, N'postal_staff', N'Ramesh Sharma', N'ramesh.sharma@indiapost.gov.in', N'+91 98110 54321', 2, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, N'Senior Postal Assistant / DNK Nodal Officer'),
(5, N'customs_officer', N'Vikramaditya Rathore, IRS', N'v.rathore.customs@gov.in', N'+91 99100 87654', 3, NULL, NULL, NULL, NULL, NULL, N'Delhi Foreign Post Office, Kotla Road', N'New Delhi', N'Delhi', N'110002', 1, NULL, N'Superintendent of Customs (Postal Appraising)'),
(6, N'admin', N'Director (International Mail & Exports)', N'admin.dnk@indiapost.gov.in', N'+91 11 2309 6000', 4, NULL, NULL, NULL, NULL, NULL, N'Dak Bhawan, Sansad Marg', N'New Delhi', N'Delhi', N'110001', 1, NULL, N'Directorate of Posts, International Mail Division');

SET IDENTITY_INSERT [Identity].[Users] OFF;

-- 3. Seed Master.HsCodeDirectories
SET IDENTITY_INSERT [Master].[HsCodeDirectories] ON;

INSERT INTO [Master].[HsCodeDirectories] ([Id], [HsCode], [Description], [Category], [Chapter], [StandardGstPercent], [ExportDutyPercent], [RodtepRatePercent], [IsRestricted], [RequiredDocuments])
VALUES
(1, N'6204.42', N'Women''s or girls'' dresses of cotton, handloom block printed', N'Textiles & Garments', N'Chapter 62', 5.00, 0.00, 2.50, 0, N'Commercial Invoice, Packing List, Self Declaration Form'),
(2, N'7113.11', N'Silver jewellery, whether or not plated or clad with other precious metal', N'Gems & Jewellery', N'Chapter 71', 3.00, 0.00, 1.00, 0, N'Hallmarking Certificate, Valuation Certificate, GJEPC Registration'),
(3, N'0902.30', N'Black tea (fermented) and partly fermented tea in immediate packings <= 3 kg', N'Agricultural & Tea', N'Chapter 09', 5.00, 0.00, 1.70, 0, N'Tea Board Non-preferential Certificate, Phytosanitary Certificate'),
(4, N'0908.31', N'Cardamom, neither crushed nor ground (Malabar / Alleppey Green)', N'Spices', N'Chapter 09', 5.00, 0.00, 2.00, 0, N'Spices Board Certificate of Registration, Phytosanitary Certificate'),
(5, N'7419.80', N'Handicrafts of brass, ornamental artware, vases, statues, candle holders', N'Handicrafts & Metalware', N'Chapter 74', 12.00, 0.00, 1.80, 0, N'Commercial Invoice, EPCH Certificate of Origin'),
(6, N'5701.10', N'Carpets and other textile floor coverings, knotted, of wool or fine animal hair', N'Handicrafts & Carpets', N'Chapter 57', 12.00, 0.00, 3.10, 0, N'CEPC Registration, GI Certification (if Kashmiri)'),
(7, N'3304.99', N'Ayurvedic herbal cosmetic preparations for the care of the skin (creams/lotions)', N'Ayush & Cosmetics', N'Chapter 33', 18.00, 0.00, 1.20, 0, N'Ayush Drug Manufacturing License, Certificate of Analysis (COA)'),
(8, N'4202.21', N'Handbags with outer surface of leather, handcrafted', N'Leather Goods', N'Chapter 42', 18.00, 0.00, 2.40, 0, N'CLE Registration, Non-CITES Animal Leather Declaration'),
(9, N'9503.00', N'Traditional handcrafted non-mechanized wooden toys (Channapatna / Kondapalli)', N'Toys & Crafts', N'Chapter 95', 12.00, 0.00, 1.50, 0, N'Non-toxic vegetable lacquer test report');

SET IDENTITY_INSERT [Master].[HsCodeDirectories] OFF;

PRINT 'DNK Enterprise SQL Server Database Seeded Successfully!';
GO
