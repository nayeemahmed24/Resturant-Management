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
using Services.AddonServices;
/// <summary>
/// "AddAddonCategory"
/// "AddAddon"
/// "basecategories/{resturantid}" 
/// "childaddons/{categoryid}"
///  "UpdateAddon"
///  "MenuItemAddon/{itemid}"
///  "AssignAddon"
/// </summary>
namespace Resturant_Management.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.User)]
    // [Authorize(Roles = Role.Admin)]
    public class AddonController : ControllerBase
    {
        private readonly IAddonService _addonService;
        private IExceptionModelGenerator _exceptionModelGenerator;

        public AddonController(IAddonService addonService, IExceptionModelGenerator exceptionModelGenerator)
        {
            _addonService = addonService;
            _exceptionModelGenerator = exceptionModelGenerator;
        }

        [HttpPost("createCategory")]
        public async Task<IActionResult> AddAddonCategory(AddonCategory addonCategory)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                addonCategory.RestaurantId = userId;
                var res = await _addonService.AddAddonCategory(addonCategory);
                if (res != null)
                {
                    var resul = _exceptionModelGenerator.setData<AddonCategory>(false, "Ok", res);
                    return StatusCode(201, resul);
                }
                var result = _exceptionModelGenerator.setData<AddonCategory>(true, "Failed", null);
                return StatusCode(400, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<AddonCategory>(true, e.Message, null));
            }
        }

        [HttpPost("createAddon")]
        public async Task<IActionResult> AddAddon(AddonInput addonInput)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                addonInput.ResturantId = userId;
                var addon = await _addonService.AddAddon(addonInput);
                if (addon != null)
                {
                    var resul = _exceptionModelGenerator.setData<Addon>(false, "Ok", addon);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<Addon>(true, "Failed", null);
                return StatusCode(400, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<Addon>(true, e.Message, null));

            }
        }

        [HttpGet("childaddons/{categoryid}")]
        public async Task<IActionResult> Addons(string categoryid)
        {
            try
            {
                var res = await _addonService.AllAddonChildByCategoryId(categoryid);
                if (res != null)
                { 
                    var resul = _exceptionModelGenerator.setData<List<Addon>>(false, "Ok", res);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<List<Addon>>(true, "Failed", null);
                return StatusCode(400, result);
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<List<Addon>>(true, e.Message, null));
            }
        }


        [HttpPut("UpdateAddon")]
        public async Task<IActionResult> EditAddon(AddonInput addonInput)
        {

            try
            {

                var addon = await _addonService.UpdateAddon(addonInput);
                if (addon != null)
                {
                    var resul = _exceptionModelGenerator.setData<Addon>(false, "Ok", addon);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<Addon>(true, "Failed", null);
                return StatusCode(400, result);

            }
            catch (Exception ex)
            {
                var result = _exceptionModelGenerator.setData<Addon>(true, ex.Message, null);
                return StatusCode(500, result);
            }
        }

        [HttpPost("changeAddonStatus")]
        public async Task<IActionResult> ChangeAddonStatus(AddonInput addonInput)
        {
            try
            {
                var addon =   await _addonService.ChangeStatus(addonInput);
                if (addon != null)
                {
                    var resul = _exceptionModelGenerator.setData<Addon>(false, "Ok", addon);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<Addon>(true, "Failed", null);
                return StatusCode(400, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<Addon>(true, e.Message, null));

            }
        }

        [HttpGet("basecategories")]
        public async Task<IActionResult> BaseCategories()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var resturantid = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                var res = await _addonService.FindBaseAddonCategory(resturantid);
                if (res != null)
                {
                    var resul = _exceptionModelGenerator.setData<List<AddonCategory>>(false, "Ok", res);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<Addon>(true, "Failed", null);
                return StatusCode(400, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<List<Addon>>(true, e.Message, null));
            }
        }



        [HttpPost("assignaddon")]
        public async Task<IActionResult> AssigAddon(AssignAddon assignAddon)
        {
            try
            {
                if (assignAddon != null)
                {
                    var res = await _addonService.AssignAddon(assignAddon);
                    if (res != null)
                    {
                        var resul = _exceptionModelGenerator.setData<dynamic>(false, "Ok", res);
                        return StatusCode(201, resul);
                    }
                    
                }
                var result = _exceptionModelGenerator.setData<dynamic>(true, "Failed", null);
                return StatusCode(400, result);
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<dynamic>(true, e.Message, null));
            }
        }


        // All Addon Using resturant ID
        [HttpGet("alladdons/{resturantid}")]
        [AllowAnonymous]
        public async Task<IActionResult> AllAddons(string resturantid)
        {
            try
            {
                if (resturantid != null)
                {
                    var list = await _addonService.AllAddonByResturantId(resturantid);
                    if (list != null)
                    {
                        var resul = _exceptionModelGenerator.setData<List<Addon>>(false, "Ok", list);
                        return StatusCode(201, resul);
                    }
                
                }
                var result = _exceptionModelGenerator.setData<List<Addon>>(true, "Failed", null);
                return StatusCode(400, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<List<Addon>>(true, e.Message, null));
            }
        }

        [HttpGet("delete/{addonid}")]
        public async Task<IActionResult> DeleteAddon(string addonid)
        {
            try
            {
                await _addonService.DeleteAddon(addonid);
                var resul = _exceptionModelGenerator.setData<string>(false, "Success", null);
                return StatusCode(201, resul);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<List<Addon>>(true, e.Message, null));
            }
        }
        [HttpGet("deleteCategory/{addoncategoryid}")]
        public async Task<IActionResult> DeleteAddonCategory(string addoncategoryid)
        {
            try
            {
                await _addonService.DeleteAddonCategory(addoncategoryid);
                var resul = _exceptionModelGenerator.setData<string>(false, "Success", null);
                return StatusCode(201, resul);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<List<Addon>>(true, e.Message, null));
            }
        }

        [HttpGet("MenuItemAddon/{itemid}")]
        [AllowAnonymous]
        public async Task<IActionResult> MenuItemAddons(string itemid)
        {
            try
            {
                if (itemid != null)
                {
                    var lists = await _addonService.FindAddonByItemId(itemid);
                    var list = lists.Distinct().ToList();
                    if (list != null)
                    {
                        var resul = _exceptionModelGenerator.setData<List<Addon>>(false, "Ok", list);
                        return StatusCode(201, resul);
                    }
        
                }
                var result = _exceptionModelGenerator.setData<List<Addon>>(true, "Failed", null);
                return StatusCode(400, result);
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<List<Addon>>(true, e.Message, null));
            }
        }


    }
}
