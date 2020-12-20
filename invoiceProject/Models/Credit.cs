using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class Credit
    {
        public int UserID { get; set; }

        public int CreditID { get; set; }

        public string StoreName { get; set; }

        public double Amount { get; set; }

        public DateTime ExpireDate { get; set; }

        public User user { get; set; }
    }
}
