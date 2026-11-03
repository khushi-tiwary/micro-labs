# Week-04- Tuesday: ADO.NET Basics

**Task:** Read customers from the TeleBill database into a List<Customer> and print them. Insert one customer and read back the identity value.

## What I did 
- Read customers with `ExceuteReader()` and mapped rows to objects by hand
- Handles nullable columns with `IsDBNull`
- Inserted using a parameterised query
- Got the new ID with `OUTPUT INSERTED.CustomerId` + `ExecuteScalar()`
- Caught duplicate phone errors with an exception filter.

## Output
Read 8 customers (1000-1007), inserted one-> new CustomerId= 1008

1000 | Aarav Sharma | 9876543210 | Bengaluru
1001 | Priya Nair | 9123456780 | Kochi
1002 | Rohan Das | 9988776655 | Kolkata
1003 | Meera Iyer | 9090909090 | Chennai
1004 | Arjun Mehta | 9811122233 | Bengaluru
1005 | Sneha Reddy | 9822233344 | Hyderabad
1006 | Vikram Singh | 9833344455 | Delhi
1007 | Ananya Gupta | 9844455566 | Bengaluru
1008 | Test User | 9000000001 | Bengaluru

Inserting new customer
That phone number already exists.
