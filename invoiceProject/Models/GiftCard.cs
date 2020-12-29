using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class GiftCard
    {
        public int GiftCardID { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "חובה לבחור את סוג הגיפט-קארד")]
        [RegularExpression(@"^[&A-Za-z0-9א-ת\s]*$")]
        [Display(Name = "סוג הגיפט-קארד")]
        public string GiftCardName { get; set; }

        [Required(ErrorMessage = "חובה להזין את סכום הגיפט-קארד")]
        [Display(Name = "סכום")]
        public double Price { get; set; }

        [Required(ErrorMessage = "חובה להזין את תוקף הגיפט-קארד")]
        [Display(Name = "תוקף גיפט-קארד")]
        public DateTime ExpireDate { get; set; }

        public ICollection<UserGiftCard> userGiftCard { get; set; }

        public GiftCard(string GiftCardName, double Price)
        {
            this.GiftCardName = GiftCardName;
            this.Price = Price;
        }
        public GiftCard() { }
    }
}
