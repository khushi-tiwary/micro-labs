using System.Security.Cryptography.X509Certificates;

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = "";
    public string? Email { get; set; }
    public string Phone { get; set; } = "";
    public string ? City { get; set; }
    public DateTime CreatedAt { get; set; }
}