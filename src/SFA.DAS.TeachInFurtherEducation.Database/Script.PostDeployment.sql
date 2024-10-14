/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO dbo.SupplierAddresses (
    Id, OrganisationName, ParentOrganisation, Type, Area, City, AddressLine1, 
    AddressLine2, AddressLine3, County, Postcode, Telephone, Website, Location, 
    LastUpdated, IsActive
)
VALUES (
    '0031c88445927158d4e0372c0ca87376', 
    'Dudley College of Technology', 
    '', 
    'General FE College', 
    'West Midlands', 
    'Dudley', 
    'The Broadway,', 
    '', 
    '', 
    '', 
    'DY1 4AS', 
    '01384 363 000', 
    'http://www.dudleycol.ac.uk', 
    'POINT (-2.082623 52.514257)', 
    '2024-09-19T14:08:01.5990000', 
    'True'
);
