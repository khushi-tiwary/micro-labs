
USE TeleBill; -- work inside TeleBill
GO   -- end of batch
 
-- Drop the existing views, functions and procedure and tables
DROP VIEW IF EXISTS dbo.vw_MonthlyUsage; -- delete old view if present
DROP FUNCTION IF EXISTS dbo.fn_GetApplicableRates; -- delete old functions
DROP FUNCTION IF EXISTS dbo.fn_CallCharge;
DROP PROCEDURE IF EXISTS dbo.usp_GetCustomerUsage; -- delete old procedure
DROP TABLE IF EXISTS dbo.Rates;   -- delete child tables first
DROP TABLE IF EXISTS dbo.UsageRecords;

--Delete existing data from the original tables 
DELETE FROM dbo.Subscriptions; -- empty tables: child first
DELETE FROM dbo.Customers; -- then parents
DELETE FROM dbo.Plans;

--Reset the identity counters. Suppose a table has an identity column that generates IDs automatically. 
DBCC CHECKIDENT ('dbo.Plans', RESEED, 0); -- next PlanId = 1
DBCC CHECKIDENT ('dbo.Customers', RESEED, 999); -- next CustomerId = 1000
DBCC CHECKIDENT ('dbo.Subscriptions', RESEED, 0); -- next SubscriptionId = 1
GO
 
 /*  2. NEW TABLES  */
CREATE TABLE dbo.UsageRecords (    -- every call/SMS/data use
    UsageId INT IDENTITY(1,1) NOT NULL,  -- auto number
    SubscriptionId INT NOT NULL,  -- which subscription used it
    UsageDate DATE  NOT NULL,  -- when
    UsageType VARCHAR(10) NOT NULL,  -- Call / SMS / Data
    Quantity DECIMAL(10,2) NOT NULL,  -- minutes / messages / MB
    CONSTRAINT PK_UsageRecords PRIMARY KEY (UsageId),
    CONSTRAINT FK_Usage_Subs FOREIGN KEY (SubscriptionId) REFERENCES dbo.Subscriptions(SubscriptionId), -- must exist
    CONSTRAINT CK_Usage_Type CHECK (UsageType IN ('Call','SMS','Data')),  -- only these 3, Check constraint
    CONSTRAINT CK_Usage_Qty CHECK (Quantity >= 0) -- no negatives, Check constraint
);
 
CREATE TABLE dbo.Rates (  -- price per unit for each plan
    RateId INT IDENTITY(1,1) NOT NULL, --Primary key
    PlanId  INT NOT NULL,   -- which plan
    UsageType VARCHAR(10) NOT NULL,   -- Call / SMS / Data
    RatePerUnit DECIMAL(10,2) NOT NULL,   -- Rs per minute / SMS / MB
    CONSTRAINT PK_Rates PRIMARY KEY (RateId),
    CONSTRAINT FK_Rates_Plans FOREIGN KEY (PlanId) REFERENCES dbo.Plans(PlanId),
    CONSTRAINT CK_Rates_Type CHECK (UsageType IN ('Call','SMS','Data')),
    CONSTRAINT UQ_Rates_PlanType UNIQUE (PlanId, UsageType)  -- one rate per type per plan
);
GO
 
/*  3. SAMPLE DATA (parents first, then children) */
INSERT INTO dbo.Plans (PlanCode, PlanName, PlanType, MonthlyFee, IncludedMinutes, DataGB)
VALUES ('PRE-199', N'Basic Prepaid', 'Prepaid',  199, 300,  1.5),   -- PlanId 1
       ('PRE-399', N'Smart Prepaid', 'Prepaid',  399, 1000, 2),     -- 2
       ('POS-599', N'Family Postpaid','Postpaid', 599, 3000, 75),    -- 3
       ('POS-999', N'Unlimited Pro','Postpaid', 999, 9999, 150),   -- 4
       ('PRE-99',  N'Starter', 'Prepaid',  99,  100,  0.5);   -- 5 (unused)
 
