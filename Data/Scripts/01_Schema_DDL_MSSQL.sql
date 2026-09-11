-- ====================================================================================
-- DAK GHAR NIRYAT KENDRA (DNK) - ENTERPRISE DATABASE SCHEMA DEFINITION (T-SQL / MSSQL)
-- Department of Posts, Ministry of Communications, Government of India
-- Version: 2.0 (Production Enterprise Architecture)
-- ====================================================================================

USE [master];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DakGharNiryatKendraDB')
BEGIN
    CREATE DATABASE [DakGharNiryatKendraDB]
    COLLATE Latin1_General_100_CI_AS_SC_UTF8;
END
GO

USE [DakGharNiryatKendraDB];
GO

-- 1. Create Dedicated Enterprise Schemas
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Identity')
    EXEC('CREATE SCHEMA [Identity]');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Master')
    EXEC('CREATE SCHEMA [Master]');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Export')
    EXEC('CREATE SCHEMA [Export]');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Postal')
    EXEC('CREATE SCHEMA [Postal]');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Tracking')
    EXEC('CREATE SCHEMA [Tracking]');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'Audit')
    EXEC('CREATE SCHEMA [Audit]');
GO

-- ====================================================================================
-- 2. SCHEMA: Master - Reference Tables
-- ====================================================================================

-- Table: Master.DnkCenters
IF OBJECT_ID(N'[Master].[DnkCenters]', N'U') IS NULL
BEGIN
    CREATE TABLE [Master].[DnkCenters] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [CenterCode] NVARCHAR(20) NOT NULL,
        [CenterName] NVARCHAR(150) NOT NULL,
        [Address] NVARCHAR(500) NOT NULL,
        [City] NVARCHAR(100) NOT NULL,
        [State] NVARCHAR(100) NOT NULL,
        [Pincode] NVARCHAR(10) NOT NULL,
        [ContactPerson] NVARCHAR(100) NULL,
        [Phone] NVARCHAR(20) NULL,
        [Email] NVARCHAR(100) NULL,
        [Latitude] FLOAT NOT NULL DEFAULT (0.0),
        [Longitude] FLOAT NOT NULL DEFAULT (0.0),
        [IsActive] BIT NOT NULL DEFAULT (1),
        [HasCustomsDesk] BIT NOT NULL DEFAULT (1),
        [HasPackagingFacility] BIT NOT NULL DEFAULT (1),
        [OperatingHours] NVARCHAR(100) NOT NULL DEFAULT ('09:00 AM - 05:30 PM (Mon-Sat)'),
        [ServicesOffered] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Master_DnkCenters] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Master_DnkCenters_CenterCode] UNIQUE NONCLUSTERED ([CenterCode] ASC)
    );
END
GO

-- Table: Master.HsCodeDirectories
IF OBJECT_ID(N'[Master].[HsCodeDirectories]', N'U') IS NULL
BEGIN
    CREATE TABLE [Master].[HsCodeDirectories] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [HsCode] NVARCHAR(12) NOT NULL,
        [Description] NVARCHAR(250) NOT NULL,
        [Category] NVARCHAR(100) NOT NULL,
        [Chapter] NVARCHAR(10) NULL,
        [StandardGstPercent] DECIMAL(5,2) NOT NULL DEFAULT (5.00),
        [ExportDutyPercent] DECIMAL(5,2) NOT NULL DEFAULT (0.00),
        [RodtepRatePercent] DECIMAL(5,2) NOT NULL DEFAULT (1.50),
        [IsRestricted] BIT NOT NULL DEFAULT (0),
        [RestrictionRemarks] NVARCHAR(500) NULL,
        [RequiredDocuments] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Master_HsCodeDirectories] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE NONCLUSTERED INDEX [IX_Master_HsCodeDirectories_HsCode] ON [Master].[HsCodeDirectories]([HsCode] ASC);
    CREATE NONCLUSTERED INDEX [IX_Master_HsCodeDirectories_Category] ON [Master].[HsCodeDirectories]([Category] ASC);
END
GO

