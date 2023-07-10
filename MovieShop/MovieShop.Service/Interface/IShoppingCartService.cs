using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.DTO;

namespace MovieShop.Service.Implementation
{
    public interface IShoppingCartService
    {
        ShoppingCartDTO getShoppingCartInfo(string userId);
        void deleteTicketFromShoppingCart(String userId, Guid id);
        bool orderNow(string userId);
    }
}
