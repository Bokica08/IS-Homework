using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Domain;

namespace MovieShop.Domain.DTO
{
    public class AddedToShoppingCartDTO
    {
        public Guid TicketId { get; set; }
        public int Quantity { get; set; }
        public Ticket SelectedTicket { get; set; }

    }
}
