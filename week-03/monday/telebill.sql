USE master; --"master" isthe system database that keeps the list of all databases.We must be in master to drop/create TeleBill
Go --SSMS "batch separator"

IF DB_ID('TeleBill') IS NOT NULL --if TeleBill already exists...
BEGIN
   ALTER DATABASE TeleBill SET SINGLE_USER WITH ROLLBACK IMMEDIATE; --kick out other connections
   DROP DATABASE TeleBill; --delete is so we start fresh
END
GO

CREATE DATABASE TeleBill; --make a new empty database
GO
USE TeleBill; --work inside it from now on
GO

CREATE TABLE dbo.Plans(
PlanId INT IDENTITY(1,1) NOT NULL,
PlanCode VARCHAR(20)  NOT NULL,
PlanName NVARCHAR(100) NOT NULL,
PlanType VARCHAR(10) NOT NULL,
MonthlyFee DECIMAL(10,2) NOT NULL,
IncludedMinutes INT NOT NULL DEFAULT 0,
DataGB DECIMAL(6,2) NOT NULL DEFAULT 0,
IsActive BIT NOT NULL DEFAULT 1,

CONSTRAINT PK_Plans PRIMARY KEY(PlanId), --unique row ID
CONSTRAINT UQ_Plans_PlanCode UNIQUE (PlanCode), --bo duplicate codes
CONSTRAINT CK_Plans_Type CHECK(PlanType IN('Prepaid', 'Postpaid')),
CONSTRAINT CK_Plans_Fee CHECK (MonthlyFee >= 0) --no negative fee
);
GO
CREATE TABLE dbo.Customers(
CustomerId INT IDENTITY(1000,1) NOT NULL,
FullName NVARCHAR(100)  NOT NULL,
Email VARCHAR(255) NULL,
Phone VARCHAR(15) NOT NULL,  --text, not  number
City NVARCHAR(60) NULL,
CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(), --auto timestamp

CONSTRAINT PK_Customers PRIMARY KEY(CustomerId), --unique row ID
CONSTRAINT UQ_Customers_Phone UNIQUE (Phone), --no duplicate phones
CONSTRAINT CK_Customer_Phone CHECK(Phone NOT LIKE '%[^0-9+]%') --on;y digits and +
);
GO

CREATE TABLE dbo.Subscriptions (
    SubscriptionId INT IDENTITY(1,1) NOT NULL,
    CustomerId     INT  NOT NULL,                  -- which customer
    PlanId         INT  NOT NULL,                  -- which plan
    StartDate      DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE), -- today
    EndDate        DATE NULL,                      -- NULL = still running
    Status         VARCHAR(10) NOT NULL DEFAULT 'Active',
 
    CONSTRAINT PK_Subscriptions  PRIMARY KEY (SubscriptionId),
    CONSTRAINT FK_Subs_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId), -- must exist
    CONSTRAINT FK_Subs_Plans     FOREIGN KEY (PlanId)     REFERENCES dbo.Plans(PlanId),
    CONSTRAINT CK_Subs_Status    CHECK (Status IN ('Active','Suspended','Cancelled')),
    CONSTRAINT CK_Subs_Dates     CHECK (EndDate IS NULL OR EndDate >= StartDate) -- end after start
);
GO

INSERT INTO dbo.Plans (PlanCode, PlanName, PlanType, MonthlyFee, IncludedMinutes, DataGB)
VALUES ('PRE-199', N'Basic Prepaid',   'Prepaid',  199, 300,  1.5),
       ('PRE-399', N'Smart Prepaid',   'Prepaid',  399, 1000, 2),
       ('POS-599', N'Family Postpaid', 'Postpaid', 599, 3000, 75);
 
INSERT INTO dbo.Customers (FullName, Email, Phone, City)
VALUES (N'Aarav Sharma', 'aarav@example.com', '9876543210', N'Bengaluru'),
       (N'Priya Nair',   'priya@example.com', '9123456780', N'Kochi'),
       (N'Rohan Das',    NULL,                '9988776655', N'Kolkata');
 
INSERT INTO dbo.Subscriptions (CustomerId, PlanId, StartDate)
VALUES (1000, 1, '2026-08-01'),   -- Aarav -> Basic Prepaid
       (1001, 3, '2026-07-15');   -- Priya -> Family Postpaid
                                  -- Rohan has none (useful for Tuesday)
GO
 
SELECT * FROM dbo.Plans;          -- check the data
SELECT * FROM dbo.Customers;
SELECT * FROM dbo.Subscriptions;

INSERT INTO dbo.Customers (FullName, Phone) VALUES (NULL, '9000000001');      -- 515: NULL not allowed
INSERT INTO dbo.Customers (FullName, Phone) VALUES (N'Copy', '9876543210');   -- 2627: duplicate phone
INSERT INTO dbo.Customers (FullName, Phone) VALUES (N'Bad', '98765ABCDE');    -- 547: CHECK failed
INSERT INTO dbo.Plans (PlanCode, PlanName, PlanType, MonthlyFee)
       VALUES ('X1', N'Neg', 'Prepaid', -10);                                 -- 547: negative fee
INSERT INTO dbo.Subscriptions (CustomerId, PlanId) VALUES (9999, 1);          -- 547: customer doesn't exist
DELETE FROM dbo.Plans WHERE PlanId = 1;                                       -- 547: plan still in use
INSERT INTO dbo.Subscriptions (CustomerId, PlanId) VALUES (1002, 2);          -- works! check the defaults
SELECT * FROM dbo.Subscriptions;




