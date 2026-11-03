using Microsoft.Data.SqlClient; // brings in the DO.NET classes for SQL Server:

var connStr = @"Server=(localdb)\MSSQLLocalDB;Database=TeleBill;Trusted_Connection=True;TrustServerCertificate= True"; //connection string, @-verbatim string so \ isn't treated as an escape character
var customers = new List<Customer>();// empty list that holds one customer per row from table

// 1. READ (SELECT)
//using= when this block ends, conn.Dispose() is called automatically,which closes the connection(even if an exception happens).
using (var conn = new SqlConnection(connStr))
{
    conn.Open();// opens the connection to SQL server
    //sql query to run. Cols are listed explicitly
    var sql = "SELECT CustomerId, FullName, Email, Phone, City, CreatedAt FROM dbo.Customers";

    //SqlCommand-"this SQL, on this connection".
    using var cmd = new SqlCommand(sql, conn);

    //Run a query that returns MANY rows. The reader is a forward only cursor- reads one row at a time
    using var reader = cmd.ExecuteReader();

    while(reader.Read()) //returns true if there was a row
    {
        //Manual mapping
        customers.Add(new Customer
        {
            CustomerId = reader.GetInt32(0), //read col 0 (CustomerId) as int
            FullName = reader.GetString(1),// read col1 as string
            Email = reader.IsDBNull(2) ? null: reader.GetString(2),// email allows null in the table
            Phone = reader.GetString(3),
            City = reader.IsDBNull(4) ? null: reader.GetString(4),// city also allows null
            CreatedAt= reader.GetDateTime(5),
        });
    }

}
foreach (var c in customers) // printing each customer
    Console.WriteLine($"{c.CustomerId} | {c.FullName} | {c.Phone} | {c.City ?? "-"}");

Console.WriteLine("\nInserting new customer ");
try
{
    // a new connection for the insert
    using var conn2 = new SqlConnection(connStr);
    conn2.Open();

    //@FullName, @ Email,....are parameters(placeholders), not actual values
    var insertSql = @"INSERT INTO dbo.Customers(FullName, Email, Phone, City)
                     OUTPUT INSERTED.CustomerId
                     VALUES (@FullName, @Email, @Phone, @City);";

    //Fill in the parameters, values are sent seperately from the sql text, so sql doesnt treat them as code
    using var insertCmd = new SqlCommand(insertSql, conn2);
    insertCmd.Parameters.AddWithValue("@FullName", "Test User");
    insertCmd.Parameters.AddWithValue("@Email", "test@telebill.com");
    insertCmd.Parameters.AddWithValue("@Phone", "9000000001");
    insertCmd.Parameters.AddWithValue("@City", "Bengaluru");

    //Executescaler- run a command and return just one value. It returns object, so we cast it to int.
    int newId = (int)insertCmd.ExecuteScalar();
    Console.WriteLine($"Inserted! New CustomerId ={newId}");
}
catch(SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
{
    Console.WriteLine("That phone number already exists.");
}
