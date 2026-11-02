# Week-04- Monday: CSV Reader with Exception Handling

Reads a usage CSV file that has some bad lines. Bad lines are skipped, counted and reported with their line number, and the program keeps going.

## Concepts used
- try/catch/finally
- Custom exception: `InvalidUsageRecordException`
- Reading files with `StreamReader` inside a `using` block
- Validating each line- wrong number of columns, non-numeric ID, empty line

## Output
Total Rows Checked: 5
Successfully Read: 2
Skipped Bad Rows: 3

Each skipped row is printed with the reason and its line number.