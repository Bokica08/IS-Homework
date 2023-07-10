using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Identity;

namespace MovieShop.Domain.DTO
{
    public class UserWithRoleDTO
    {
        public MovieApplicationUser User { get; set; }
        public bool IsAdmin { get; set; }
    }
}
