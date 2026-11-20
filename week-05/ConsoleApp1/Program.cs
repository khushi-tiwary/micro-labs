
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Thursday_micro_lab
{
    class Program
    {
        // Main method is the entry point of the application.
        static void Main(string[] args)
        {
            Console.WriteLine("================================");
            Console.WriteLine(" THURSDAY EF CORE MICRO-LAB");
            Console.WriteLine("================================");

            // TASK 1: Demonstrate the EF Core Change Tracker.
            ChangeTrackerDemo().GetAwaiter().GetResult();

            Console.WriteLine();


            static async Task ConcurrencyDemo()
            {
                Console.WriteLine("TASK 2: CONCURRENCY DEMONSTRATION");

                using (var context1 = new AppDbContext())
                using (var context2 = new AppDbContext())
                {
                    // Both users retrieve the same record.
                    var plan1 = await context1.Plans
                        .FirstOrDefaultAsync(p => p.PlanId == 1);

                    var plan2 = await context2.Plans
                        .FirstOrDefaultAsync(p => p.PlanId == 1);

                    if (plan1 == null || plan2 == null)
                    {
                        Console.WriteLine("Plan not found.");
                        return;
                    }

                    // Store the original price.
                    decimal originalPrice = plan1.MonthlyPrice;

                    Console.WriteLine("Both users loaded the same plan.");
                    Console.WriteLine("Original Price: " + originalPrice);

                    // USER A updates the price.
                    plan1.MonthlyPrice = originalPrice + 50;

                    await context1.SaveChangesAsync();

                    Console.WriteLine(
                        "User A updated the price to " + plan1.MonthlyPrice);

                    // USER B attempts to update using an outdated RowVersion.
                    plan2.MonthlyPrice = originalPrice + 100;

                    try
                    {
                        await context2.SaveChangesAsync();

                        Console.WriteLine("User B updated successfully.");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        Console.WriteLine("CONCURRENCY CONFLICT DETECTED!");

                        Console.WriteLine(
                            "User B's changes were rejected.");
                    }
                }

                // Retrieve the final database value.
                using (var finalContext = new AppDbContext())
                {
                    var finalPlan = await finalContext.Plans
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.PlanId == 1);

                    if (finalPlan != null)
                    {
                        Console.WriteLine(
                            "Final Database Price: " +
                            finalPlan.MonthlyPrice);
                    }
                }
            }

        }
    }