-- Table: Master.TariffZones
IF OBJECT_ID(N'[Master].[TariffZones]', N'U') IS NULL
BEGIN
    CREATE TABLE [Master].[TariffZones] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ServiceType] INT NOT NULL, -- Enum: 1=EMS, 2=AirParcel, 3=ITPS, 4=RegisteredAirMail
        [CountryCode] NVARCHAR(5) NOT NULL,
        [CountryName] NVARCHAR(100) NOT NULL,
        [PostalZone] NVARCHAR(50) NOT NULL,
        [BaseWeightGrams] INT NOT NULL DEFAULT (250),
        [BaseRateINR] DECIMAL(18,2) NOT NULL DEFAULT (650.00),
        [StepWeightGrams] INT NOT NULL DEFAULT (250),
        [StepRateINR] DECIMAL(18,2) NOT NULL DEFAULT (150.00),
        [MinDeliveryDays] INT NOT NULL DEFAULT (4),
        [MaxDeliveryDays] INT NOT NULL DEFAULT (8),
        [MaxAllowedWeightKg] DECIMAL(8,2) NOT NULL DEFAULT (30.00),
        [FuelSurchargePercent] DECIMAL(5,2) NOT NULL DEFAULT (10.00),
        [GstPercent] DECIMAL(5,2) NOT NULL DEFAULT (18.00),
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Master_TariffZones] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE NONCLUSTERED INDEX [IX_Master_TariffZones_Country_Service] ON [Master].[TariffZones]([CountryCode] ASC, [ServiceType] ASC);
END
GO

-- ====================================================================================
-- 3. SCHEMA: Identity - Users & Stakeholder Profiles
-- ====================================================================================

IF OBJECT_ID(N'[Identity].[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [Identity].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Username] NVARCHAR(50) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [PhoneNumber] NVARCHAR(20) NOT NULL,
        [Role] INT NOT NULL DEFAULT (1), -- 1=Exporter, 2=PostalStaff, 3=CustomsOfficer, 4=Admin
        [BusinessName] NVARCHAR(150) NULL,
        [IecNumber] NVARCHAR(10) NULL,
        [Gstin] NVARCHAR(15) NULL,
        [AdCode] NVARCHAR(20) NULL,
        [LutNumber] NVARCHAR(30) NULL,
        [Address] NVARCHAR(500) NULL,
        [City] NVARCHAR(100) NULL,
        [State] NVARCHAR(100) NULL,
        [Pincode] NVARCHAR(10) NULL,
        [IsVerified] BIT NOT NULL DEFAULT (1),
        [AssignedDnkCenterId] INT NULL,
        [Designation] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Identity_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Identity_Users_Username] UNIQUE NONCLUSTERED ([Username] ASC),
        CONSTRAINT [FK_Identity_Users_DnkCenters_AssignedDnkCenterId] FOREIGN KEY ([AssignedDnkCenterId]) REFERENCES [Master].[DnkCenters]([Id]) ON DELETE SET NULL
    );
    CREATE NONCLUSTERED INDEX [IX_Identity_Users_IecNumber] ON [Identity].[Users]([IecNumber] ASC) WHERE [IecNumber] IS NOT NULL;
END
GO

-- ====================================================================================
-- 4. SCHEMA: Postal - Dispatch Manifests
-- ====================================================================================

IF OBJECT_ID(N'[Postal].[DispatchManifests]', N'U') IS NULL
BEGIN
    CREATE TABLE [Postal].[DispatchManifests] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ManifestNumber] NVARCHAR(30) NOT NULL,
        [OriginDnkCenterId] INT NOT NULL,
        [DestinationFpo] NVARCHAR(150) NOT NULL DEFAULT ('Delhi Foreign Post Office (FPO Kotla Road)'),
        [DispatchDate] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
        [BagBarcodeNumber] NVARCHAR(50) NOT NULL,
        [SealNumber] NVARCHAR(50) NOT NULL,
        [TotalArticles] INT NOT NULL DEFAULT (0),
        [TotalWeightKg] DECIMAL(10,3) NOT NULL DEFAULT (0.000),
        [CreatedByStaff] NVARCHAR(100) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT ('Dispatched'),
        CONSTRAINT [PK_Postal_DispatchManifests] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Postal_DispatchManifests_ManifestNumber] UNIQUE NONCLUSTERED ([ManifestNumber] ASC),
        CONSTRAINT [FK_Postal_DispatchManifests_DnkCenters_OriginDnkCenterId] FOREIGN KEY ([OriginDnkCenterId]) REFERENCES [Master].[DnkCenters]([Id]) ON DELETE RESTRICT
    );
END
GO

-- ====================================================================================
-- 5. SCHEMA: Export - Consignments & Line Items
-- ====================================================================================

