/* Problem stmnt: Model Customer, TariffPlan and Subscription with validation inside constructors and setters. 
Build a small inheritance tree: Customer, PrepaidCustomer, PostpaidCustomer. */

using System;

enum SubscriptionType // enum defines a fixed set of named values
{
Prepaid, // prepaid subscription
Postpaid // postpaid subscription
}

class TariffPlan
{
    //stores plan's name
    public string Name {get; private set;} // private set-only this class can change it's value
   
    //stores monthly charge for tis tariff plan
    public decimal MonthlyCharge {get; private set;} 

// constructor runs whenever a new TariffPlan object is created
    public TariffPlan(string name, decimal monthlyCharge){
    
    if(string.IsNullOrWhiteSpace(name)) // checks for null, empty r ehitespace only plan name
    {
        throw new ArgumentException("Plan name cannot be empty"); //throws an exception if name is invalid
    }
    if(monthlyCharge < 0){
        throw new ArgumentException("Charge cannot be negative");
    }
    Name= name;
    MonthlyCharge = monthlyCharge;
    }
}

// Customer is the parent class that stores common customer information
class Customer 
{
    private string name= string.Empty;


    public string Name // property controls how the customer's name is read and changed
    {
        get{
            return name; // returns the current value of the private field
        }
        set
        {
if (string.IsNullOrWhiteSpace(value))
{
    throw new ArgumentException("Name cannot be empty");
}
            // Stores the supplied value after it passes validation.
            // 'value' represents the new value being assigned to Name.
            name = value;
        }
    }
 
    // Stores the customer's mobile number.
    // Other classes can read it, but cannot directly change it.
    public string MobileNumber { get; private set; }
 
    // Stores the tariff plan associated with this customer.
    // The property refers to a TariffPlan object, not just a string.
    public TariffPlan Plan { get; private set; }
 
    // The Customer constructor initializes a new customer.
    // It receives the name, mobile number, and tariff plan.
    public Customer (string name, string mobileNumber, TariffPlan plan)
    {
        // Checks whether the mobile number is missing or has the wrong length.
        if (string.IsNullOrWhiteSpace(mobileNumber) || mobileNumber.Length != 10)
        {
            // Rejects a mobile number that fails the validation above.
            throw new ArgumentException("Mobile number must have 10 characters.");
        }
 
        // Assigning Name calls its setter, so the name is validated.
        Name = name;
 
        // Stores the validated mobile number.
        MobileNumber = mobileNumber;
 
        // Rejects a null tariff plan before storing it.
        // nameof(plan) supplies the parameter name in the exception.
        Plan = plan ??
            throw new ArgumentNullException(nameof(plan));
    }
 
    // virtual allows child classes to provide their own version
    // of this method using the override keyword.
    public virtual void DisplayDetails()
    {
        // Prints the customer's name.
        Console.WriteLine($"Name: {Name}");
 
        // Prints the customer's mobile number.
        Console.WriteLine($"Mobile: {MobileNumber}");
 
        // Prints the name of the customer's tariff plan.
        Console.WriteLine($"Plan: {Plan.Name}");
    }
}
 
// PrepaidCustomer inherits from Customer.
// The colon means PrepaidCustomer is a specialized type of Customer.
class PrepaidCustomer : Customer
{
    // Stores the prepaid customer's available balance.
    // Other classes can read the balance but cannot directly change it.
    public decimal Balance { get; private set; }
 
    // The child constructor receives customer details and a balance.
    public PrepaidCustomer(
        string name,
        string mobileNumber,
        TariffPlan plan,
        decimal balance)
 
        // Calls the parent Customer constructor first.
        // This initializes the name, mobile number, and tariff plan.
        : base(name, mobileNumber, plan)
    {
        // A prepaid balance must not be negative.
        if (balance < 0)
        {
            // Rejects an invalid starting balance.
            throw new ArgumentException("Balance cannot be negative.");
        }
 
        // Stores the starting balance after validation.
        Balance = balance;
    }
 
