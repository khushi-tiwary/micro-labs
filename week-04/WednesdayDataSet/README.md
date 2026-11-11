# Week 4- DataSet, DataRelation and SqlTransaction

Works with the TeleBillDB database (Customers and Subscriptions).

## What it does
1. Fill a DataSet with Customers and Subscriptions using SqlDataAdapter(disconnected model).
2. Adds a DataRelation and prints each customer's subscription with GetChildRows.
3. Changes a customer's city in memory and saves it with SqlAdapter.Update (SqlCommanBuilder writes the UPDATE).
4. Inserts a customer and a subscription inside one SqlTransaction:
   - PlanId 999 fails the foreign key, so thw whole transaction rolls back(customer not saved).
   - A valid PlanId commits both insets.

## Concepts Used
  - DataTable. DataSet, SqlDataAdapter, SqlCommandBuilder, DataRelation
  - RowState(Modified-> Unchanged after Update), UpdateBatchSize
  - SqlTransaction: Commit and Rollback
  - DataReader vs DataSet: reader is fast and forward only; DataSet is in-memory and editable

## Output
Adapter.Update saved 1 row(s).
Failed transaction: customer saved? False (rolled back)
Successful transaction: customer saved? True

