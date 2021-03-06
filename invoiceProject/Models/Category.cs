﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }
        public ICollection<Credit> Credit { get; set; }
        public ICollection<Invoice> Invoice { get; set; }
    }
}
