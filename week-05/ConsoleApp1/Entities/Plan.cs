
using System;

namespace Thursday_micro_lab.Entities
{
    public class Plan
    {
        // Unique ID of the plan.
        // This corresponds to the SQL Server primary key.
        public int PlanId { get; set; }

        // Stores the name of the telecom plan.
        public string PlanName { get; set; }

        // Stores the monthly subscription price.
        public decimal MonthlyPrice { get; set; }

        // Stores the SQL Server RowVersion value.
        // SQL Server automatically generates this value.
        public byte[] RowVersion { get; set; }
    }
}
