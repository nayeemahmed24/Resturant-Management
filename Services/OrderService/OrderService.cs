using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Entities;
using Model.View_Model;
using Repository;
using Services.AddonServices;
using Services.MenuServices;

namespace Services.OrderService
{
    public class OrderService : IOrderService
    {
        private IMongoRepository _repository;
        private IMenuServices _menuServices;
        private IAddonService _addonService;
        public OrderService(IAddonService addonService,IMongoRepository repository,IMenuServices menuServices)
        {
            _addonService = addonService;
            _menuServices = menuServices;
            _repository = repository;
        }

        public async Task<Order> makeOrder(Order order)
        {
            if (order.Items != null)
            {
                await _repository.SaveAsync<Order>(order);
                return order;
            }

            return null;
        }
        public async Task<Order> makePayment(String orderId)
        {
            if (orderId != null)
            {
                var order = await FindOrderbyId(orderId);
                order.Paid = true;
                await _repository.UpdateAsync<Order>(d => d.Id == order.Id, order);
                return order;
            }

            return null;
        }


        public async Task<OrderDetail> GetFullOrder(string orderId)
        {
            if (orderId != null)
            {
                var order = await FindOrderbyId(orderId);
                if (order != null)
                {
                    var orderDetail = await BuildOrderDetail(order);
                    return orderDetail;
                }
            }

            return null;
        }

        public async Task<List<Order>> ActiveOrders(string ResturantId)
        {
            var res = await _repository.GetItemsAsync<Order>(d =>
                d.ResturantId == ResturantId && d.Status == OrderStatus.Received);
            var list = res?.ToList();
            return list;
        }

        public async Task<Order> FindOrderbyId(string orderId)
        {
            return await _repository.GetItemAsync<Order>(d => d.Id == orderId);
        }


        public async Task<OrderDetail> BuildOrderDetail(Order order)
        {
            var OrderDetail = new OrderDetail();
            OrderDetail.Id = order.Id;
            OrderDetail.ResturantId = order.ResturantId;
            OrderDetail.Status = order.Status;
            OrderDetail.Items = new List<OrderUnitView>();
            
            foreach (var menuItem in order.Items)
            {
                var menu = await _menuServices.FindMenuItemById(menuItem.MenuItemId);

                if (menu != null)
                {
                    var resMenu = new OrderUnitView();
                    resMenu.Addons = new List<AddonUnitView>();
                    resMenu.MenuItem = menu;
                    if (menuItem.Addons != null)
                    {
                        foreach (var addons in menuItem.Addons)
                        {
                            var addon = await _addonService.FindAddonById(addons.Id);
                            if (addon != null)
                            {
                                var resAddon = new AddonUnitView();
                                resAddon.Addon = addon;
                                resAddon.Quantity = addons.Quantity;
                                resMenu.Addons.Add(resAddon);
                            }
                        }
                        
                    }
                    OrderDetail.Items.Add(resMenu);
                }

            }

            return OrderDetail;
        }
    }
}
