USE TeleBill;
GO

/* 1. Make the 100,000 usage rows(run once) */
SET NOCOUNT ON;
DELETE FROM dbo.UsageRecords WHERE UsageId>15; --keeps the 15 original rows, safw re-run
DROP INDEX IF EXISTS IX_Usage_Sub_Date ON dbo.UsageRecords; --remove indexes from earlier runs
DROP INDEX IF EXISTS IX_Usage_Sub_Date_Covering ON dbo.UsageRecords; 

INSERT INTO dbo.UsageRecords(SubscriptionId, UsageDate, UsageType, Quantity)
SELECT TOP(100000)
       ABS(CHECKSUM(NEWID())) % 7 +1, --random subscription 1-7
       DATEADD(DAY, -(ABS(CHECKSUM(NEWID()))%365), '2026-10-31'), --random date in last year
       CASE ABS(CHECKSUM(NEWID())) % 3 WHEN 0 THEN 'Call'
                                       WHEN 1 THEN 'SMS' ELSE 'DATA' END, --random type
       ABS(CHECKSUM(NEWID())) % 500 +1
       FROM sys.all_objects a CROSS JOIN sys.all_objects b; --cross join=lots of rows
SELECT COUNT(*) AS TotalRows FROM dbo.UsageRecords;
GO

/* 2. Slow query, NO index */

SET STATISTICS IO ON; --SHOW PAGES READ
SET STATISTICS TIME ON; --show time taken

SELECT UsageDate, UsageType, Quantity
FROM dbo.UsageRecords
WHERE SubscriptionId =3 AND UsageDate= '2026-10-15'
--plan: clustered index scan. logical reads in massages
GO

/*3. Add a normal index */
CREATE NONCLUSTERED INDEX IX_Usage_Sub_Date
ON dbo.UsageRecords (SubscriptionId, UsageDate);

SELECT UsageDate, UsageType, Quantity
FROM dbo.UsageRecords WHERE SubscriptionId= 3 AND UsageDate ='2026-10-15'
--plan:index seek + key lookup. Fewer reads
GO

/*4. Covering index */
DROP INDEX IX_Usage_Sub_Date ON dbo.UsageRecords;

CREATE NONCLUSTERED INDEX IX_Usage_Sub_Date_Covering
ON dbo.UsageRecords(SubscriptionId, UsageDate)
INCLUDE (UsageType, Quantity);

SELECT UsageDate, UsageType, Quantity
FROM dbo.UsageRecords
WHERE SubscriptionId= 3 AND UsageDate ='2026-10-15';
--plan: index seek only.Fewer reads


SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;
GO

/*5. Audit trigger */
DROP TABLE IF EXISTS dbo.PlanPriceAudit;
CREATE TABLE dbo.PlanPriceAudit (
AuditId INT IDENTITY(1,1) PRIMARY KEY,
PlanId INT NOT NULL,
OldFee DECIMAL(10,2) NOT NULL,
NewFee DECIMAL(10,2) NOT NULL,
ChangedAt DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(), --when
ChangedBy NVARCHAR(128) NOT NULL DEFAULT SUSER_SNAME(), --whO
);
GO

CREATE OR ALTER TRIGGER dbo.trg_Plans_AuditFee
ON dbo.Plans
AFTER UPDATE  -- runs after any UPDATE on Plans
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(MonthlyFee) RETURN; -- skip if fee wasn't touched
 
    INSERT INTO dbo.PlanPriceAudit (PlanId, OldFee, NewFee)
    SELECT d.PlanId, d.MonthlyFee, i.MonthlyFee -- old value from deleted, new from inserted
    FROM deleted d
    JOIN inserted i ON i.PlanId = d.PlanId
    WHERE d.MonthlyFee <> i.MonthlyFee; -- only real changes
END;
GO
 
-- Test the trigger
UPDATE dbo.Plans SET MonthlyFee = 249 WHERE PlanId = 1; -- Basic 199 -> 249
SELECT * FROM dbo.PlanPriceAudit; -- expect 1 row: 199 -> 249
GO
 
/* STEP 6: Rollback demo */
BEGIN TRANSACTION;  -- start a "draft"
    UPDATE dbo.Plans SET MonthlyFee = MonthlyFee * 2; -- mistake: no WHERE, doubles ALL fees
    SELECT PlanName, MonthlyFee FROM dbo.Plans;  -- all doubled (only you can see this)
ROLLBACK TRANSACTION; -- undo everything since BEGIN
 
SELECT PlanName, MonthlyFee FROM dbo.Plans; -- back to normal
SELECT COUNT(*) AS AuditRows FROM dbo.PlanPriceAudit; -- still 1: trigger's rows were rolled back too
GO
 
/* STEP 7: All or nothing with TRY/CATCH */
BEGIN TRY
    BEGIN TRANSACTION;
        INSERT INTO dbo.Plans (PlanCode, PlanName, PlanType, MonthlyFee)
        VALUES ('PRE-149', N'Mini', 'Prepaid', 149); -- valid
        INSERT INTO dbo.Plans (PlanCode, PlanName, PlanType, MonthlyFee)
        VALUES ('PRE-BAD', N'Broken', 'Prepaid', -5); -- fails CHECK (negative fee)
    COMMIT TRANSACTION; -- never reached
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION; -- undo the valid insert too
    SELECT ERROR_MESSAGE() AS ErrorMessage;
END CATCH;
 
SELECT * FROM dbo.Plans WHERE PlanCode IN ('PRE-149','PRE-BAD');  -- expect 0 rows
GO
 

