CREATE TABLE SupplierAddresses (
    Id NVARCHAR(450) NOT NULL PRIMARY KEY,
    OrganisationName NVARCHAR(100) NOT NULL,
    ParentOrganisation NVARCHAR(100),
    Type NVARCHAR(100) NOT NULL,
    Area NVARCHAR(100) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    AddressLine1 NVARCHAR(100),
    AddressLine2 NVARCHAR(100),
    AddressLine3 NVARCHAR(100),
    County NVARCHAR(50),
    Postcode NVARCHAR(10) NOT NULL,
    Telephone NVARCHAR(100),
    Website NVARCHAR(100) NOT NULL,
    Location GEOGRAPHY,
    LastUpdated DATETIME2 NOT NULL,
    IsActive BIT NOT NULL
);
GO

CREATE INDEX [IX_SupplierAddresses_Postcode] ON [dbo].[SupplierAddresses] ([Postcode])
