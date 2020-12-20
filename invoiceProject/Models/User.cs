using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class User
    {

        
        public int UserID { get; set; }

        [StringLength(50)]
        [Required]
        [RegularExpression(@"^[A-Za-z\s]*$")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[A-Za-z\s]*$")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z0-9\s]*$")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required]
        [RegularExpression(@"^[A-Za-z0-9\s]*$")]
        public string Password { get; set; }

        [Display(Name = "Admin User")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Email Address")]
        public String Email { get; set; }

        [Display(Name = "Entrance Date")]
        [DataType(DataType.Date)]
        public DateTime EnteranceDate { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Credit> Credit_Invoices { get; set; }

        public ICollection<UserGiftCard> userGiftCard { get; set; }


    }
}
