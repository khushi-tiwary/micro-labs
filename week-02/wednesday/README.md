# Wednesday micro-lab

### Topics covered:
- Delegates, Func, Action, Predicate 
- Events and event handlers (publisher/subscriber) 
- Lambda expressions and closures 
- Extension methods
 
 **Task:** Add an InvoiceGenerated event with two subscribers (a console logger and a high-bill alert). Replace a hand-written filter loop with a lambda-based filter method. 

 **How:** BillingService raises InvoiceGenerated for every invoice, the logger and the alert subscriber wth +=. Filter takes the condition as a Func<Invoice,bool> so one method works for any condition.

 **Run:** `dotnet run`