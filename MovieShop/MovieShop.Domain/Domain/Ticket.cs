using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieShop.Domain.Domain
{
    public class Ticket : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Place { get; set; }

        public string Url { get; set; }
        public string Genre { get; set; }
        public int Price { get; set; }

        public virtual ICollection<TicketInShoppingCart> Ticketsinshoppingcart { get; set; }
        public virtual ICollection<OrderTicket> Orders { get; set; }


    }
}
