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
using Services.Sort_Service;
using Services.UserServices;

namespace Resturant_Management.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]

    [Authorize(Roles = Role.Admin)]

    public class MenuController : ControllerBase
    {
        private IMenuServices _menuServices;
        private IExceptionModelGenerator _exceptionModelGenerator;
        private IAddonService _addonService;
        private ISortService _sortService;
        public MenuController(ISortService sortService,IAddonService addonService,IMenuServices menuServices,IExceptionModelGenerator exceptionModelGenerator)
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
                var sortOrder = await GetSortOrder(parentId);

                categoryList = _sortService.SortCategory(sortOrder, categoryList);

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
                var sortOrder = await GetSortOrder("base");

                categoryList = _sortService.SortCategory(sortOrder, categoryList);
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
                var result = _exceptionModelGenerator.setData<MenuItem>(true, "Invalid information", null);
                return StatusCode(203, result);


            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));
            }
        }

        [HttpPut("menuUpdate")]
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

        [HttpGet("getmenuitems/{parentId}")]
        public async Task<IActionResult> GetMenuItemsByParenId(string parentId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                var menu = await _menuServices.FindMenuParentId(parentId);
                if (menu != null)
                {
                    return StatusCode(200, _exceptionModelGenerator.setData<List<MenuItem>>(false, "Ok", menu));
                }
                else
                {
                    return StatusCode(200, _exceptionModelGenerator.setData<MenuItem>(true, "Not found", null));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));
            }
        }



        [HttpPut("changeItemStatus")]
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


        // MenuItem with all Addon List
        // In View Model = MenuItemDetails

        

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

        [HttpPost("sortedit")]
        public async Task<IActionResult> EditSort(SortOrder sort)
        {
            try
            {
                if (sort.ParentId != null)
                {
                    if (sort.SortList != null)
                    {
                        var res = await  _sortService.EditSort(sort);
                        if (res != null)
                        {
                            var resul = _exceptionModelGenerator.setData<SortOrder>(false, "Ok", res);
                            return StatusCode(201, resul);
                        }
                    }
                }
                var result = _exceptionModelGenerator.setData<MenuItemDetailes>(true, "Ok", null);
                return StatusCode(500, result);
            }
            catch (Exception e)
            {
                var reslt = _exceptionModelGenerator.setData<SortOrder>(true, "Ok", null);
                return StatusCode(500, reslt);
            }
        }
        private async Task<SortOrder> GetSortOrder(string parentId)
        {
            return await _sortService.FindSortUsingParentId(parentId);
        }


    }
}
