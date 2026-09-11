-- ====================================================================================
-- DAK GHAR NIRYAT KENDRA (DNK) - POSTGRESQL ENTERPRISE SCHEMA & CONSTRAINTS
-- Department of Posts, Ministry of Communications, Government of India
-- ====================================================================================

-- 1. Create Dedicated Schemas
CREATE SCHEMA IF NOT EXISTS master;
CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS export;
CREATE SCHEMA IF NOT EXISTS postal;
CREATE SCHEMA IF NOT EXISTS tracking;

-- 2. Master Tables
CREATE TABLE IF NOT EXISTS master.dnk_centers (
    id SERIAL PRIMARY KEY,
    center_code VARCHAR(20) NOT NULL UNIQUE,
    center_name VARCHAR(150) NOT NULL,
    address TEXT NOT NULL,
    city VARCHAR(100) NOT NULL,
    state VARCHAR(100) NOT NULL,
    pincode VARCHAR(10) NOT NULL,
    contact_person VARCHAR(100),
    phone VARCHAR(20),
    email VARCHAR(100),
    latitude DOUBLE PRECISION DEFAULT 0.0,
    longitude DOUBLE PRECISION DEFAULT 0.0,
    is_active BOOLEAN DEFAULT TRUE,
    has_customs_desk BOOLEAN DEFAULT TRUE,
    has_packaging_facility BOOLEAN DEFAULT TRUE,
    operating_hours VARCHAR(100) DEFAULT '09:00 AM - 05:30 PM (Mon-Sat)',
    services_offered TEXT,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS master.hs_code_directories (
    id SERIAL PRIMARY KEY,
    hs_code VARCHAR(12) NOT NULL,
    description VARCHAR(250) NOT NULL,
    category VARCHAR(100) NOT NULL,
    chapter VARCHAR(10),
    standard_gst_percent NUMERIC(5,2) DEFAULT 5.00,
    export_duty_percent NUMERIC(5,2) DEFAULT 0.00,
    rodtep_rate_percent NUMERIC(5,2) DEFAULT 1.50,
    is_restricted BOOLEAN DEFAULT FALSE,
    required_documents TEXT,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS idx_hs_code ON master.hs_code_directories(hs_code);

-- 3. Identity Users
CREATE TABLE IF NOT EXISTS identity.users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    phone_number VARCHAR(20) NOT NULL,
    role INT DEFAULT 1,
    business_name VARCHAR(150),
    iec_number VARCHAR(10),
    gstin VARCHAR(15),
    ad_code VARCHAR(20),
    lut_number VARCHAR(30),
    address TEXT,
    city VARCHAR(100),
    state VARCHAR(100),
    pincode VARCHAR(10),
    is_verified BOOLEAN DEFAULT TRUE,
    assigned_dnk_center_id INT REFERENCES master.dnk_centers(id) ON DELETE SET NULL,
    designation VARCHAR(100),
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- 4. Export Consignments
CREATE TABLE IF NOT EXISTS export.export_consignments (
    id SERIAL PRIMARY KEY,
    tracking_number VARCHAR(20) NOT NULL UNIQUE,
    pbe_number VARCHAR(35) NOT NULL,
    pbe_type INT DEFAULT 1,
    service_type INT DEFAULT 1,
    status INT DEFAULT 1,
    customs_status INT DEFAULT 1,
    exporter_id INT NOT NULL REFERENCES identity.users(id) ON DELETE RESTRICT,
    dnk_center_id INT NOT NULL REFERENCES master.dnk_centers(id) ON DELETE RESTRICT,
    recipient_name VARCHAR(150) NOT NULL,
    recipient_company VARCHAR(150),
    recipient_email VARCHAR(100) NOT NULL,
    recipient_phone VARCHAR(25) NOT NULL,
    recipient_address TEXT NOT NULL,
    recipient_city VARCHAR(100) NOT NULL,
    recipient_state VARCHAR(100),
    recipient_postal_code VARCHAR(20) NOT NULL,
    recipient_country VARCHAR(100) NOT NULL,
    destination_country_code VARCHAR(5) NOT NULL,
    actual_weight_kg NUMERIC(10,3) DEFAULT 0.500,
    volumetric_weight_kg NUMERIC(10,3) DEFAULT 0.600,
    chargeable_weight_kg NUMERIC(10,3) DEFAULT 0.600,
    declared_value_inr NUMERIC(18,2) DEFAULT 0.00,
    total_shipping_cost_inr NUMERIC(18,2) DEFAULT 0.00,
    customs_assessed_duty_inr NUMERIC(18,2) DEFAULT 0.00,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- 5. Tracking Milestones
CREATE TABLE IF NOT EXISTS tracking.tracking_milestones (
    id SERIAL PRIMARY KEY,
    export_consignment_id INT NOT NULL REFERENCES export.export_consignments(id) ON DELETE CASCADE,
    tracking_number VARCHAR(20) NOT NULL,
    event_time TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    facility_location VARCHAR(150) NOT NULL,
    status_headline VARCHAR(100) NOT NULL,
    status_category INT NOT NULL,
    detailed_description TEXT,
    performed_by VARCHAR(100)
);
CREATE INDEX IF NOT EXISTS idx_tracking_num_time ON tracking.tracking_milestones(tracking_number, event_time DESC);
