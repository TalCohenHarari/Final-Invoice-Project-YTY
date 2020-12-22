using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class Invoice
    {
        public int UserID { get; set; }

        public int InvoiceID { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "חובה להזין את שם החנות")]
        [RegularExpression(@"^[A-Za-z0-9\s]*$")]
        [Display(Name = "שם החנות")]
        public string StoreName { get; set; }
        [Required(ErrorMessage = "חובה להזין תאריך קנייה")]
        [Display(Name = "תאריך קנייה")]
        public DateTime PurchaseDate { get; set; }
        [Required(ErrorMessage = "חובה להזין סכום")]
        [Display(Name = "סכום")]
        public double Amount { get; set; }

        public int CategoryID { get; set; }
        [Required(ErrorMessage = "חובה להזין את תוקף החשבונית")]
        [Display(Name = "תוקף חשבונית")]
        public DateTime ExpireDate { get; set; }

        public User user { get; set; }
    }
}
