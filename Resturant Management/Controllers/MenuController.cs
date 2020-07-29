using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    [Authorize(Roles = Role.User)]

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
            _sortService = sortService;
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
                if (categoryList.Count == 0)
                {
                    return StatusCode(203, _exceptionModelGenerator.setData<MenuCategoryInput>(true, "No data", null));
                }
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
                if(categoryList.Count == 0)
                {
                    return StatusCode(204, _exceptionModelGenerator.setData<MenuCatergory>(false, "No data", null));
                }
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
        [HttpPut("menucategoryUpdate")]
        public async Task<IActionResult> UpdateMenuCategory(MenuCategoryInput menuCategoryItemInput)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                if (menuCategoryItemInput.Id == null )
                {
                    var Errorresult = _exceptionModelGenerator.setData<MenuCategoryInput>(true, "Invalid input", null);
                    return StatusCode(400, Errorresult);
                }

                menuCategoryItemInput.RestaurantId = userId;
                var menuItem = await _menuServices.UpdateMenuCategory(menuCategoryItemInput);
                if (menuItem != null)
                {
                    var resul = _exceptionModelGenerator.setData<MenuCatergory>(false, "Ok", menuItem);
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
                    return StatusCode(203, Errorresult);
                }

                menuItemInput.ResturantId = userId;
                var menuItem = await _menuServices.UpdateMenu(menuItemInput);
                if (menuItem != null)
                {
                    var resul = _exceptionModelGenerator.setData<MenuItem>(false, "Ok", menuItem);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<MenuItem>(true, "Ok", null);
                return StatusCode(203, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<MenuCatergory>(true, e.Message, null));
            }
        }

        [HttpGet("getmenuitems/{parentId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMenuItemsByParenId(string parentId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                var menu = await _menuServices.FindMenuParentId(parentId);
                if (menu.Count!=0)
                {
                    var sort = await _sortService.FindSortUsingParentId(parentId);
                    menu = _sortService.SortItems(sort, menu);
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

        [HttpGet("restaurantMenu")]
        public async Task<IActionResult> GetRestaurantItems()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var restaurantId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                var menuList = await  _menuServices.GetRestaurantMenu(restaurantId);
                if (menuList != null && menuList.Count!=0)
                {
                    return StatusCode(200, _exceptionModelGenerator.setData<List<MenuItem>>(false, "ok", menuList));
                }

                return StatusCode(203, _exceptionModelGenerator.setData<MenuItem>(true, "Not data", null));

            }catch(Exception e)
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


        [HttpGet("deletecategory/{categoryid}")]
        public async Task<IActionResult> Delete(string categoryid)
        {
            try
            {
                await _menuServices.DeleteMenuCategory(categoryid);
                var result = _exceptionModelGenerator.setData<string>(false, "Sucess", null);
                return StatusCode(201, result);
            }
            catch (Exception e)
            {
                var reslt = _exceptionModelGenerator.setData<SortOrder>(true, e.Message, null);
                return StatusCode(500, reslt);
            }
        }

        [HttpGet("delete/{tableid}")]
        public async Task<IActionResult> DeleteTable(string tableid)
        {
            try
            {
                await _menuServices.DeleteMenuWithAll(tableid);
                var result = _exceptionModelGenerator.setData<string>(false, "Sucess", null);
                return StatusCode(201, result);
            }
            catch (Exception e)
            {
                var reslt = _exceptionModelGenerator.setData<SortOrder>(true, e.Message, null);
                return StatusCode(500, reslt);
            }
        }

        // MenuItem with all Addon List
        // In View Model = MenuItemDetails

        [HttpGet("itemDetailes/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ItemDetailes(string id)
        {
            try
            {
                if (id != null)
                {
                    var menu = await _menuServices.FindMenuItemById(id);
                    if (menu != null)
                    {
                        var listAddon = await _addonService.FindAddonByItemId(menu.Id);
                        var menuItemDetails = new MenuItemDetailes
                        {
                            Menu = menu,
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
                var result = _exceptionModelGenerator.setData<OrderDetail>(true, "Ok", null);
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
