using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Domain;
using MovieShop.Domain.DTO;

namespace MovieShop.Service.Implementation
{
    public interface ITicketService
    {
        List<Ticket> GetAllTickets();
        Ticket GetDetailsForTicket(Guid? id);
        void CreateNewTicket(Ticket t);
        void UpdeteExistingTicket(Ticket t);
        AddedToShoppingCartDTO GetShoppingCartInfo(Guid? id);
        void DeleteTicket(Guid id);
        bool AddToShoppingCart(AddedToShoppingCartDTO item, string userID);
        IEnumerable<Ticket> GetAllTickets(DateTime? dateFilter);
    }
}
