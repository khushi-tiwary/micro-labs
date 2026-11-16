using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MonTeleBillEFCoreDemo.Models
{
    public partial class Customers
    {
        public Customers()
        {
            Bills = new HashSet<Bills>();
            Subscriptions = new HashSet<Subscriptions>();
        }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Bills> Bills { get; set; }
        public virtual ICollection<Subscriptions> Subscriptions { get; set; }
    }
}
