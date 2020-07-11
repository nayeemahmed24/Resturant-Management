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
using Model.Input_Model;
using Services.MenuServices;
using Services.UserServices;

namespace Resturant_Management.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.User)]
    public class MenuController : ControllerBase
    {
        private IMenuServices _menuServices;
        private IExceptionModelGenerator _exceptionModelGenerator;
        //private IUserAccessService _userAccessService;
        public MenuController(IMenuServices menuServices,IExceptionModelGenerator exceptionModelGenerator)
        {
          //  _userAccessService = userAccessService;
            _menuServices = menuServices;
            _exceptionModelGenerator = exceptionModelGenerator;

        }

        [HttpPost("addcategory")]
        public async Task<IActionResult> AddCategory(MenuCategoryInput menuCategoryInput)
        {
            // For Test Off
            // Authorize and Role Set
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
           
            try
            {   // For Test Off
                menuCategoryInput.RestaurantId = userId;
                var menu = await _menuServices.AddCategory(menuCategoryInput);
                if (menu != null)
                {
                    var resul = _exceptionModelGenerator.setData<MenuCatergory>(false, "Ok", menu);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<MenuCatergory>(true, "Ok", null);
                return StatusCode(500, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));

            }


        }

        [HttpGet("getchild/{parentId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChildCategories(String parentId)
        {
            
           // var userId = ResturantId;
            try
            {
                // For Test Off
               // menuCategoryInput.RestaurantId = userId;
                var categoryList = await _menuServices.GetChildCategories(parentId);
                return StatusCode(201,
                    _exceptionModelGenerator.setData<List<MenuCatergory>>(false, null, categoryList));

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));

            }
        }

        [HttpGet("base/{ResturantId}")]
        [AllowAnonymous]
        public async Task<IActionResult> BaseCategories(String ResturantId)
        {
            
             var userId = ResturantId;
            try
            {
                
                var categoryList = await _menuServices.GetBaseCategories(userId);
                return StatusCode(201,
                    _exceptionModelGenerator.setData<List<MenuCatergory>>(false, null, categoryList));

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));

            }
        }

        [HttpPost("createmenu")]
        public async Task<IActionResult> CreateMenu(MenuItemInput menuItemInput)
        {
            // For Test Off
            // Authorize and Role Set
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                menuItemInput.ResturantId = userId;
                var menuItem = await _menuServices.AddMenuItem(menuItemInput);
                if (menuItem != null)
                {
                    var resul = _exceptionModelGenerator.setData<MenuItem>(false, "Ok", menuItem);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<MenuItem>(true, "Ok", null);
                return StatusCode(500, result);


            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));
            }
        }

        [HttpPost("MenuUpdate")]
        public async Task<IActionResult> UpdateMenu(MenuItemInput menuItemInput)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
               
                

                if (menuItemInput.Id == null || menuItemInput.ParentId == null)
                {
                    var Errorresult = _exceptionModelGenerator.setData<MenuItem>(true, "Ok", null);
                    return StatusCode(500, Errorresult);
                }

                menuItemInput.ResturantId = userId;
                var menuItem = await _menuServices.UpdateMenu(menuItemInput);
                if (menuItem != null)
                {
                    var resul = _exceptionModelGenerator.setData<MenuItem>(false, "Ok", menuItem);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<MenuItem>(true, "Ok", null);
                return StatusCode(500, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));
            }
        }



        [HttpPost("ChangeAvailableStatus")]
        public async Task<IActionResult> ChangeAvailableStatus(MenuItemInput menuItemInput)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                if (menuItemInput.Id == null || menuItemInput.ParentId == null)
                {
                    var Errorresult = _exceptionModelGenerator.setData<MenuItem>(true, "Ok", null);
                    return StatusCode(500, Errorresult);
                }

                menuItemInput.ResturantId= userId;
                var menuItem = await _menuServices.ChangeAvailableStatus(menuItemInput);
                if (menuItem != null)
                {
                    var resul = _exceptionModelGenerator.setData<MenuItem>(false, "Ok", menuItem);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<MenuItem>(true, "Ok", null);
                return StatusCode(500, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));
            }
        }
    }
}
