# Week 04 - Thursday: SQL injection, Stored Procedures, Async

1. Vulnerable customer search built with string concatenation, exploited with `zzz' OR 1=1 --` (returns every customer) and a UNION attack (reads the Plans table).
2. Fixed with a parametraised query: the same input returns 0 rows.
3. Calls usp_RateUsage from c# with an input parameter and reads the return code (second run bills 0 rows).
4. All database calls are async(OpenAsync, ExecuteReaderAsync, ReadAsync).
5. Connection string is read from App.config, not hard-coded.