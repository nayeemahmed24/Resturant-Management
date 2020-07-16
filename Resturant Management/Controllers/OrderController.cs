using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Entities;
using Model.Error_Handler;
using Model.View_Model;
using Services.OrderService;

namespace Resturant_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.User)]
    public class OrderController : ControllerBase
    {
        private IExceptionModelGenerator _exceptionModelGenerator;
        private IOrderService _orderService;
        public OrderController(IOrderService orderService,IExceptionModelGenerator exceptionModelGenerator)
        {
            _orderService = orderService;
            _exceptionModelGenerator = exceptionModelGenerator;
        }

        [HttpPost("Order")]
        [AllowAnonymous]
        public async Task<IActionResult> MakeOrder(Order order)
        {
            try
            {
                order.Status = OrderStatus.Received;
                var res = await _orderService.makeOrder(order);
                if (res != null)
                {

                    var resul = _exceptionModelGenerator.setData<Order>(false, "Ok", res);
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
        [HttpGet("ActiveOrder")]
        [AllowAnonymous]
        public async Task<IActionResult> ActiveOrder()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                var res = await _orderService.ActiveOrders(userId);
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
