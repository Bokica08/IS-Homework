using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Domain.Domain
{
    public class OrderTicket : BaseEntity
    {
        public Guid TicketId { get; set; }
        public Order UserOrder { get; set; }
        public Guid OrderId { get; set; }
        public Ticket SelectedTicket { get; set; }
        public int Quantity { get; set; }
    }
}
