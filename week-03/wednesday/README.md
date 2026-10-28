# Week 3 - Wednesday: Procedures, Functions and Views
 
Built on the TeleBill database.
 
## What's in lab.sql
- **usp_GetCustomerUsage**: stored procedure: a customer's usage for a month (OUTPUT minutes, return codes, TRY/CATCH)
- **fn_GetApplicableRates**: table-valued function: rates for a customer's active plan
- **vw_MonthlyUsage**: view: monthly usage per customer
 
## How to run
1. Open lab.sql in SSMS, connect to `(localdb)\MSSQLLocalDB`, select TeleBill.
2. Press F5.
 
## Key learnings
- Procedures run with EXEC; functions are used inside SELECT
- OUTPUT sends a value back; RETURN sends a status code
- A view is a saved query with no data of its own
 
