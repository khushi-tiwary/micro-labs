// 10 customers and 1,000 generated usage records

string[] names= {"Khushi", "Ravi", "Asha", "Dev", "Meera", "Arjun", "Sara", "Kabir", "Nisha", "Rohan"}; // 10 customer names in array 
List<Customer> customers= new List<Customer>();// empty list that holds 10 customer objs
for(int i=0;i<names.Length;i++) //loop 10 times
  customers.Add(new Customer ("98765000" + i.ToString("D2"), names[i]));//  ToString- turns 0 into 00, 7 into 07 so that mobiles 9876500000 to 9876500009

  Random random = new Random(42); //fixed seed, so the same "random" data comes out every run
  string[] types ={"Call", "SMS", "Data"}; // the three usage types
  List<Usage> usage = new List<Usage>(); // empty list, the 1,000 records go here
  for(int i=0;i<1000; i++)
  {
    string mobile = customers[random.Next(customers.Count)].Mobile; // Gives a number from 0 to 9, pick a random customer
    string type= types[random.Next(3)]; // pick a random type (gives 0,1 or 2)
    int quantity = type =="Call"? random.Next(1,61) // call lasts: 1-60 mins
                 : type =="Sms"? random.Next(1,11) // sms: 1-10 messages
                 : random.Next(10,1001); // data: 10-1000 MB
    decimal rate= type == "Call" ? 0.06m: type=="Sms"? 0.20m : 0.10m; // price per unit
    usage.Add(new Usage(mobile, type, quantity, quantity*rate)); //create the record(charge=quantity*rate) and add it to the list
  }
  Console.WriteLine($"Generated {usage.Count} usage records for {customers.Count} customers\n"); 
    
    //These are the 10 LINQ drills
    // 1. Count: how many records are calls?
    int callCount = usage.Count(u=>u.Type =="Call");
    Console.WriteLine($"1. Number of calls:{callCount}");

   // 2. Sum: adds up total revenue from all usages
    decimal revenue= usage.Sum(u=>u.Charge);
    Console.WriteLine($"2. Total revenue: Rs.{revenue:N2}"); // N2-shows 2 decimals with commas, like 22,269.80

    // 3. Where + Average: First keep only the calls (Where) , then find avg of their minutes
    double avgCall = usage.Where(u=>u.Type =="Call").Average(u=>u.Quantity);
    Console.WriteLine($"3. Average call length: {avgCall:F1} min"); // F1-1 decimal place like 30.6

//4. Max+ Min: longestcall and smallestdata session
    int longestCall = usage.Where(u=>u.Type=="Call").Max(u=> u.Quantity);//biggest number of mins among all calls
    int smallestData = usage.Where(u=>u.Type=="Data").Min(u=> u.Quantity);//smallest number of MB among all data sessions
    Console.WriteLine($"4. Longest call: {longestCall} min, smallest data session:{smallestData} MB");

 // 5. OrderByDescending + Take + Select: 3 most expensive records
    var top3= usage.OrderByDescending(u=>u.Charge).Take(3). Select(u=>$"{u.Type} {u.Quantity} = Rs.{u.Charge:N2}");// sort by charge, biggest first->keep the first 3->turn each record into a short text line
    Console.WriteLine("5. Top 3 most expensive records: " + string.Join (" | ", top3));

// 6. GroupBy: number of records and revenue per usage types
     Console.WriteLine("6. Per usage type:");
     foreach (var g in usage.GroupBy(u=>u.Type))// GroupBy puts the records into buckets-one bucket for call, one for sms, one for data
     Console.WriteLine($"  {g.Key, -4}:{g.Count()} records, Rs.{g.Sum(u=>u.Charge):N2}"); // g.Key-bucket's name("Call"), g.Count() and g.Sum() work oon just that bucket 

// 7. Any + All: yes/no quesztions
      bool anyLongCall = usage.Any(u=>u.Type=="Call" && u.Quantity == 60); //is there at least one 60 mins - call?
      bool allPositive = usage.All(u=>u.Charge > 0); //is every charge above zer0?
      Console.WriteLine($"7. Any 60-mincall? {anyLongCall}. All changes positive? {allPositive}");

// 8. FirstOrDefault: Khushi's first SMS , and a record that doesn't exist
Usage? firstSms = usage.FirstOrDefault(u=>u.Mobile == "9876500000" && u.Type =="Sms");
Usage? roaming = usage.FirstOrDefault(u=> u.Type == "Roaming"); // no such type, so this is null. "?"- only read quantity if firstSms is not null
Console.WriteLine($"8. Khushi's first SMS: {firstSms?.Quantity}messages. Roaming record: {(roaming == null ? "none" : "found")}");
     
      // 9. Join+ GroupBy + anonymous type: top 3 customers by spend, with names
      var topSpenders = usage // 3 customers who spend the most
      //Join: match each usage record to the customer with the same mobile number,
    // and keep just the customer's name and the record's charge
          .Join(customers, u=>u.Mobile, c=> c.Mobile, (u,c)=>new{c.Name, u.Charge}) // attach the name to each record
          .GroupBy(x=>x.Name) //one group per customer
          .Select(g=> new {Name= g.Key, Total= g.Sum(x=>x.Charge)}) //Select: for each bucket, make a small object with the name and the total of its charges
          .OrderByDescending(x=> x.Total) //biggest spender first
          .Take(3); //keep the top 3
          // turn each spender into "Name Rs.total" and join them with commas
          Console.WriteLine("9. Top 3 spenders: " + string.Join(", ", topSpenders.Select(x=>$"{x.Name} Rs. {x.Total:N2}")));

      // 10. Query syntax: big data sessions(over 950 MB), sorted, the same as method syntax
      var bigData = from u in usage // // go through every record u
                        where u.Type == "Data" && u.Quantity >950
                        orderby u.Quantity descending //keep data sessions over 950 MB
                        select u.Quantity; //keep just the MB number
         // Count() and First() each run the query (deferred execution: it runs when used)
          Console.WriteLine($"10. Data sessions over 950 MB:{bigData.Count()} (largest{bigData.First()} MB)");
        
         // a record = a short class that only holds data
          record Customer (string Mobile, string Name); // a customer
          record Usage(string Mobile, string Type, int Quantity, decimal Charge); // one usage record

      

