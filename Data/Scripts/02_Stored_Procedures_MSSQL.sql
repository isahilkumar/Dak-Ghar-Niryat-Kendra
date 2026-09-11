-- ====================================================================================
-- DAK GHAR NIRYAT KENDRA (DNK) - ENTERPRISE STORED PROCEDURES (T-SQL / MSSQL)
-- Department of Posts, Ministry of Communications, Government of India
-- ====================================================================================

USE [DakGharNiryatKendraDB];
GO

-- ====================================================================================
-- 1. PROCEDURE: sp_InductArticleAtCounter
-- Atomically inducts an article, updates gross weight, sets status, and creates milestone
-- ====================================================================================
CREATE OR ALTER PROCEDURE [Postal].[sp_InductArticleAtCounter]
    @TrackingNumber NVARCHAR(20),
    @VerifiedWeightKg DECIMAL(10,3),
    @StaffName NVARCHAR(100),
    @ResultCode INT OUTPUT,
    @ResultMessage NVARCHAR(250) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @ConsignmentId INT;
        DECLARE @DnkCenterName NVARCHAR(150);

        SELECT @ConsignmentId = c.[Id], @DnkCenterName = d.[CenterName]
        FROM [Export].[ExportConsignments] c
        INNER JOIN [Master].[DnkCenters] d ON c.[DnkCenterId] = d.[Id]
        WHERE c.[TrackingNumber] = @TrackingNumber;

        IF @ConsignmentId IS NULL
        BEGIN
            SET @ResultCode = 404;
            SET @ResultMessage = N'Article Tracking Number not found in DNK network.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Update Consignment status
        UPDATE [Export].[ExportConsignments]
        SET [Status] = 4, -- InductedAtDNK
            [InductedAt] = SYSUTCDATETIME(),
            [InductedByStaffName] = @StaffName,
            [VerifiedWeightKg] = @VerifiedWeightKg
        WHERE [Id] = @ConsignmentId;

        -- Insert Tracking Milestone
        INSERT INTO [Tracking].[TrackingMilestones] (
            [ExportConsignmentId],
            [TrackingNumber],
            [EventTime],
            [FacilityLocation],
            [StatusHeadline],
            [StatusCategory],
            [DetailedDescription],
            [PerformedBy]
        ) VALUES (
            @ConsignmentId,
            @TrackingNumber,
            SYSUTCDATETIME(),
            @DnkCenterName,
            N'Article Inducted at DNK Counter',
            4, -- InductedAtDNK
            CONCAT(N'Article physically received and verified. Gross Weight: ', CAST(@VerifiedWeightKg AS NVARCHAR(20)), N' Kg. Prepared for dispatch to Office of Exchange.'),
            @StaffName
        );

        COMMIT TRANSACTION;
        SET @ResultCode = 200;
        SET @ResultMessage = N'Article successfully inducted and verified.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @ResultCode = 500;
        SET @ResultMessage = ERROR_MESSAGE();
    END CATCH
END;
GO

