using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MonTeleBillEFCoreDemo.Models
{
    public partial class Bills
    {
        public int BillId { get; set; }
        public int CustomerId { get; set; }
        public decimal BillAmount { get; set; }
        public DateTime BillDate { get; set; }

        public virtual Customers Customer { get; set; }
    }
}
