-- ✅ Generate 1000 random Contact Person records
INSERT INTO [CarsDealersManagementDb].[dbo].[ContactPersons]
    ([FirstName], [LastName], [Email], [PhoneNumber],
     [ShowroomId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
SELECT TOP (1000)
    FN.FirstName,
    LN.LastName,
    LOWER(FN.FirstName + '.' + LN.LastName + CAST(ABS(CHECKSUM(NEWID())) % 1000 AS NVARCHAR(3)) + '@example.com') AS [Email],
    CONCAT('+1-', FORMAT(ABS(CHECKSUM(NEWID())) % 900 + 100, '000'),
                 '-', FORMAT(ABS(CHECKSUM(NEWID())) % 900 + 100, '000'),
                 '-', FORMAT(ABS(CHECKSUM(NEWID())) % 9000 + 1000, '0000')) AS [PhoneNumber],
    S.Id AS [ShowroomId],
    0 AS [CreatedBy],
    DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 365, GETDATE()) AS [CreatedAt],
    0 AS [UpdatedBy],
    DATEADD(DAY, -ABS(CHECKSUM(NEWID())) % 100, GETDATE()) AS [UpdatedAt]
FROM 
    (SELECT value AS FirstName 
     FROM STRING_SPLIT('John,Jane,David,Emily,Michael,Sarah,Robert,Linda,Daniel,Karen,Chris,Laura,Matthew,Olivia,Benjamin,Sophia,James,Emma,Joseph,Mia', ',')) AS FN
CROSS JOIN 
    (SELECT value AS LastName 
     FROM STRING_SPLIT('Smith,Johnson,Williams,Brown,Jones,Miller,Davis,Garcia,Martinez,Wilson,Anderson,Taylor,Thomas,Moore,Jackson,Martin,Lee,White,Harris,Clark', ',')) AS LN
CROSS JOIN 
    (SELECT TOP (1000) Id FROM [CarsDealersManagementDb].[dbo].[Showrooms] ORDER BY NEWID()) AS S
ORDER BY NEWID();
