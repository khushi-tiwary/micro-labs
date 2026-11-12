using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ThursdaySecureAccess
{
    class Program
    {
        //Read from App.config: no connection string hard-coded here
        static readonly string ConnStr = ConfigurationManager.ConnectionStrings["TeleBillDB"].ConnectionString;

        static async Task Main(string[] args) //async all the way up, starting at Main
        {
            //DbSetup.Run(); //run ONCE

            try
            {
                Console.WriteLine("1. VULNERABLE search(string concatenation)");
                VulnerableSearch("Sharma");
                VulnerableSearch("zzz' OR 1=1 --");
                VulnerableSearch("zzz' UNION SELECT PlanId, PlanName, PlanCode FROM dbo.Plans --");

                Console.WriteLine("2. SAFE search (parameters+ async)");
                await SafeSearchAsync("Sharma");
                await SafeSearchAsync("zzz' OR 1=1 --");

                Console.WriteLine("3. Call usp_RateUsage from C#");
                await RateUsageAsync(new DateTime (2026, 8, 1)); //bills August
                await RateUsageAsync(new DateTime(2026, 8, 1)); //run again: 0 rows billed (safe to re-run)
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
            }
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        //DON'T DO THIS:user input is glued into the SQL text
        static void VulnerableSearch(string name)
        {
            string sql= "SELECT CustomerId, FullName, Phone FROM dbo.Customers WHERE FullName LIKE '%" + name + "%'";
            Console.WriteLine($"\nInput: {name}");
            Console.WriteLine($"SQL sent: {sql}");

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int count = 0;
                    while(reader.Read())
                    {
                        if (count < 5)  //print the first 5 only
                            Console.WriteLine($"    {reader.GetInt32(0)} {reader.GetString(1)} {reader.GetString(2)}");
                        count++;
                    }
                    Console.WriteLine($"    ->{count} row(s) returned");
                }
            }
        }
        //SAFE: the input is a parameter, so SQL Server treats it as data only
        static async Task SafeSearchAsync(string name)
        {
            const string sql = "SELECT CustomerId, FullName, Phone FROM dbo.Customers WHERE FullName LIKE @Search";
            Console.WriteLine($"\nInput: {name}");

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value = "%" + name + "%";

                await conn.OpenAsync(); //async open
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())  //async query
                {
                    int count = 0;
                    while (await reader.ReadAsync())
                    {
                        if (count < 5)
                            Console.WriteLine($"  {reader.GetInt32(0)}  {reader.GetString(1)}  {reader.GetString(2)}");
                        count++;
                    }
                    Console.WriteLine($"   ->{count} row(s) returned");

                }
            }
        }
        //Calls the stored procedurefrom week-3 with an input parameter and reads te return value
        static async Task RateUsageAsync(DateTime month)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("dbo.usp_RateUsage", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;

                cmd.Parameters.Add("@BillingMonth", SqlDbType.Date).Value = month; //input parameter

                SqlParameter returnValue = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                returnValue.Direction = ParameterDirection.ReturnValue;

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync()) //the summary row the procedure SELECTs
                        Console.WriteLine($"Billed {reader.GetDateTime(0):MMM yyyy}:{reader.GetInt32(1)} usage rows, {reader.GetInt32(2)} invoices for the month");
                }
                //return value
                Console.WriteLine($"Return code: {returnValue.Value}");
            }
        }
        
    }
}
