using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MovieShop.Domain.Domain;
using MovieShop.Domain.DTO;
using MovieShop.Repository.Interface;
using MovieShop.Service.Implementation;

namespace MovieShop.Service.Interface
{
    public class ShoppingCartServiceImpl : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<EmailMessage> _mailRepository;
        private readonly IRepository<OrderTicket> _orderTicketRepository;
        private readonly IUserRepository _userRepository;
        public ShoppingCartServiceImpl(IRepository<ShoppingCart> shoppingCartRepository, IRepository<EmailMessage> mailRepository, IRepository<Order> orderRepository, IRepository<OrderTicket> orderTicketRepository, IUserRepository userRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _orderRepository = orderRepository;
            _mailRepository = mailRepository;
            _orderTicketRepository = orderTicketRepository;
            _userRepository = userRepository;
        }

        public void deleteTicketFromShoppingCart(String userId, Guid id)
        {
            if (!string.IsNullOrEmpty(userId) && id != null)
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userShoppingCart = loggedInUser.UserCart;
                var producttodelete = userShoppingCart.Tickets.Where(z => z.TicketId.Equals(id)).FirstOrDefault();
                userShoppingCart.Tickets.Remove(producttodelete);
                this._shoppingCartRepository.Update(userShoppingCart);
            }

        }

        public ShoppingCartDTO getShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);
            var userShoppingCart = loggedInUser.UserCart;
            if(userShoppingCart == null)
            {
                return null;
            }
            var ticketPrice = userShoppingCart.Tickets.Select(z => new
            {
                ticketPrice = z.Ticket.Price,
                Quantity = z.Quantity

            }).ToList();
            double totalPrice = 0;
            foreach (var item in ticketPrice)
            {
                totalPrice = totalPrice + item.ticketPrice * item.Quantity;
            }
            ShoppingCartDTO shopingcartdtoitem = new ShoppingCartDTO
            {
                TicketInShoppingCart = userShoppingCart.Tickets.ToList(),
                Total = totalPrice
            };
            return shopingcartdtoitem;
        }

        public bool orderNow(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userShoppingCart = loggedInUser.UserCart;
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.MailTo = loggedInUser.Email;
                emailMessage.Subject = "Succesfull order";
                emailMessage.Status = false;
                Order orderItem = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    User = loggedInUser
                };
                this._orderRepository.Insert(orderItem);
                List<OrderTicket> ticketInOrders = new List<OrderTicket>();
                ticketInOrders = userShoppingCart.Tickets.Select(z => new OrderTicket
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderItem.Id,
                    TicketId = z.Ticket.Id,
                    SelectedTicket = z.Ticket,
                    UserOrder = orderItem,
                    Quantity = z.Quantity
                }).ToList();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Order is completed it contains");
                var totalPrice = 0;
                for (int i = 1; i <= ticketInOrders.Count(); i++)
                {
                    var item = ticketInOrders[i - 1];
                    totalPrice = item.Quantity * item.SelectedTicket.Price;
                    sb.AppendLine(i.ToString() + " " + item.SelectedTicket.Name + " price: " + item.SelectedTicket.Price + " quantity: " + item.Quantity);
                }
                sb.AppendLine("total price" + totalPrice.ToString());
                emailMessage.Content = sb.ToString();
                foreach (var item in ticketInOrders)
                {
                    this._orderTicketRepository.Insert(item);
                }
                loggedInUser.UserCart.Tickets.Clear();
                this._mailRepository.Insert(emailMessage);
                this._userRepository.Update(loggedInUser);
                return true;
            }
            return false;
        }
    }
}
