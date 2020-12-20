using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace invoiceProject.Models
{
    public class GiftCard
    {
        public int GiftCardID { get; set; }

        public string GiftCardName { get; set; }

        public ICollection<UserGiftCard> userGiftCard { get; set; }
    }
}
