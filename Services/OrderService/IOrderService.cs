using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Model.View_Model;

namespace Services.OrderService
{
    public interface IOrderService
    {
        public  Task<Order> makeOrder(Order order);
        public Task<List<Order>> ActiveOrders(string ResturantId);
        public Task<OrderDetail> GetFullOrder(string orderId);
        public Task<Order> makePayment(String orderId);
    }
}
