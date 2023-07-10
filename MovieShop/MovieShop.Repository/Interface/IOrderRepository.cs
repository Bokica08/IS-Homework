using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Domain;

namespace MovieShop.Repository.Interface
{
    public interface IOrderRepository
    {
        List<Order> listAll();
        Order getOrderDetails(BaseEntity model);
        List<Order> getOrdersByUserId(string userId);
    }
}
