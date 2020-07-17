using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Error_Handler;
using Services.UserServices;
using Model.Entities;
using Services.UtilityService;
using Model.View_Model;

namespace Resturant_Management.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private IUserAccessService _userAccessService;
        private IExceptionModelGenerator _exceptionModelGenerator;
        public CommonController(IUserAccessService userAccessService,IExceptionModelGenerator exceptionModelGenerator)
        {
            _userAccessService = userAccessService;
            _exceptionModelGenerator = exceptionModelGenerator;
        }
        // GET: api/Customer
        [HttpGet("restaurant/{restuarantId}")]
        public IActionResult GetRestaurant(string restuarantId)
        {
            try
            {
                var restaurant = _userAccessService.GetUser(restuarantId).FilterForCustomer();
                if (restaurant == null)
                {
                    return StatusCode(404, _exceptionModelGenerator.setData<CustomerViewModel>(true, "No Restaurant", null));
                }
                return StatusCode(200, _exceptionModelGenerator.setData<CustomerViewModel>(false, "Ok", restaurant));
            }catch(Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<CustomerViewModel>(true, e.Message, null));
            }
        }

        // GET: api/Customer/5
        [HttpGet("background/{restuarantId}")]
        public IActionResult GetRestaurantImage(string restuarantId)
        {
            try
            {
                var restaurant = _userAccessService.GetUser(restuarantId);
                if (restaurant == null || restaurant.backgroundImage == null)
                {
                    return StatusCode(204, _exceptionModelGenerator.setData<RestaurantModel>(true, "NOT_FOUND", null));
                }
                var fileData = _userAccessService.ImagePath(restaurant.backgroundImage);

                var b = PhysicalFile(fileData.path, "image/" + fileData.Imagetype);

                return b;
            }
            catch(Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<RestaurantModel>(true, e.Message, null));
            }
            
        }

        [HttpGet("logo/{restuarantId}")]
        public IActionResult GetRestaurantLogo(string restuarantId)
        {
            try
            {
                var restaurant = _userAccessService.GetUser(restuarantId);
                if (restaurant == null || restaurant.logo == null)
                {
                    return StatusCode(204, _exceptionModelGenerator.setData<RestaurantModel>(true, "NOT_FOUND", null));
                }
                var fileData = _userAccessService.ImagePath(restaurant.logo);

                var b = PhysicalFile(fileData.path, "image/" + fileData.Imagetype);

                return b;
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<RestaurantModel>(true, e.Message, null));
            }

        }
    }
}
