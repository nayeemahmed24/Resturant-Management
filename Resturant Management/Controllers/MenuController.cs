using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Entities;
using Model.Error_Handler;
using Model.Input_Model;
using Services.MenuServices;

namespace Resturant_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = Role.User)]
    public class MenuController : ControllerBase
    {
        private IMenuServices _menuServices;
        private IExceptionModelGenerator _exceptionModelGenerator;
        public MenuController(IMenuServices menuServices,IExceptionModelGenerator exceptionModelGenerator)
        {
            _menuServices = menuServices;
            _exceptionModelGenerator = exceptionModelGenerator;

        }

        [HttpPost("addcategory")]
        [AllowAnonymous]
        public async Task<IActionResult> AddCategory(MenuCategoryInput menuCategoryInput)
        {
            try
            {
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



    }
}
