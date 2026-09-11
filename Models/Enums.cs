namespace DakGharNiryatKendra.Models
{
    public enum UserRole
    {
        Exporter = 1,
        PostalStaff = 2,
        CustomsOfficer = 3,
        Admin = 4
    }

    public enum ServiceType
    {
        EMS_SpeedPost = 1,          // International EMS
        InternationalAirParcel = 2, // Air Parcel (up to 20kg/30kg)
        ITPS_TrackedPacket = 3,     // International Tracked Packet (for e-commerce < 2kg)
        RegisteredAirMail = 4       // Small Packets / Documents
    }

    public enum PbeType
    {
        PBE_I = 1,  // E-Commerce Exports (Postal Bill of Export I)
        PBE_II = 2  // Commercial / Non-eCommerce Exports (Postal Bill of Export II)
    }

    public enum ConsignmentStatus
    {
        Draft = 0,
        Booked = 1,
        PickupScheduled = 2,
        PickedUp = 3,
        InductedAtDNK = 4,
        BaggedAndManifested = 5,
        DispatchedToFPO = 6,
        ReceivedAtFPO = 7,
        CustomsHold = 8,
        CustomsCleared = 9,
        OutwardInternationalHub = 10,
        DepartedOriginCountry = 11,
        ArrivedDestinationHub = 12,
        CustomsAtDestination = 13,
        OutForDelivery = 14,
        Delivered = 15,
        ReturnedToOrigin = 16
    }

    public enum CustomsStatus
    {
        PendingVerification = 1,
        Approved = 2,
        HoldForQuery = 3,
        PhysicalInspectionRequired = 4,
        DutyAssessed = 5,
        Exempted = 6
    }

    public enum PickupStatus
    {
        NotRequested = 0,
        Requested = 1,
        Assigned = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum TimeSlot
    {
        Morning_09_12 = 1,
        Afternoon_12_16 = 2,
        Evening_16_19 = 3
    }
}
