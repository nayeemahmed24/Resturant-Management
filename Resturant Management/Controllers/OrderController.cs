using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Model;
using Model.Entities;
using Model.Error_Handler;
using Model.Input_Model;
using Model.View_Model;
using Payment_System.Model;
using Payment_System.Service;
using Resturant_Management.Communication.Hubs;
using Services.OrderService;
using Services.UserServices;

namespace Resturant_Management.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.User)]
    public class OrderController : ControllerBase
    {
        private IExceptionModelGenerator _exceptionModelGenerator;
        private IOrderService _orderService;
        private IHubContext<CommunicatonHub> _hubContext;
        //Test Payment
        private IPaymentService _paymentService;
        private IUserAccessService _userAccessService;
        public OrderController( IUserAccessService userAccessService,IHubContext<CommunicatonHub> hubContext,IPaymentService paymentService,IOrderService orderService,IExceptionModelGenerator exceptionModelGenerator)

        {
            _userAccessService = userAccessService;
            _paymentService = paymentService;
            _orderService = orderService;
            _exceptionModelGenerator = exceptionModelGenerator;
            _hubContext = hubContext;
        }


        [HttpGet("pay")]
        [AllowAnonymous]
        public async Task<IActionResult> Pay(PaymentInputModel pm)
        {
            
            try
            {

                var res = await _paymentService.MakePayment(pm);
                if (res)
                {
                    var order = await _orderService.makePayment(pm.OrderId);
                    if (order != null)
                    {
                        return StatusCode(201, _exceptionModelGenerator.setData<Order>(false, "Ok", order));

                    }
                }
                return StatusCode(201, _exceptionModelGenerator.setData<Order>(true, "Ok", null));



            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<RestaurantInputModel>(true, e.Message, null));
            }
        }

        [HttpPost("Order/{restaurantId}")]
        [AllowAnonymous]
        public async Task<IActionResult> PlaceOrder(string restaurantId,Order order)
        {
            try
            {
                if (order.ResturantId != null)
                {
                    var status = await _userAccessService.GetStatus(order.ResturantId);
                    if (status == ResturantStatus.Close)
                    {
                        var resut = _exceptionModelGenerator.setData<Order>(true, "Resturant Is Closed", null);
                        return StatusCode(500, resut);

                    }
                }
                order.Status = OrderStatus.Received;
                var res = await _orderService.makeOrder(order);
                if (res != null)
                {
                    var resul = _exceptionModelGenerator.setData<Order>(false, "Ok", res);
                    await _hubContext.Clients.All.SendAsync(restaurantId, resul);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<Table>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<Order>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }
        // Get All Processing Order
        [HttpGet("ProcessingOrder")]
        public async Task<IActionResult> ProcessingOrder()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                var res = await _orderService.ProcessingOrders(userId);
                if (res != null)
                {
                    var resul = _exceptionModelGenerator.setData<List<Order>>(false, "Ok", res);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<List<Order>>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<List<Order>>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }
        // Get All Recieced Order
        [HttpGet("RecievedOrder")]
        public async Task<IActionResult> ReceivedOrder()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                var res = await _orderService.ReceivedOrders(userId);
                if (res != null)
                {
                    var resul = _exceptionModelGenerator.setData<List<Order>>(false, "Ok", res);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<List<Order>>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<List<Order>>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }
        
        [HttpGet("changestatus/process/{orderid}")]
        public async Task<IActionResult> MakeProcessOrder(string orderid)
        {
            
            try
            {
                var res = await _orderService.MakeProcessing(orderid);
                if (res != null)
                {
                    var resul = _exceptionModelGenerator.setData<Order>(false, "Ok", res);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<Order>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<Order>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }
        [HttpGet("changestatus/ready/{orderid}")]
        public async Task<IActionResult> MakeReadyOrder(string orderid)
        {

            try
            {
                var res = await _orderService.MakeReady(orderid);
                if (res != null)
                {
                    var resul = _exceptionModelGenerator.setData<Order>(false, "Ok", res);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<Order>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<Order>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }

        [HttpGet("TotalSell/{itemtype}")]
        public async Task<IActionResult> TotalSell(string itemtype)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                var res = await _orderService.FindTotalSellByItemType(itemtype,userId);
                if (res != null)
                {
                    var resul = _exceptionModelGenerator.setData<SoldQuantity>(false, "Ok", res);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<SoldQuantity>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<SoldQuantity>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }





        [HttpGet]
        public async Task<IActionResult> GetOrderDetail(string orderId)
        {
            try
            {
                var res = await _orderService.GetFullOrder(orderId);
                if (res != null)
                {

                    var resul = _exceptionModelGenerator.setData<OrderDetail>(false, "Ok", res);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<OrderDetail>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<OrderDetail>(true, e.Message, null);
                return StatusCode(500, result);
            }
        }
    }
}
