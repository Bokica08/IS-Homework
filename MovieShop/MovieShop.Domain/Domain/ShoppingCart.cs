using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Identity;

namespace MovieShop.Domain.Domain
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; }
        public MovieApplicationUser Owner { get; set; }
        public virtual ICollection<TicketInShoppingCart> Tickets { get; set; }
    }
}
