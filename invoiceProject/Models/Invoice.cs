﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class Invoice
    {

        public int InvoiceID { get; set; }

        [Display(Name = "מספר הלקוח")]
        public int UserID { get; set; }
        public User user { get; set; }

        [Required(ErrorMessage = "חובה להזין קטגוריה")]
        [Display(Name = "קטגוריה")]
        public int CategoryID { get; set; }
        public Category Category { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "חובה להזין את שם החנות")]
        [RegularExpression(@"^[A-Za-z0-9א-ת&-_\s]*$")]
        [Display(Name = "שם החנות")]
        public string StoreName { get; set; }

        [Required(ErrorMessage = "חובה להזין תאריך קנייה")]
        [Display(Name = "תאריך קנייה")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "חובה להזין סכום")]
        [Display(Name = "סכום")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "חובה להזין את תוקף האחריות")]
        [Display(Name = "תוקף אחריות")]
        public DateTime ExpireDate { get; set; }
    }
}