INSERT INTO dbo.Customers (FullName, Email, Phone, City)
VALUES (N'Aarav Sharma', 'aarav@example.com',  '9876543210', N'Bengaluru'),  -- 1000
       (N'Priya Nair',   'priya@example.com',  '9123456780', N'Kochi'),      -- 1001
       (N'Rohan Das',    NULL,                 '9988776655', N'Kolkata'),    -- 1002
       (N'Meera Iyer',   'meera@example.com',  '9090909090', N'Chennai'),    -- 1003
       (N'Arjun Mehta',  NULL,                 '9811122233', N'Bengaluru'),  -- 1004
       (N'Sneha Reddy',  'sneha@example.com',  '9822233344', N'Hyderabad'),  -- 1005
       (N'Vikram Singh', 'vikram@example.com', '9833344455', N'Delhi'),      -- 1006 (no plan)
       (N'Ananya Gupta', NULL,                 '9844455566', N'Bengaluru');  -- 1007 (no plan)
 
INSERT INTO dbo.Subscriptions (CustomerId, PlanId, StartDate, EndDate, Status)
VALUES (1000, 1, '2026-08-01', NULL,         'Active'),     -- Sub 1: Aarav, Basic
       (1001, 3, '2026-07-15', NULL,         'Active'),     -- 2: Priya, Family
       (1002, 2, '2026-06-01', '2026-09-30', 'Cancelled'),  -- 3: Rohan, old plan
       (1002, 4, '2026-10-01', NULL,         'Active'),     -- 4: Rohan, Unlimited
       (1003, 2, '2026-09-10', NULL,         'Active'),     -- 5: Meera, Smart
       (1004, 3, '2026-08-20', NULL,         'Suspended'),  -- 6: Arjun, Family
       (1005, 4, '2026-09-01', NULL,         'Active');     -- 7: Sneha, Unlimited
 
INSERT INTO dbo.UsageRecords (SubscriptionId, UsageDate, UsageType, Quantity)
VALUES (1,'2026-10-02','Call',120), (1,'2026-10-15','Call',95), (1,'2026-09-20','Call',80),  -- Aarav calls
       (2,'2026-10-03','Call',400), (2,'2026-10-18','Call',250),  -- Priya calls
       (4,'2026-10-05','Call',600),                               -- Rohan calls
       (5,'2026-10-07','Call',180), (5,'2026-09-25','Call',220),  -- Meera calls
       (7,'2026-10-10','Call',900), (7,'2026-10-20','Call',300),  -- Sneha calls
       (1,'2026-10-05','SMS',50),   (5,'2026-10-08','SMS',30),    -- SMS
       (1,'2026-10-06','Data',1024), (2,'2026-10-09','Data',20480), (7,'2026-10-12','Data',51200); -- data in MB
 
INSERT INTO dbo.Rates (PlanId, UsageType, RatePerUnit)
VALUES (1,'Call',1.00), (1,'SMS',0.50), (1,'Data',0.05),   -- Basic
       (2,'Call',0.80), (2,'SMS',0.40), (2,'Data',0.04),   -- Smart
       (3,'Call',0.60), (3,'SMS',0.30), (3,'Data',0.02),   -- Family
       (4,'Call',0.00), (4,'SMS',0.00), (4,'Data',0.01),   -- Unlimited (free calls)
       (5,'Call',1.20), (5,'SMS',0.60), (5,'Data',0.08);   -- Starter
GO
 
/*  4. STORED PROCEDURE: customer's usage for one month  */
CREATE OR ALTER PROCEDURE dbo.usp_GetCustomerUsage -- CREATE OR ALTER creates the procedure if it does not exist, or updates it if it already exists.
    @CustomerId INT, -- input: which customer
    @Year INT, -- input: year
    @Month INT, -- input: month (1-12)
    @TotalMinutes DECIMAL(10,2) OUTPUT -- output: sent back to caller
AS
BEGIN --start od the procedure
    SET NOCOUNT ON; -- hide "(n rows affected)",suppresses these row-count messages.
    BEGIN TRY  -- like try { } in C#
        IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE CustomerId = @CustomerId) --Validate the customer
            RETURN 1; -- 1 = customer not found
        IF @Month NOT BETWEEN 1 AND 12
            RETURN 2; -- 2 = invalid month
 
        DECLARE @From DATE = DATEFROMPARTS(@Year, @Month, 1);  -- first day, e.g. 2026-10-01
        DECLARE @To   DATE = DATEADD(MONTH, 1, @From); -- first day of next month
 
        SELECT u.UsageType, SUM(u.Quantity) AS TotalQuantity -- result: total per type
        FROM dbo.UsageRecords u
        JOIN dbo.Subscriptions s ON s.SubscriptionId = u.SubscriptionId  -- usage -> customer
        WHERE s.CustomerId = @CustomerId
          AND u.UsageDate >= @From AND u.UsageDate < @To    -- only that month
        GROUP BY u.UsageType; -- one row per Call/SMS/Data
 
        SELECT @TotalMinutes = ISNULL(SUM(u.Quantity), 0)  -- store call minutes in OUTPUT (0 if none)
        FROM dbo.UsageRecords u
        JOIN dbo.Subscriptions s ON s.SubscriptionId = u.SubscriptionId
        WHERE s.CustomerId = @CustomerId AND u.UsageType = 'Call'
          AND u.UsageDate >= @From AND u.UsageDate < @To;
 
        RETURN 0; -- 0 = success
    END TRY
    BEGIN CATCH  -- runs only if an error happened
        SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_MESSAGE() AS ErrorMessage;  -- show the error
        RETURN 99;  -- 99 = unexpected error
    END CATCH
