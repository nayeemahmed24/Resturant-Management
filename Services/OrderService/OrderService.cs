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
using Services.UserServices;

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

        public async Task<SoldQuantity> FindTotalSellByItemType(string itemType, string resturantId)
        {
            if (itemType != null && resturantId != null)
            {
                var result = new SoldQuantity();
                result.ItemType = itemType;
                result.Quantity = 0;result.TotalPrice = 0;
                var Orders = await _repository.GetItemsAsync<Order>(d => d.ResturantId == resturantId);
                if (Orders != null)
                {
                    foreach (var order in Orders)
                    {
                        var menuList = order.Items;
                        foreach (var menu in menuList)
                        {
                            var or = menu.MenuItemId;
                            var item = await _menuServices.FindMenuItemById(or);
                            if (item.ItemType == itemType)
                            {
                                result.Quantity += menu.quantity;
                                result.TotalPrice += menu.quantity * item.Price;
                            }
                        }
                    }

                    return result;
                }

                

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

        public async Task<List<OrderDetail>> ProcessingOrders(string ResturantId)
        {
            var res = await _repository.GetItemsAsync<Order>(d =>
                d.ResturantId == ResturantId && d.Status == OrderStatus.Proccesing);
            var list = res?.ToList();
            var listRes = new List<OrderDetail>();
            foreach (var order in list)
            {
                var resOrderDetail = await BuildOrderDetail(order);
                listRes.Add(resOrderDetail);

            }
            return listRes;
        }
        public async Task<List<Order>> ReceivedOrders(string ResturantId)
        {
            var res = await _repository.GetItemsAsync<Order>(d =>
                d.ResturantId == ResturantId && d.Status == OrderStatus.Received);
            var list = res?.ToList();
           // var listRes = new List<OrderDetail>();
            //foreach (var order in list)
            //{
            //    var resOrderDetail = await BuildOrderDetail(order);
            //    listRes.Add(resOrderDetail);
            //}

            return list;
        }

        public async Task<Order> MakeProcessing(string OrderId)
        {
            if (OrderId != null)
            {
                var order = await FindOrderbyId(OrderId);
                if (order != null)
                {
                    order.Status = OrderStatus.Proccesing;
                    await _repository.UpdateAsync<Order>(d => d.Id == order.Id, order);
                    return order;
                }
            }

            return null;
        }
        public async Task<Order> MakeReady(string OrderId)
        {
            if (OrderId != null)
            {
                var order = await FindOrderbyId(OrderId);
                if (order != null)
                {
                    order.Status = OrderStatus.Ready;
                    await _repository.UpdateAsync<Order>(d => d.Id == order.Id, order);
                    return order;
                }
            }

            return null;
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


        public async Task<ItemTypeAnalysis> AnalysisBasedOnType(DateTime start, DateTime end, string resturantId)
        {
            Dictionary<string, ItemUnit> trac = new Dictionary<string, ItemUnit>();
            var Orders = await _repository.GetItemsAsync<Order>(d => d.ResturantId == resturantId);
            if (Orders != null)
            {
                foreach (var order in Orders)
                {
                    if (order.OrderdAt != null)
                    {
                        if (DateTime.Compare(order.OrderdAt, start) > 0 &&
                            DateTime.Compare(order.OrderdAt, end) < 0)
                        {
                            var menuList = order.Items;
                            foreach (var menu in menuList)
                            {
                                var or = menu.MenuItemId;
                                var item = await _menuServices.FindMenuItemById(or);
                                if (trac[item.ItemType] == null)
                                {
                                    trac[item.ItemType] = new ItemUnit();
                                    trac[item.ItemType].ItemType = item.ItemType;
                                    trac[item.ItemType].Quantity = 0;
                                    trac[item.ItemType].TotalSell = 0.0;
                                }

                                trac[item.ItemType].Quantity += menu.quantity;
                                trac[item.ItemType].TotalSell += menu.quantity * item.Price;
                            }
                        }
                    }


                }

                var res = new ItemTypeAnalysis();
                for (int i = 0; i < trac.Count; i++)
                {
                    res.ItemUnits = new List<ItemUnit>();
                    res.ItemUnits.Add(trac[trac.Keys.ElementAt(i)]);
                }

                return res;
            }

            return null;

        }
    }
}
