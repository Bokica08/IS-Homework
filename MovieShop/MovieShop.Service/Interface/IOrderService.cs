using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Domain;

namespace MovieShop.Service.Implementation
{
    public interface IOrderService
    {
        public List<Order> getAllOrders();
        public Order getOrderDetails(BaseEntity model);
        List<Order> getOrdersByUserId(string userId);
    }
}
