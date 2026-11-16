using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MonTeleBillEFCoreDemo.Models
{
    public partial class Plans
    {
        public Plans()
        {
            Subscriptions = new HashSet<Subscriptions>();
        }

        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public decimal Amount { get; set; }
        public int ValidityDays { get; set; }

        public virtual ICollection<Subscriptions> Subscriptions { get; set; }
    }
}