-- ====================================================================================
-- 2. PROCEDURE: sp_GrantCustomsOutOfCharge
-- Approves e-PBE under Section 84 Customs Act 1962 and grants Out of Charge (OOC)
-- ====================================================================================
CREATE OR ALTER PROCEDURE [Export].[sp_GrantCustomsOutOfCharge]
    @TrackingNumber NVARCHAR(20),
    @OfficerName NVARCHAR(100),
    @Remarks NVARCHAR(500) = NULL,
    @ResultCode INT OUTPUT,
    @ResultMessage NVARCHAR(250) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @ConsignmentId INT;
        SELECT @ConsignmentId = [Id]
        FROM [Export].[ExportConsignments]
        WHERE [TrackingNumber] = @TrackingNumber;

        IF @ConsignmentId IS NULL
        BEGIN
            SET @ResultCode = 404;
            SET @ResultMessage = N'Consignment not found.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF @Remarks IS NULL OR @Remarks = ''
            SET @Remarks = N'Electronic PBE verified under Section 84 of Customs Act 1962. Out of Charge (OOC) granted.';

        -- Update Customs & Consignment Status
        UPDATE [Export].[ExportConsignments]
        SET [CustomsStatus] = 2, -- Approved
            [Status] = 9,        -- CustomsCleared
            [ScrutinyOfficerName] = @OfficerName,
            [CustomsOfficerRemarks] = @Remarks,
            [CustomsClearedAt] = SYSUTCDATETIME()
        WHERE [Id] = @ConsignmentId;

        -- Add Customs Milestone
        INSERT INTO [Tracking].[TrackingMilestones] (
            [ExportConsignmentId],
            [TrackingNumber],
            [EventTime],
            [FacilityLocation],
            [StatusHeadline],
            [StatusCategory],
            [DetailedDescription],
            [PerformedBy]
        ) VALUES (
            @ConsignmentId,
            @TrackingNumber,
            SYSUTCDATETIME(),
            N'Delhi Foreign Post Office (FPO) Customs Appraising Wing',
            N'Customs Out of Charge (OOC) Granted',
            9, -- CustomsCleared
            @Remarks,
            @OfficerName
        );

        COMMIT TRANSACTION;
        SET @ResultCode = 200;
        SET @ResultMessage = N'Customs Out of Charge (OOC) successfully granted.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @ResultCode = 500;
        SET @ResultMessage = ERROR_MESSAGE();
    END CATCH
END;
GO

-- ====================================================================================
-- 3. PROCEDURE: sp_GenerateDispatchManifest
-- Generates secure container bag and links inducted articles to FPO manifest
-- ====================================================================================
CREATE OR ALTER PROCEDURE [Postal].[sp_GenerateDispatchManifest]
    @OriginDnkCenterId INT,
    @DestinationFpo NVARCHAR(150),
    @CreatedByStaff NVARCHAR(100),
    @ConsignmentIdsCsv NVARCHAR(MAX),
    @ManifestNumber NVARCHAR(30) OUTPUT,
    @BagBarcode NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @CenterCode NVARCHAR(20);
        DECLARE @CenterName NVARCHAR(150);

        SELECT @CenterCode = [CenterCode], @CenterName = [CenterName]
        FROM [Master].[DnkCenters]
        WHERE [Id] = @OriginDnkCenterId;

        DECLARE @CenterSlug NVARCHAR(20) = REPLACE(@CenterCode, N'DNK-', N'');
        DECLARE @DateStr NVARCHAR(8) = FORMAT(SYSUTCDATETIME(), 'yyyyMMdd');
        DECLARE @RandSeq INT = ABS(CHECKSUM(NEWID())) % 90 + 10;

        SET @ManifestNumber = CONCAT(N'MNF/', @CenterSlug, N'/', @DateStr, N'/', @RandSeq);
        SET @BagBarcode = CONCAT(N'BAG-IND-', @CenterSlug, N'-', ABS(CHECKSUM(NEWID())) % 900000 + 100000);
        DECLARE @SealNumber NVARCHAR(50) = CONCAT(N'SL-', @CenterSlug, N'-', ABS(CHECKSUM(NEWID())) % 900000 + 100000);

        -- Parse CSV ids into table variable
        DECLARE @IdTable TABLE ([Id] INT);
        INSERT INTO @IdTable ([Id])
        SELECT CAST([value] AS INT) FROM STRING_SPLIT(@ConsignmentIdsCsv, ',');

        DECLARE @TotalArticles INT = (SELECT COUNT(*) FROM @IdTable);
        DECLARE @TotalWeight DECIMAL(10,3) = (
            SELECT ISNULL(SUM(ISNULL([VerifiedWeightKg], [ActualWeightKg])), 0)
            FROM [Export].[ExportConsignments]
            WHERE [Id] IN (SELECT [Id] FROM @IdTable)
        );

        -- Insert Manifest
        INSERT INTO [Postal].[DispatchManifests] (
            [ManifestNumber],
            [OriginDnkCenterId],
            [DestinationFpo],
            [DispatchDate],
            [BagBarcodeNumber],
            [SealNumber],
            [TotalArticles],
            [TotalWeightKg],
            [CreatedByStaff],
            [Status]
        ) VALUES (
            @ManifestNumber,
            @OriginDnkCenterId,
            @DestinationFpo,
            SYSUTCDATETIME(),
            @BagBarcode,
            @SealNumber,
            @TotalArticles,
            @TotalWeight,
            @CreatedByStaff,
            N'Dispatched'
        );

        DECLARE @ManifestId INT = SCOPE_IDENTITY();

        -- Update Consignments
        UPDATE [Export].[ExportConsignments]
        SET [DispatchManifestId] = @ManifestId,
            [Status] = 5 -- BaggedAndManifested
        WHERE [Id] IN (SELECT [Id] FROM @IdTable);

        -- Insert Milestones for each consignment
        INSERT INTO [Tracking].[TrackingMilestones] (
            [ExportConsignmentId],
            [TrackingNumber],
            [EventTime],
            [FacilityLocation],
            [StatusHeadline],
            [StatusCategory],
            [DetailedDescription],
            [PerformedBy]
        )
        SELECT 
            c.[Id],
            c.[TrackingNumber],
            SYSUTCDATETIME(),
            @CenterName,
            N'Bagged and Manifested for FPO Transit',
            5, -- BaggedAndManifested
            CONCAT(N'Enclosed in secured mail bag ', @BagBarcode, N' on Manifest ', @ManifestNumber, N' to ', @DestinationFpo),
            @CreatedByStaff
        FROM [Export].[ExportConsignments] c
        WHERE c.[Id] IN (SELECT [Id] FROM @IdTable);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- ====================================================================================