IF OBJECT_ID(N'[Export].[ExportConsignments]', N'U') IS NULL
BEGIN
    CREATE TABLE [Export].[ExportConsignments] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [TrackingNumber] NVARCHAR(20) NOT NULL, -- S10 Format: EM892104928IN
        [PbeNumber] NVARCHAR(35) NOT NULL,      -- PBE-I/2026/DEL/000204
        [PbeType] INT NOT NULL DEFAULT (1),      -- 1=PBE_I, 2=PBE_II
        [ServiceType] INT NOT NULL DEFAULT (1),  -- 1=EMS, 2=AirParcel, 3=ITPS, 4=RegisteredAirMail
        [Status] INT NOT NULL DEFAULT (1),       -- Enum: ConsignmentStatus
        [CustomsStatus] INT NOT NULL DEFAULT (1),-- Enum: CustomsStatus
        
        -- Parties
        [ExporterId] INT NOT NULL,
        [DnkCenterId] INT NOT NULL,
        
        -- Recipient (Consignee)
        [RecipientName] NVARCHAR(150) NOT NULL,
        [RecipientCompany] NVARCHAR(150) NULL,
        [RecipientEmail] NVARCHAR(100) NOT NULL,
        [RecipientPhone] NVARCHAR(25) NOT NULL,
        [RecipientAddress] NVARCHAR(500) NOT NULL,
        [RecipientCity] NVARCHAR(100) NOT NULL,
        [RecipientState] NVARCHAR(100) NULL,
        [RecipientPostalCode] NVARCHAR(20) NOT NULL,
        [RecipientCountry] NVARCHAR(100) NOT NULL,
        [DestinationCountryCode] NVARCHAR(5) NOT NULL,
        
        -- Physical & Volumetric Weight
        [ActualWeightKg] DECIMAL(10,3) NOT NULL DEFAULT (0.500),
        [LengthCm] DECIMAL(8,2) NOT NULL DEFAULT (20.00),
        [WidthCm] DECIMAL(8,2) NOT NULL DEFAULT (15.00),
        [HeightCm] DECIMAL(8,2) NOT NULL DEFAULT (10.00),
        [VolumetricWeightKg] DECIMAL(10,3) NOT NULL DEFAULT (0.600),
        [ChargeableWeightKg] DECIMAL(10,3) NOT NULL DEFAULT (0.600),
        [VerifiedWeightKg] DECIMAL(10,3) NULL,
        
        -- Customs & Invoicing
        [InvoiceNumber] NVARCHAR(50) NULL,
        [InvoiceDate] DATETIME2(7) NULL,
        [LutNumber] NVARCHAR(30) NULL,
        [NatureOfExport] NVARCHAR(50) NOT NULL DEFAULT ('Sale of Goods (Commercial)'),
        [CurrencyCode] NVARCHAR(10) NOT NULL DEFAULT ('INR'),
        [DeclaredValueForeign] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        [ExchangeRateToINR] DECIMAL(10,4) NOT NULL DEFAULT (1.0000),
        [DeclaredValueINR] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        
        -- Pricing Breakdown
        [PostageAmountINR] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        [FuelSurchargeINR] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        [InsuranceAmountINR] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        [GstAmountINR] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        [TotalShippingCostINR] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        
        -- Customs Clearance
        [CustomsAssessedDutyINR] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        [CustomsOfficerRemarks] NVARCHAR(1000) NULL,
        [CustomsInspectionLog] NVARCHAR(1000) NULL,
        [ScrutinyOfficerName] NVARCHAR(100) NULL,
        [CustomsClearedAt] DATETIME2(7) NULL,
        
        -- Induction & Bagging
        [InductedAt] DATETIME2(7) NULL,
        [InductedByStaffName] NVARCHAR(100) NULL,
        [DispatchManifestId] INT NULL,
        
        -- Doorstep Pickup
        [IsPickupRequested] BIT NOT NULL DEFAULT (0),
        [PickupStatus] INT NOT NULL DEFAULT (0),
        [ScheduledPickupDate] DATETIME2(7) NULL,
        [PickupTimeSlot] INT NULL,
        
        -- Timestamps
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
        [DeliveredAt] DATETIME2(7) NULL,
        
        CONSTRAINT [PK_Export_ExportConsignments] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_Export_ExportConsignments_TrackingNumber] UNIQUE NONCLUSTERED ([TrackingNumber] ASC),
        CONSTRAINT [FK_Export_ExportConsignments_Users_ExporterId] FOREIGN KEY ([ExporterId]) REFERENCES [Identity].[Users]([Id]) ON DELETE RESTRICT,
        CONSTRAINT [FK_Export_ExportConsignments_DnkCenters_DnkCenterId] FOREIGN KEY ([DnkCenterId]) REFERENCES [Master].[DnkCenters]([Id]) ON DELETE RESTRICT,
        CONSTRAINT [FK_Export_ExportConsignments_DispatchManifests_DispatchManifestId] FOREIGN KEY ([DispatchManifestId]) REFERENCES [Postal].[DispatchManifests]([Id]) ON DELETE SET NULL
    );

    CREATE NONCLUSTERED INDEX [IX_Export_ExportConsignments_PbeNumber] ON [Export].[ExportConsignments]([PbeNumber] ASC);
    CREATE NONCLUSTERED INDEX [IX_Export_ExportConsignments_Exporter_Status] ON [Export].[ExportConsignments]([ExporterId] ASC, [Status] ASC);
    CREATE NONCLUSTERED INDEX [IX_Export_ExportConsignments_CustomsStatus] ON [Export].[ExportConsignments]([CustomsStatus] ASC);
    CREATE NONCLUSTERED INDEX [IX_Export_ExportConsignments_Destination] ON [Export].[ExportConsignments]([DestinationCountryCode] ASC, [Status] ASC);
