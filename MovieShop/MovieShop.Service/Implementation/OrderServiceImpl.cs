using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Domain;
using MovieShop.Repository.Interface;
using MovieShop.Service.Implementation;

namespace MovieShop.Service.Interface
{
    public class OrderServiceImpl : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderServiceImpl(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository;
        }
        public List<Order> getAllOrders()
        {
            return _orderRepository.listAll();
        }

        public Order getOrderDetails(BaseEntity model)
        {
            return _orderRepository.getOrderDetails(model);
        }
        public List<Order> getOrdersByUserId(string userId)
        {
            return _orderRepository.getOrdersByUserId(userId);
        }

    }
}
