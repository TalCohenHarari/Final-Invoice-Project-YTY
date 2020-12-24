using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class UserGiftCard
    {
        public int UserID { get; set; }

        public User user { get; set; }

        public int GiftCardID { get; set; }

        public GiftCard giftCard { get; set; }

        [Required(ErrorMessage = "חובה להזין סכום")]
        [Display(Name = "סכום")]
        public double price { get; set; }


    }
}
