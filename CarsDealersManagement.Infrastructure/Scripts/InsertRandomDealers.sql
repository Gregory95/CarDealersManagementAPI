INSERT INTO [CarsDealersManagementDb].[dbo].[Dealers]
    ([FirstName], [LastName], [Email], [PhoneNumber],
     [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
SELECT TOP 1000
    -- Random first and last names from sample sets
    FirstNames.value AS FirstName,
    LastNames.value AS LastName,
    LOWER(FirstNames.value + '.' + LastNames.value + CAST(ABS(CHECKSUM(NEWID())) % 100 AS NVARCHAR(3)) + '@example.com') AS Email,
    CONCAT('+1-', FORMAT(ABS(CHECKSUM(NEWID())) % 900 + 100, '000'),
                 '-', FORMAT(ABS(CHECKSUM(NEWID())) % 900 + 100, '000'),
                 '-', FORMAT(ABS(CHECKSUM(NEWID())) % 9000 + 1000, '0000')) AS PhoneNumber,
    0 AS CreatedBy,
    DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 365, GETDATE()) AS CreatedAt,
    0 AS UpdatedBy,
    DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 100, GETDATE()) AS UpdatedAt
FROM 
    (SELECT value FROM STRING_SPLIT('John,Jane,David,Emily,Michael,Sarah,Robert,Linda,Daniel,Karen', ',')) AS FirstNames
CROSS JOIN 
    (SELECT value FROM STRING_SPLIT('Smith,Johnson,Williams,Brown,Jones,Miller,Davis,Garcia,Martinez,Wilson', ',')) AS LastNames
ORDER BY NEWID();