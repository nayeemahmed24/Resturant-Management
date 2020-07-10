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
using Model.View_Model;
using Services.AddonServices;
using Services.MenuServices;
using Services.UserServices;

namespace Resturant_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = Role.User)]
    [Authorize(Roles = Role.Admin)]

    public class MenuController : ControllerBase
    {
        private IMenuServices _menuServices;
        private IExceptionModelGenerator _exceptionModelGenerator;
        private IAddonService _addonService;
        public MenuController(IAddonService addonService,IMenuServices menuServices,IExceptionModelGenerator exceptionModelGenerator)
        {
            _addonService = addonService;
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

        [HttpPost("{ResturantId}/getChildCategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChildCategories(String ResturantId, MenuCategoryInput menuCategoryInput)
        {
            
            var userId = ResturantId;
            try
            {
                // For Test Off
                menuCategoryInput.RestaurantId = userId;
                var categoryList = await _menuServices.GetChildCategories(menuCategoryInput);
                return StatusCode(201,
                    _exceptionModelGenerator.setData<List<MenuCatergory>>(false, null, categoryList));

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));

            }
        }

        [HttpGet("{ResturantId}/basecategories")]
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

        [HttpPost("itemDetailes")]
        [AllowAnonymous]
        public async Task<IActionResult> ItemDetailes(MenuItemInput menuItem)
        {
            try
            {
                if (menuItem.Id != null)
                {
                    var menu = await _menuServices.FindMenuItemById(menuItem.Id);
                    if (menu != null)
                    {
                        var listAddon = await _addonService.AllAddonByMenuItemId(menu.Id);
                        var menuItemDetails = new MenuItemDetailes
                        {
                            MenuItem = menu,
                            Addons = listAddon
                        };
                        var resul = _exceptionModelGenerator.setData<MenuItemDetailes>(false, "Ok", menuItemDetails);
                        return StatusCode(201, resul);

                    }
                    var resut = _exceptionModelGenerator.setData<MenuItemDetailes>(true, "Ok", null);
                    return StatusCode(500, resut);
                }
                var result = _exceptionModelGenerator.setData<MenuItemDetailes>(true, "Ok", null);
                return StatusCode(500, result);

            }
            catch (Exception e)
            {
                var reslt = _exceptionModelGenerator.setData<MenuItem>(true, "Ok", null);
                return StatusCode(500, reslt);
            }
            
        }



        


    }
}
