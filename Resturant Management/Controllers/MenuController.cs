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
    [Route("api/[controller]")]
    [ApiController]
     [Authorize(Roles = Role.User)]
    public class MenuController : ControllerBase
    {
        private IMenuServices _menuServices;
        private IExceptionModelGenerator _exceptionModelGenerator;
        private IUserAccessService _userAccessService;
        public MenuController(IMenuServices menuServices,IExceptionModelGenerator exceptionModelGenerator,IUserAccessService userAccessService)
        {
            _userAccessService = userAccessService;
            _menuServices = menuServices;
            _exceptionModelGenerator = exceptionModelGenerator;

        }

        [HttpPost("addcategory")]
        [AllowAnonymous]
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

        [HttpPost("getChildCategories")]
        public async Task<IActionResult> GetChildCategories(MenuCategoryInput menuCategoryInput)
        {
            // For Test Off
            // Authorize and Role Set
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
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

        [HttpGet("basecategories")]
        public async Task<IActionResult> BaseCategories()
        {
            // For Test Off
            // Authorize and Role Set
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            // var userId = "5f075a729138c5a8d03175b5";
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


    }
}
