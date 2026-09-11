-- ====================================================================================
-- DAK GHAR NIRYAT KENDRA (DNK) - ENTERPRISE VIEWS AND FUNCTIONS (T-SQL / MSSQL)
-- Department of Posts, Ministry of Communications, Government of India
-- ====================================================================================

USE [DakGharNiryatKendraDB];
GO

-- ====================================================================================
-- 1. FUNCTION: fn_CalculateVolumetricWeight
-- Computes IATA/UPU postal volumetric weight in Kg: (L x W x H in cm) / 5000
-- ====================================================================================
CREATE OR ALTER FUNCTION [Master].[fn_CalculateVolumetricWeight](
    @LengthCm DECIMAL(8,2),
    @WidthCm DECIMAL(8,2),
    @HeightCm DECIMAL(8,2)
)
RETURNS DECIMAL(10,3)
WITH SCHEMABINDING
AS
BEGIN
    IF @LengthCm <= 0 OR @WidthCm <= 0 OR @HeightCm <= 0
        RETURN 0.000;

    RETURN CAST((@LengthCm * @WidthCm * @HeightCm) / 5000.0 AS DECIMAL(10,3));
END;
GO

-- ====================================================================================
-- 2. FUNCTION: fn_GetS10CheckDigit
-- Computes official Universal Postal Union (UPU) S10 9-digit modulus 11 checksum
-- ====================================================================================
CREATE OR ALTER FUNCTION [Master].[fn_GetS10CheckDigit](
    @EightDigitSerial NVARCHAR(8)
)
RETURNS INT
WITH SCHEMABINDING
AS
BEGIN
    IF LEN(@EightDigitSerial) <> 8
        RETURN 0;

    DECLARE @Weights TABLE (Pos INT, W INT);
    -- Standard UPU Weights: 8, 6, 4, 2, 3, 5, 9, 7
    DECLARE @Sum INT = 
        CAST(SUBSTRING(@EightDigitSerial, 1, 1) AS INT) * 8 +
        CAST(SUBSTRING(@EightDigitSerial, 2, 1) AS INT) * 6 +
        CAST(SUBSTRING(@EightDigitSerial, 3, 1) AS INT) * 4 +
        CAST(SUBSTRING(@EightDigitSerial, 4, 1) AS INT) * 2 +
        CAST(SUBSTRING(@EightDigitSerial, 5, 1) AS INT) * 3 +
        CAST(SUBSTRING(@EightDigitSerial, 6, 1) AS INT) * 5 +
        CAST(SUBSTRING(@EightDigitSerial, 7, 1) AS INT) * 9 +
        CAST(SUBSTRING(@EightDigitSerial, 8, 1) AS INT) * 7;

    DECLARE @Remainder INT = @Sum % 11;
    IF @Remainder = 0 RETURN 5;
    IF @Remainder = 1 RETURN 0;
    RETURN 11 - @Remainder;
END;
GO

-- ====================================================================================
-- 3. VIEW: vw_ActiveExportPipeline
-- Real-time unified pipeline view linking Exporters, Induction Centers & Transit
-- ====================================================================================
CREATE OR ALTER VIEW [Export].[vw_ActiveExportPipeline]
AS
SELECT 
    c.[Id] AS [ConsignmentId],
    c.[TrackingNumber],
    c.[PbeNumber],
    c.[PbeType],
    c.[ServiceType],
    CASE c.[ServiceType]
        WHEN 1 THEN N'International EMS (Speed Post)'
        WHEN 2 THEN N'International Air Parcel'
        WHEN 3 THEN N'ITPS (Tracked Packet)'
        WHEN 4 THEN N'Registered Air Mail'
        ELSE N'Other Postal Service'
    END AS [ServiceTypeName],
    c.[Status] AS [StatusCode],
    c.[CustomsStatus] AS [CustomsStatusCode],
    
    -- Exporter Details
    u.[Id] AS [ExporterId],
    u.[BusinessName] AS [ExporterBusinessName],
    u.[FullName] AS [ExporterContactPerson],
    u.[IecNumber] AS [ExporterIec],
    u.[Gstin] AS [ExporterGstin],
    u.[LutNumber] AS [ExporterLut],
    
    -- Induction Kendra
    d.[Id] AS [DnkCenterId],
    d.[CenterCode] AS [DnkCenterCode],
    d.[CenterName] AS [DnkCenterName],
    d.[City] AS [DnkCity],
    d.[State] AS [DnkState],
    
    -- Consignee & Transit
    c.[RecipientName],
    c.[RecipientCity],
    c.[RecipientCountry],
    c.[DestinationCountryCode],
    c.[ActualWeightKg],
    c.[VolumetricWeightKg],
    c.[ChargeableWeightKg],
    c.[DeclaredValueINR],
    c.[TotalShippingCostINR],
    c.[CreatedAt] AS [BookingTimestamp],
    c.[InductedAt],
    c.[CustomsClearedAt],
    c.[DeliveredAt]
