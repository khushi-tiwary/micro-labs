var billing = new BillingService(); // the publisher:it raises the event
billing.InvoiceGenerated += LogInvoice; //subscriber 1: console logger, named method

decimal threshold=500; //bills above this amount count as "high"
billing.InvoiceGenerated+= invoice=> //subscriber 2: high-bill alert (a lambda)
{
    if (invoice.Amount > threshold) //the lambda uses threshold from outside (a closure)
    Console.WriteLine($"[ALERT] High bill for {invoice.Customer} : Rs.{invoice.Amount}");
};

billing.Generate("Khushi", 320);
billing.Generate("Ravi", 780);
billing.Generate("Asha", 150);
billing.Generate("Dev", 1200);

List<Invoice> highBills = new List<Invoice>();// empty list for the results
foreach(Invoice inv in billing.Invoices) //check every invoice
{
    if(inv.Amount> threshold)
    highBills.Add(inv);
}

Console.WriteLine("\nHigh bills (loop): " + highBills.Count);

List<Invoice> high = Filter(billing.Invoices, inv=>inv.Amount > threshold);
List<Invoice> low = Filter(billing.Invoices, inv=>inv.Amount <200);
Console.WriteLine("High bills (lambda) :" + high.Count);
Console.WriteLine("Low bills (lambda) :" + low.Count);

// subscriber 1: matches the event 's shape
static void LogInvoice(Invoice invoice)
{
    Console.WriteLine ($"[LOG] Invoice generated for{invoice.Customer}: Rs.{invoice.Amount}");
}

//lambda-based filter- Func <Invoice, bool> is a method that takes an Invoice and returns true/false
static List<Invoice> Filter (List<Invoice> items, Func <Invoice, bool> condition)
{
    List<Invoice> result= new List <Invoice>();//empty list for the matches
    foreach(Invoice item in items)
    {
        if(condition(item))
        result.Add(item);
    }
    return result;
}

record Invoice(String Customer, decimal Amount);//

class BillingService // the publisher
{
public List<Invoice> Invoices {get; }= new List<Invoice> ();
public event Action <Invoice>? InvoiceGenerated;

public void Generate (string customer, decimal amount)
    {
    Invoice invoice = new Invoice(customer, amount); // build the invoice
    Invoices.Add(invoice); // save it
    InvoiceGenerated?.Invoke (invoice); //raise the event, every subscriber runs, in the order they subscribed
}
}