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
        
        public Task<OrderDetail> GetFullOrder(string orderId);
        public Task<Order> makePayment(String orderId);
        Task<List<Order>> ReceivedOrders(string ResturantId);
        public Task<List<OrderDetail>> ProcessingOrders(string ResturantId);
        Task<Order> MakeReady(string OrderId);
        Task<Order> MakeProcessing(string OrderId);
        Task<SoldQuantity> FindTotalSellByItemType(string itemType, string resturantId);
        Task<ItemTypeAnalysis> AnalysisBasedOnType(DateTime start, DateTime end, string resturantId);
        Task<Order> DeleteOrder(string id);
    }
}
