-- ✅ Correct T-SQL script to generate 1000 random showroom records
INSERT INTO [CarsDealersManagementDb].[dbo].[Showrooms]
    ([Name], [Location], [Capacity], [DealerId],
     [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
SELECT TOP (1000)
    CONCAT(LN.LocationName, ' ', CAST(ABS(CHECKSUM(NEWID())) % 200 + 1 AS NVARCHAR(4)), ' Showroom') AS [Name],
    CONCAT(LN.LocationName, ', ', RN.RegionName) AS [Location],
    ABS(CHECKSUM(NEWID())) % 200 + 10 AS [Capacity],   -- between 10 and 210 cars
    D.Id AS [DealerId],                                -- existing DealerId from Dealers table
    0 AS [CreatedBy],
    DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 365, GETDATE()) AS [CreatedAt],
    0 AS [UpdatedBy],
    DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 100, GETDATE()) AS [UpdatedAt]
FROM 
    (SELECT value AS LocationName 
     FROM STRING_SPLIT('Downtown,Airport,Harbor,Suburban,Central,West End,East Side,North Park,South Valley,Hilltop', ',')) AS LN
CROSS JOIN 
    (SELECT value AS RegionName 
     FROM STRING_SPLIT('New York,Los Angeles,Chicago,Houston,Phoenix,Philadelphia,San Antonio,San Diego,Dallas,San Jose', ',')) AS RN
CROSS JOIN 
    (SELECT TOP (1000) Id FROM [CarsDealersManagementDb].[dbo].[Dealers] ORDER BY NEWID()) AS D
ORDER BY NEWID();
