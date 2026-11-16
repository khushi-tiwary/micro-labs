using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MonTeleBillEFCoreDemo.Models
{
    public partial class Subscriptions
    {
        public int SubscriptionId { get; set; }
        public int CustomerId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }

        public virtual Customers Customer { get; set; }
        public virtual Plans Plan { get; set; }
    }
}
