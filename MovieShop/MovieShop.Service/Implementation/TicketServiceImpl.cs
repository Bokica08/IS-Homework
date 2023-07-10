using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MovieShop.Domain.Domain;
using MovieShop.Domain.DTO;
using MovieShop.Domain.Identity;
using MovieShop.Repository.Interface;
using MovieShop.Service.Implementation;

namespace MovieShop.Service.Interface
{
    public class TicketServiceImpl : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketinShoppingCartRepository;
        private readonly IUserRepository _userRepository;
        public TicketServiceImpl(IRepository<Ticket> ticketRepository, IRepository<TicketInShoppingCart> ticketinShoppingCartRepository, IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _ticketinShoppingCartRepository = ticketinShoppingCartRepository;
            _userRepository = userRepository;
        }
        public bool AddToShoppingCart(AddedToShoppingCartDTO item, string userID)
        {
            var user = this._userRepository.Get(userID);
            var userShoppingCard = user.UserCart;

            if (item.TicketId != null && userShoppingCard != null)
            {
                var ticket = this.GetDetailsForTicket(item.TicketId);
                if (ticket != null)
                {
                    TicketInShoppingCart itemToAdd = new TicketInShoppingCart
                    {
                        Id = Guid.NewGuid(),
                        Ticket = ticket,
                        TicketId = ticket.Id,
                        Shoppingcart = userShoppingCard,
                        ShoppingCartId = userShoppingCard.Id,
                        Quantity = item.Quantity
                    };
                    this._ticketinShoppingCartRepository.Insert(itemToAdd);
                    return true;
                }
                return false;
            }
            return false;
        }

        public void CreateNewTicket(Ticket t)
        {
            this._ticketRepository.Insert(t);
        }

        public void DeleteTicket(Guid id)
        {
            var ticket = this.GetDetailsForTicket(id);
            this._ticketRepository.Delete(ticket);
        }

        public List<Ticket> GetAllTickets()
        {
            return this._ticketRepository.listAll().ToList();
        }

        public Ticket GetDetailsForTicket(Guid? id)
        {
            return this._ticketRepository.Get(id);
        }

        public AddedToShoppingCartDTO GetShoppingCartInfo(Guid? id)
        {
            var ticket = this.GetDetailsForTicket(id);
            AddedToShoppingCartDTO model = new AddedToShoppingCartDTO
            {
                SelectedTicket = ticket,
                TicketId = ticket.Id,
                Quantity = 1
            };
            return model;
        }

        public void UpdeteExistingTicket(Ticket t)
        {
            this._ticketRepository.Update(t);
        }
        public IEnumerable<Ticket> GetAllTickets(DateTime? dateFilter)
        {
            if (dateFilter.HasValue)
            {
                return _ticketRepository.GetAllTickets(dateFilter.Value);
            }

            return this._ticketRepository.listAll().ToList();
        }
    }
}
