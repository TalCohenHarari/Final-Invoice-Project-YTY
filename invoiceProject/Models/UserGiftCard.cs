using System;
using System.Collections.Generic;
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

        public double price { get; set; }


    }
}
