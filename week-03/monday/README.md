# Week 3 – Monday: TeleBill Database
 
Created the TeleBill database in SQL Server with three tables, then broke each constraint on purpose to read the errors.
 
## Tables
- **Plans** – plans we sell (PK, UNIQUE code, CHECK type and fee)
- **Customers** – who buys plans (PK, UNIQUE phone, CHECK digits only)
- **Subscriptions** – links customers to plans (PK, two FKs, CHECK status and dates)
 
## How to run
1. Open `telebill.sql` in SSMS, connect to `(localdb)\MSSQLLocalDB`.
2. Run lines 1–80 to create everything.
3. Run lines 82–90 one at a time and read the errors.
 
## Errors tested
| Error | Cause |
|---|---|
| 515 | NULL in a required column |
| 2627 | Duplicate phone |
| 547 | CHECK or FOREIGN KEY failed |
 
## Key learnings
- Money → `DECIMAL`, phone → text
- Use `IS NULL`, never `= NULL`
- Always name constraints
