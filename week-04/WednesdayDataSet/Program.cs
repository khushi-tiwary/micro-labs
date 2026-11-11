using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace WednesdayDataSet
{
    class Program
    {
        const string ConnStr = @"Server= (localdb)\MSSQLLocalDB; Database= TeleBillDB; Trusted_Connection=True;";
        static void Main(string[] args)
        {
            DbSetup.Run(); //run once to build TeleBillDB

            try
            {
                // Part-1:DataSet+ DataRelation
                DataSet ds = new DataSet("TeleBill");

                using (var custAdapter = new SqlDataAdapter("SELECT CustomerId, FullName, Phone, City FROM dbo.Customers", ConnStr))
                using (var subAdapter = new SqlDataAdapter("SELECT SubscriptionId, CustomerId, PlanId, Status FROM dbo.Subscriptions", ConnStr))
                {
                    custAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey; //also load the primary key
                    subAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                    custAdapter.Fill(ds, "Customers"); //opens connection, copies rows, closes it
                    subAdapter.Fill(ds, "Subscriptions");

                    DataTable customers = ds.Tables["Customers"];
                    DataTable subscriptions = ds.Tables["subscriptions"];
                    Console.WriteLine($"Loaded {customers.Rows.Count} customers and {subscriptions.Rows.Count} subscriptions (connection is now closed).");

                    //DataRelation = a foreign key held in memory
                    DataRelation rel = ds.Relations.Add("CustomerSubscriptions",
                        customers.Columns["CustomeriD"],
                        subscriptions.Columns["CustomerId"]);

                    foreach (DataRow cust in customers.Rows.Cast<DataRow>().Take(5))
                    {
                        Console.WriteLine($"\n{cust["CustomerId"]}  {cust["FullName"]}");
                        foreach (DataRow sub in cust.GetChildRows(rel))
                            Console.WriteLine($"  Subscription{sub["SubscriptionId"]}: plan{sub["PlanId"]}, {sub["Status"]}");
                    }

                    //Part-2: Update through the adapter
                    using (var builder = new SqlCommandBuilder(custAdapter)) // writes the UPDATE SQL for us
                    {
                        DataRow first = customers.Rows.Find(1000); // works because the primary key was loaded
                        Console.WriteLine($"\nCustomer 1000 city before: {first["City"]}");
                        first["City"] = "Pune";
                        Console.WriteLine($"Row state: {first.RowState}");

                        custAdapter.UpdateBatchSize = 50;
                        int saved = custAdapter.Update(customers);
                        Console.WriteLine($"Adapter.Update saved {saved} row(s). Row State now: {first.RowState}");
                    }
                }

                //Part-3: Two-table insertion ina SqlTransaction
                Console.WriteLine("\n Transaction that FAILS (PlanId 999 does not exist)");
                string badPhone = "7" + DateTime.Now.ToString("HHmmssfff");
                InsertCustomerWithSubscription("Fail Test", badPhone, 999);
                Console.WriteLine($"Customer with phone {badPhone} saved? {CustomerExists(badPhone)}"); // FALSE

                Console.WriteLine("\n Transaction that SUCCEEDS");
                string goodPhone = "6" + DateTime.Now.ToString("HHmmssfff");
                InsertCustomerWithSubscription("Pass TTest", goodPhone, 2);
                Console.WriteLine($"Customer with phone {goodPhone} saved? {CustomerExists(goodPhone)}"); //True
            }

            catch (SqlException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
         // Inserts a customer AND a subscription: both succeed or neither does
         static void InsertCustomerWithSubscription(string name, string phone, int planId)
        {
            using (var conn= new SqlConnection(ConnStr))
                {
                conn.Open();
                using (SqlTransaction tx= conn.BeginTransaction())
                {
                    try
                    {
                        int newId;
                        using (var cmd1 = new SqlCommand(
                            "INSERT INTO dbo.Customers(FullName, Phone, City) OUTPUT INSERTED.CustomerId VALUES(@Name, @Phone, N'Bengaluru');",
                            conn, tx))  // tx: part of the transaction
                        {
                            cmd1.Parameters.AddWithValue("@Name", name);
                            cmd1.Parameters.AddWithValue("@Phone", phone);
                            newId = (int)cmd1.ExecuteScalar();
                        }
                        Console.WriteLine($"Inserted customer {newId} (not commited yet)");
                        using (var cmd2 = new SqlCommand(
                             "INSERT INTO dbo.Subscriptions (CustomerId, PlanId, Status) VALUES(@CustomerId, @PlanId, 'Active');",
                             conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("@CustomerId", newId);
                            cmd2.Parameters.AddWithValue("@PlanId", planId);
                            cmd2.ExecuteNonQuery();
                        }
                        tx.Commit();
                        Console.WriteLine("Committed both inserts.");
                    }

                    catch(SqlException ex)
                    {
                        tx.Rollback();
                        Console.WriteLine($"Rolled back. Reason: {ex.Message}");
                    }

                            
                    }
                }
            }

        static bool CustomerExists(string phone)
        {
            using (var conn= new SqlConnection(ConnStr))
            using (var cmd= new SqlCommand("SELECT COUNT(*) FROM dbo.Customers WHERE Phone = @Phone", conn))
            {
                cmd.Parameters.AddWithValue("@Phone", phone);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }

          }
        }
      
    }

