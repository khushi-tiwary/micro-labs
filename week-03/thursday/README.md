# Week 3 - Thursday: Indexes, Triggers and Transactions
 
## What's in lab.sql
- Generated 100,000 usage rows
- Compared execution plans: no index (scan) -> index (seek + key lookup) -> covering index (seek only)
- Audit trigger logging plan fee changes (old and new values)
- Rollback demo and an all-or-nothing transaction with TRY/CATCH
 
## Key learnings
- Logical reads show whether an index helped
- A covering index removes key lookups
- A rollback undoes the trigger's work too
