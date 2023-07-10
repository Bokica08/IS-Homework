using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using MovieShop.Domain.Domain;

namespace MovieShop.Domain.Identity
{
    public class MovieApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public virtual ShoppingCart UserCart { get; set; }
    }
}