END;--end of the procedure
GO
 
/*  5. TABLE-VALUED FUNCTION: rates for a customer's active plan  */
CREATE OR ALTER FUNCTION dbo.fn_GetApplicableRates (@CustomerId INT)  -- input: customer
RETURNS TABLE  -- returns rows, not one value
AS
RETURN
(
    SELECT c.FullName, p.PlanName, r.UsageType, r.RatePerUnit
    FROM dbo.Subscriptions s
    JOIN dbo.Customers c ON c.CustomerId = s.CustomerId   -- get name
    JOIN dbo.Plans p ON p.PlanId = s.PlanId  -- get plan name
    JOIN dbo.Rates r ON r.PlanId = s.PlanId  -- get that plan's rates
    WHERE s.CustomerId = @CustomerId AND s.Status = 'Active'  -- current plan only
);
GO
 
/*  6. SCALAR FUNCTION: returns one value */
CREATE OR ALTER FUNCTION dbo.fn_CallCharge (@Minutes DECIMAL(10,2), @Rate DECIMAL(10,2))
RETURNS DECIMAL(10,2)  -- returns a single number
AS
BEGIN
    RETURN @Minutes * @Rate;  -- charge = minutes x rate
END;
GO
 
/*  7. VIEW: monthly usage per customer (saved query)  */
CREATE OR ALTER VIEW dbo.vw_MonthlyUsage
AS
SELECT c.CustomerId,
       c.FullName,
       YEAR(u.UsageDate)  AS UsageYear,  -- 2026
       MONTH(u.UsageDate) AS UsageMonth, -- 9, 10
       SUM(CASE WHEN u.UsageType = 'Call' THEN u.Quantity ELSE 0 END) AS CallMinutes,-- add only calls
       SUM(CASE WHEN u.UsageType = 'SMS'  THEN u.Quantity ELSE 0 END) AS SmsCount, -- add only SMS
       SUM(CASE WHEN u.UsageType = 'Data' THEN u.Quantity ELSE 0 END) AS DataMB -- add only data
FROM dbo.Customers c
JOIN dbo.Subscriptions s ON s.CustomerId = c.CustomerId
JOIN dbo.UsageRecords u  ON u.SubscriptionId = s.SubscriptionId
GROUP BY c.CustomerId, c.FullName, YEAR(u.UsageDate), MONTH(u.UsageDate);  -- one row per customer per month
GO                         -- (no ORDER BY allowed inside a view)
 
/*  8. TESTS */
DECLARE @Minutes DECIMAL(10,2), @Result INT; -- variables to catch output + return code
 
EXEC @Result = dbo.usp_GetCustomerUsage 1000, 2026, 10, @Minutes OUTPUT;  -- Aarav, Oct
SELECT @Result AS ReturnCode, @Minutes AS TotalMinutes; -- expect 0, 215
 
EXEC @Result = dbo.usp_GetCustomerUsage 9999, 2026, 10, @Minutes OUTPUT;  -- fake customer
SELECT @Result AS ReturnCode_NoCustomer; -- expect 1
 
EXEC @Result = dbo.usp_GetCustomerUsage 1000, 2026, 13, @Minutes OUTPUT;  -- month 13
SELECT @Result AS ReturnCode_BadMonth; -- expect 2
 
SELECT * FROM dbo.fn_GetApplicableRates(1002); -- Rohan: Unlimited Pro rates (3 rows)
SELECT dbo.fn_CallCharge(215, 1.00) AS Charge; -- expect 215.00
SELECT * FROM dbo.vw_MonthlyUsage ORDER BY FullName, UsageMonth; -- expect 7 rows
GO
 