-- 4. PROCEDURE: sp_GetExportAnalyticsByCountry
-- Aggregates country-wise volumes and turnover using Common Table Expressions (CTE)
-- ====================================================================================
CREATE OR ALTER PROCEDURE [Export].[sp_GetExportAnalyticsByCountry]
    @StartDate DATETIME2(7) = NULL,
    @EndDate DATETIME2(7) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @StartDate IS NULL SET @StartDate = DATEADD(YEAR, -1, SYSUTCDATETIME());
    IF @EndDate IS NULL SET @EndDate = SYSUTCDATETIME();

    WITH CountrySummary AS (
        SELECT 
            c.[DestinationCountryCode],
            c.[RecipientCountry],
            COUNT(c.[Id]) AS [TotalConsignments],
            SUM(c.[DeclaredValueINR]) AS [TotalExportValueINR],
            SUM(c.[TotalShippingCostINR]) AS [TotalPostageRevenueINR],
            AVG(ISNULL(c.[VerifiedWeightKg], c.[ActualWeightKg])) AS [AvgWeightKg],
            SUM(CASE WHEN c.[Status] = 15 THEN 1 ELSE 0 END) AS [DeliveredCount]
        FROM [Export].[ExportConsignments] c
        WHERE c.[CreatedAt] BETWEEN @StartDate AND @EndDate
        GROUP BY c.[DestinationCountryCode], c.[RecipientCountry]
    )
    SELECT 
        [DestinationCountryCode],
        [RecipientCountry],
        [TotalConsignments],
        [TotalExportValueINR],
        [TotalPostageRevenueINR],
        CAST([AvgWeightKg] AS DECIMAL(10,2)) AS [AvgWeightKg],
        [DeliveredCount],
        CAST(([DeliveredCount] * 100.0 / NULLIF([TotalConsignments], 0)) AS DECIMAL(5,2)) AS [SuccessRatePercent]
    FROM CountrySummary
    ORDER BY [TotalExportValueINR] DESC;
END;
GO
