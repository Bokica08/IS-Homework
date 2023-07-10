using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Domain.Domain
{
    public class TicketInShoppingCart : BaseEntity
    {
        public Guid TicketId { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Ticket Ticket { get; set; }
        public int Quantity { get; set; }
        public ShoppingCart Shoppingcart { get; set; }
    }
}
