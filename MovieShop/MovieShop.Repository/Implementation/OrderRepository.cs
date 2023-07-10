using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MovieShop.Domain.Domain;
using MovieShop.Repository.Interface;

namespace MovieShop.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }
        public List<Order> listAll()
        {
            return entities
                .Include(z => z.OrderTicket)
                .Include(z => z.User)
                .Include("OrderTicket.SelectedTicket")
                .ToListAsync().Result;
        }

        public Order getOrderDetails(BaseEntity model)
        {
            return entities
                .Include(z => z.OrderTicket)
                .Include(z => z.User)
                .Include("OrderTicket.SelectedTicket")
                .SingleOrDefaultAsync(z => z.Id == model.Id).Result;
        }
        public List<Order> getOrdersByUserId(string userId)
        {
            return entities
                .Include(o => o.OrderTicket)
                .Include("OrderTicket.SelectedTicket")
                .Where(o => o.UserId == userId)
                .ToList();
        }
    }
}
