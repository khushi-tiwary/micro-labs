
using System;                        
using System.Linq;                    
using Microsoft.EntityFrameworkCore;  // Provides Entity Framework Core functionality

namespace OffersManagement            
{
    class Program                     
    {
        static void Main(string[] args) 
        {
            using (var context = new AppDbContext()) // Creates the database context and disposes it after use
            {
                // TASK 3: LINQ SORTING
                   var offers = context.Offers           // Accesses the Offers table in the database
                    .OrderByDescending(o => o.Discount) // Sorts offers by Discount from highest to lowest
                    .ToList();                        // Executes the query and stores results in a list

                // DISPLAY RESULTS

                Console.WriteLine("Offers sorted by highest discount:"); 

                foreach (var offer in offers)         // Iterates through each sorted offer
                {
                    Console.WriteLine(                
                        offer.OfferName + " - " +    
                        offer.Discount + "%"         
                    );
                }

                // TASK 3: ASCENDING SORTING

                var ascendingOffers = context.Offers  // Accesses the Offers table again
                    .OrderBy(o => o.Discount)         // Sorts offers from lowest to highest discount
                    .ToList();                        // Executes the query and stores results in a list

                Console.WriteLine("\nOffers sorted by lowest discount:"); 

                foreach (var offer in ascendingOffers) // Iterates through ascending sorted offers
                {
                    Console.WriteLine(               
                        offer.OfferName + " - " +    
                        offer.Discount + "%"         
                    );
                }
            }                                         

            Console.ReadLine(); // Waits for user input before closing the console
        }
    }
}
