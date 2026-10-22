var plan= new TariffPlan (0.60m, 0.20m, 0.10m); // call, sms, data rates. m=decimal number
List <IUsageRecord> usage = new List<IUsageRecord> // list whose item type is the interface,
//  so it holds calls, SMS and datarecords together
{
    new CallRecord(12), // 12-min call
    new SmsRecord(3), // 3 sms messages
    new DataRecord(250), //250 mb of data
    new CallRecord(5) // a 5-min call
};

foreach(IUsageRecord r in usage)
Console.WriteLine(r.GetType().Name + ":Rs." + r.Rate(plan)); // GetType().Name- gives the real class name (CallRecord, SmsReord...);
//  r.Rate(plan) runs THAT class's own Rate()

Console.WriteLine("Total: Rs." + RateAll(usage, plan)); // call the lab method once for the whole mixed list

 static decimal RateAll(List<IUsageRecord> records, TariffPlan plan) // static local function takes the list and the plan, returns the total
{
    decimal total=0;
    foreach(IUsageRecord r in records)
    total+=r.Rate(plan); // the real object's Rate() runs-polymorphism
    return total;
}

record TariffPlan(decimal CallRate, decimal SmsRate, decimal DataRate);//one line gives a class with 3 read-only properties, a constructor, and value-based equality

interface IUsageRecord // the contract- every record must be able to rate itself
{
    decimal Rate(TariffPlan plan); // only the signature, no body
}

class CallRecord: IUsageRecord //":IUsageRecord" means it implements the interface.
{
    public int Minutes {get; } // read on;y property
    public CallRecord(int minutes){ 
        Minutes = minutes;}// constructor saves the mins when we write new CallRecord(12)
        public decimal Rate(TariffPlan plan)=> Minutes * plan.CallRate; // keeps the interface's promise, price=minutes*call rate
    }

    class SmsRecord : IUsageRecord
    {
        public int Count {get; } // read only property-number of messages
        public SmsRecord(int count){
            Count=count;} // constructor saves the count
            public decimal Rate(TariffPlan plan) =>Count* plan.SmsRate ; // price=messages*SMS rate
        }

        class DataRecord : IUsageRecord
        {
            public int Megabytes{get; } // read only data used in MB
            public DataRecord( int megabytes) // constructor saves the MB
            {
                Megabytes=megabytes;
            }
            public decimal Rate(TariffPlan plan)=>Megabytes*plan.DataRate; // price=MB * data rate
        }