END
GO

-- Table: Export.ConsignmentItems
IF OBJECT_ID(N'[Export].[ConsignmentItems]', N'U') IS NULL
BEGIN
    CREATE TABLE [Export].[ConsignmentItems] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ExportConsignmentId] INT NOT NULL,
        [ItemDescription] NVARCHAR(250) NOT NULL,
        [HsCode] NVARCHAR(12) NOT NULL,
        [Quantity] INT NOT NULL DEFAULT (1),
        [UnitOfMeasurement] NVARCHAR(20) NOT NULL DEFAULT ('PCS'),
        [UnitPriceINR] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        [TotalPriceINR] DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        [NetWeightKg] DECIMAL(10,3) NOT NULL DEFAULT (0.500),
        [CountryOfOrigin] NVARCHAR(100) NOT NULL DEFAULT ('India'),
        CONSTRAINT [PK_Export_ConsignmentItems] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Export_ConsignmentItems_ExportConsignments_ExportConsignmentId] FOREIGN KEY ([ExportConsignmentId]) REFERENCES [Export].[ExportConsignments]([Id]) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_Export_ConsignmentItems_ConsignmentId] ON [Export].[ConsignmentItems]([ExportConsignmentId] ASC);
    CREATE NONCLUSTERED INDEX [IX_Export_ConsignmentItems_HsCode] ON [Export].[ConsignmentItems]([HsCode] ASC);
END
GO

-- Table: Export.PickupRequests
IF OBJECT_ID(N'[Export].[PickupRequests]', N'U') IS NULL
BEGIN
    CREATE TABLE [Export].[PickupRequests] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ExportConsignmentId] INT NOT NULL,
        [ExporterId] INT NOT NULL,
        [ScheduledDate] DATETIME2(7) NOT NULL,
        [TimeSlot] INT NOT NULL DEFAULT (1),
        [PickupAddress] NVARCHAR(500) NOT NULL,
        [Pincode] NVARCHAR(10) NOT NULL,
        [Status] INT NOT NULL DEFAULT (1),
        [AssignedPostmanName] NVARCHAR(100) NULL,
        [AssignedPostmanPhone] NVARCHAR(20) NULL,
        [SpecialInstructions] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Export_PickupRequests] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Export_PickupRequests_ExportConsignments_ExportConsignmentId] FOREIGN KEY ([ExportConsignmentId]) REFERENCES [Export].[ExportConsignments]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Export_PickupRequests_Users_ExporterId] FOREIGN KEY ([ExporterId]) REFERENCES [Identity].[Users]([Id]) ON DELETE RESTRICT
    );
END
GO

-- ====================================================================================
-- 6. SCHEMA: Tracking - Milestone Events
-- ====================================================================================

IF OBJECT_ID(N'[Tracking].[TrackingMilestones]', N'U') IS NULL
BEGIN
    CREATE TABLE [Tracking].[TrackingMilestones] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ExportConsignmentId] INT NOT NULL,
        [TrackingNumber] NVARCHAR(20) NOT NULL,
        [EventTime] DATETIME2(7) NOT NULL DEFAULT (SYSUTCDATETIME()),
        [FacilityLocation] NVARCHAR(150) NOT NULL,
        [StatusHeadline] NVARCHAR(100) NOT NULL,
        [StatusCategory] INT NOT NULL,
        [DetailedDescription] NVARCHAR(1000) NULL,
        [PerformedBy] NVARCHAR(100) NULL,
        [NextExpectedAction] NVARCHAR(500) NULL,
        CONSTRAINT [PK_Tracking_TrackingMilestones] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Tracking_TrackingMilestones_ExportConsignments_ExportConsignmentId] FOREIGN KEY ([ExportConsignmentId]) REFERENCES [Export].[ExportConsignments]([Id]) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_Tracking_TrackingMilestones_TrackingNumber_EventTime] ON [Tracking].[TrackingMilestones]([TrackingNumber] ASC, [EventTime] DESC);
END
GO
