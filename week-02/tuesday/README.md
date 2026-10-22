# Tuesday micro-lab

### Topics covered: 
Interfaces and polymorphsm

**Task:** Define IUsageRecord with CallRecord, SmsRecord and DataRecord. Write one method that rates a mixed list of records without any if/else on the type.

**HOw:** Each record has it's own `Rate()`method. `RateAll` just calls `r.Rate(plan)` onevery item, and C# runs the right one automatically (polymorphism).

 **Run:** `dotnet run`