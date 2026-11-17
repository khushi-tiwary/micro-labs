
# Tuesday Micro-Lab – Entity Framework Core

## Topics Covered
- Migrations: Add, Update, Remove, SQL Script
- Relationships: One-to-One, One-to-Many, Many-to-Many
- Navigation Properties
- Foreign Keys and Cascade Behaviours
- Required vs Optional Relationships
- Value Conversions and Owned Types

## Practical Implementation
Created an `OffersManagement` project using C#, Entity Framework Core and SQL Server.

Implemented a Many-to-Many relationship between `Offer` and `PlanOffer` using the `OfferPlanOffer` junction table.

## Migration Commands

```powershell
Add-Migration InitialCreate
Update-Database
Remove-Migration
Script-Migration -Output "OffersDB.sql"
```

## Learning Outcomes
- Implemented Many-to-Many relationships.
- Configured navigation properties and foreign keys.
- Generated and analysed SQL migration scripts.
- Retrieved related data using `Include()` and `ThenInclude()`.

## Output

```text
Plan: Premium Plan
Summer Discount - 20%
New Customer Offer - 10%

Plan: Basic Plan
Summer Discount - 20%
```

## Conclusion
Gained practical experience with EF Core migrations, database relationships and SQL Server integration.
