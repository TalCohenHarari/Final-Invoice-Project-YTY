using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class UserGiftCard
    {
        [Display(Name = "מספר הלקוח")]
        public int UserID { get; set; }

        public User user { get; set; }

        [Display(Name = "מספר הגיפט-קארד")]
        public int GiftCardID { get; set; }

        public GiftCard giftCard { get; set; }

        
        [Display(Name = "כמות")]
        public int Count { get; set; }


    }
}
