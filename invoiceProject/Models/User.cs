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
        [Display(Name = "חובה להזין את מספר הלקוח")]
        public int UserID { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage ="חובה להזין שם פרטי")]
        [RegularExpression(@"^[A-Za-zא-ת\s]*$")]
        [Display(Name = "שם פרטי")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "חובה להזין שם משפחה")]
        [Display(Name = "שם משפחה")]
        [RegularExpression(@"^[A-Za-zא-ת\s]*$")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "חובה להזין שם משתמש")]
        [Display(Name = "שם משתמש")]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z0-9א-ת\s]*$")]
        public string UserName { get; set; }

        [Display(Name = "סיסמה")]
        [Required(ErrorMessage = "חובה להזין סיסמה")]
        [RegularExpression(@"^[A-Za-z0-9א-ת\s]*$")]
        public string Password { get; set; }

        [Display(Name = "אדמין")]
        public bool IsAdmin { get; set; }

        [Display(Name = "אימייל")]
        [Required(ErrorMessage = "חובה להזין אימייל")]
        public String Email { get; set; }

        [Display(Name = "תאריך הצטרפות")]
        //[DataType(DataType.Date)]
        [Required(ErrorMessage = "חובה להזין תאריך הצטרפות")]
        public DateTime EnteranceDate { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Credit> Credit_Invoices { get; set; }

        public ICollection<UserGiftCard> userGiftCard { get; set; }


    }
}
