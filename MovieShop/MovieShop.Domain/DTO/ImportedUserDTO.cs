using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Domain.DTO
{
    public class ImportedUserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
