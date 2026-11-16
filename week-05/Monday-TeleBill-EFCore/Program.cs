
using System;
using System.Linq;
using MonTeleBillEFCoreDemo.Models;

namespace MonTeleBillEFCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new TeleBillContext())
            {
                Console.WriteLine("TELEBILL CUSTOMER DETAILS");

                var customers = db.Customers.ToList();

                foreach (var customer in customers)
                {
                    Console.WriteLine("Customer ID: " + customer.CustomerId);
                    Console.WriteLine("Name: " + customer.CustomerName);
                    Console.WriteLine("Phone: " + customer.PhoneNumber);
                    Console.WriteLine("Email: " + customer.Email);

                    
                }
            }

            Console.ReadLine();
        }
    }
}