    // override replaces the inherited virtual DisplayDetails method
    // with a version that also displays the prepaid balance.
    public override void DisplayDetails()
    {
        // Calls the parent's method to display common customer details.
        base.DisplayDetails();
 
        // Displays the additional information specific to prepaid customers.
        // F2 formats the balance with two digits after the decimal point.
        Console.WriteLine($"Prepaid balance: Rs {Balance:F2}");
    }
}
 
// PostpaidCustomer is another child class of Customer.
// It represents a customer who can have an outstanding bill.
class PostpaidCustomer : Customer
{
    // Stores the maximum permitted credit limit for this example.
    // This code stores the limit but does not enforce billing against it.
    public decimal CreditLimit { get; private set; }
 
    // The constructor receives the common customer information
    // along with the postpaid customer's credit limit.
    public PostpaidCustomer(
        string name,
        string mobileNumber,
        TariffPlan plan,
        decimal creditLimit)
 
        // Initializes the inherited Customer properties by calling
        // the parent constructor.
        : base(name, mobileNumber, plan)
    {
        // Rejects a negative credit limit.
        if (creditLimit < 0)
        {
            // Throws an exception if the credit limit is invalid.
            throw new ArgumentException("Credit limit cannot be negative.");
        }
 
        // Stores the validated credit limit.
        CreditLimit = creditLimit;
    }
 
    // Provides a customized version of DisplayDetails for postpaid customers.
    public override void DisplayDetails()
    {
        // Displays the common customer details from the parent class.
        base.DisplayDetails();
 
        // Displays the credit limit specific to postpaid customers.
        Console.WriteLine($"Credit limit: Rs {CreditLimit:F2}");
    }
}
 
// Subscription represents a subscription associated with a customer.
// It demonstrates composition because it contains a Customer object.
class Subscription
{
    // Stores the customer associated with the subscription.
    // The property can be read publicly but changed only inside this class.
    public Customer Customer { get; private set; }
 
    // Stores whether this subscription is Prepaid or Postpaid.
    public SubscriptionType Type { get; private set; }
 
    // Creates a subscription using a customer and a subscription type.
    public Subscription(Customer customer, SubscriptionType type)
    {
        // Rejects a null customer.
        // If customer is not null, it is assigned to the property.
        Customer = customer ??
            throw new ArgumentNullException(nameof(customer));
 
        // Saves the selected subscription type.
        Type = type;
    }
}
 
// Program contains the application's entry point and execution logic.
class Program
{
    // Main is the method where the console application starts running.
    static void Main()
    {
        // Creates a TariffPlan object named plan.
        // The plan has a name of Basic and a monthly charge of Rs 199.
        // The 'm' suffix makes 199 a decimal literal.
        TariffPlan plan = new TariffPlan("Basic", 199m);
 
        // Creates a PrepaidCustomer object.
        // The variable is declared as Customer, the parent type.
        // The actual object is a PrepaidCustomer with a balance of Rs 500.
        Customer prepaid = new PrepaidCustomer(
            "Asha",
            "9876543210",
            plan,
            500m);
 
        // Creates a PostpaidCustomer object.
        // It has the same tariff plan and a credit limit of Rs 1000.
        // Again, the variable uses the parent Customer type.
        Customer postpaid = new PostpaidCustomer(
            "Riya",
            "9876543211",
            plan,
            1000m);
 
        // Creates a Subscription object for the prepaid customer.
        // The enum value identifies the subscription as Prepaid.
        Subscription subscription =
            new Subscription(prepaid, SubscriptionType.Prepaid);
 
        // Calls DisplayDetails on the prepaid customer.
        // C# runs PrepaidCustomer's overridden implementation.
        prepaid.DisplayDetails();
 
        // Prints a blank line to separate the customer outputs.
        Console.WriteLine();
 
        // Calls DisplayDetails on the postpaid customer.
        // C# runs PostpaidCustomer's overridden implementation.
        postpaid.DisplayDetails();
 
        // Prints another blank line before displaying subscription details.
        Console.WriteLine();
 
        // Displays the subscription type stored in the enum property.
        Console.WriteLine($"Subscription type: {subscription.Type}");
    }
}
 