using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Domain;

namespace MovieShop.Domain.DTO
{
    public class ShoppingCartDTO
    {
        public List<TicketInShoppingCart> TicketInShoppingCart { get; set; }
        public double Total { get; set; }
    }
}