FROM [Export].[ExportConsignments] c
INNER JOIN [Identity].[Users] u ON c.[ExporterId] = u.[Id]
INNER JOIN [Master].[DnkCenters] d ON c.[DnkCenterId] = d.[Id];
GO

-- ====================================================================================
-- 4. VIEW: vw_CustomsScrutinyRegistry
-- Customs official ledger for e-PBE appraisal, duty assessments, and OOC status
-- ====================================================================================
CREATE OR ALTER VIEW [Export].[vw_CustomsScrutinyRegistry]
AS
SELECT 
    c.[Id] AS [ConsignmentId],
    c.[PbeNumber],
    c.[TrackingNumber],
    c.[NatureOfExport],
    c.[InvoiceNumber],
    c.[InvoiceDate],
    c.[LutNumber],
    c.[DeclaredValueINR],
    c.[CurrencyCode],
    c.[DeclaredValueForeign],
    c.[CustomsStatus],
    c.[CustomsAssessedDutyINR],
    c.[CustomsOfficerRemarks],
    c.[ScrutinyOfficerName],
    c.[CustomsClearedAt],
    u.[BusinessName] AS [ExporterName],
    u.[IecNumber],
    u.[Gstin],
    d.[CenterName] AS [InductionKendra],
    COUNT(i.[Id]) AS [TotalLineItems],
    SUM(i.[NetWeightKg]) AS [TotalNetWeightKg]
FROM [Export].[ExportConsignments] c
INNER JOIN [Identity].[Users] u ON c.[ExporterId] = u.[Id]
INNER JOIN [Master].[DnkCenters] d ON c.[DnkCenterId] = d.[Id]
LEFT JOIN [Export].[ConsignmentItems] i ON c.[Id] = i.[ExportConsignmentId]
GROUP BY 
    c.[Id], c.[PbeNumber], c.[TrackingNumber], c.[NatureOfExport],
    c.[InvoiceNumber], c.[InvoiceDate], c.[LutNumber], c.[DeclaredValueINR],
    c.[CurrencyCode], c.[DeclaredValueForeign], c.[CustomsStatus],
    c.[CustomsAssessedDutyINR], c.[CustomsOfficerRemarks], c.[ScrutinyOfficerName],
    c.[CustomsClearedAt], u.[BusinessName], u.[IecNumber], u.[Gstin], d.[CenterName];
GO

-- ====================================================================================
-- 5. VIEW: vw_DnkBranchPerformance
-- Aggregated operational performance matrix per DNK branch
-- ====================================================================================
CREATE OR ALTER VIEW [Master].[vw_DnkBranchPerformance]
AS
SELECT 
    d.[Id] AS [DnkCenterId],
    d.[CenterCode],
    d.[CenterName],
    d.[City],
    d.[State],
    d.[Pincode],
    d.[OperatingHours],
    COUNT(c.[Id]) AS [TotalConsignmentsInducted],
    ISNULL(SUM(c.[DeclaredValueINR]), 0) AS [TotalExportTurnoverINR],
    ISNULL(SUM(c.[TotalShippingCostINR]), 0) AS [TotalPostageCollectedINR],
    ISNULL(SUM(CASE WHEN c.[Status] = 15 THEN 1 ELSE 0 END), 0) AS [TotalDeliveredAbroad],
    ISNULL(SUM(CASE WHEN c.[IsPickupRequested] = 1 THEN 1 ELSE 0 END), 0) AS [TotalDoorstepPickups]
FROM [Master].[DnkCenters] d
LEFT JOIN [Export].[ExportConsignments] c ON d.[Id] = c.[DnkCenterId]
GROUP BY 
    d.[Id], d.[CenterCode], d.[CenterName], d.[City], d.[State],
    d.[Pincode], d.[OperatingHours];
GO
