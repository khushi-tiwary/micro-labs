using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.SqlClient;

namespace ThursdaySecureAccess
{
    // runs the week-03 SQL scripts from C#, 
    static class DbSetup
    {
        const string ScriptFolder = @"C:\Users\user\Source\Repos\TeleBill\week-03\sql\";
        const string MasterConnStr = @"Server=(localdb)\MSSQLLocalDB; Database= master; Trusted_Connection=True;";
        public static void Run()
        {
            RunScript("03_views_functions.sql");
            RunScript("04_rate_usage.sql");
            Console.WriteLine("Procedures ready.\n");
        }
        static void RunScript(string fileName)
        {
            string script = File.ReadAllText(Path.Combine(ScriptFolder, fileName));

            //GO is an SSMS separator, not sql, so split the script into batches of line that say GO
            var batches = new System.Collections.Generic.List<string>();
            var current = new System.Text.StringBuilder();
            foreach (string line in script.Split('\n'))
            {
                if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
                {
                    batches.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.AppendLine(line);
                }
            }
            batches.Add(current.ToString()); //last batch after the final GI

            using (var conn = new SqlConnection(MasterConnStr))
            {
                conn.InfoMessage += (s, e) => Console.WriteLine(e.Message);
                conn.Open();

                foreach (string batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch)) continue;
                    using (var cmd = new SqlCommand(batch, conn))
                    {
                        cmd.CommandTimeout = 300; //seeding 120,000 rows can take a while
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            Console.WriteLine($"Ran{fileName}");
        }
    }

}


