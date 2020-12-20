using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class Invoice
    {
        public int UserID { get; set; }

        public int InvoiceID { get; set; }

        public string StoreName { get; set; }

        public DateTime PurchaseDate { get; set; }

        public double Amount { get; set; }

        public int CategoryID { get; set; }

        public DateTime ExpireDate { get; set; }

        public User user { get; set; }
    }
}
