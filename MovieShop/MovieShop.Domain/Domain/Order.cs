using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Identity;

namespace MovieShop.Domain.Domain
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public MovieApplicationUser User { get; set; }
        public virtual ICollection<OrderTicket> OrderTicket { get; set; }
    }
}
