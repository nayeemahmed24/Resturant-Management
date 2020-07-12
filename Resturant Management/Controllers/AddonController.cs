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

namespace Resturant_Management.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("AddAddon")]
        
        public async Task<IActionResult> AddAddon(AddonInput addonInput)
        {

            try
            {
                var addon = await _addonService.AddAddon(addonInput);
                if (addon != null)
                {
                    var resul = _exceptionModelGenerator.setData<Addon>(false, "Ok", addon);
                    return StatusCode(201, resul);
                }

                var result = _exceptionModelGenerator.setData<Addon>(true, "Ok", null);
                return StatusCode(500, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<Addon>(true, e.Message, null));

            }
        }

        [HttpPost("UpdateAddon")]
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

                var result = _exceptionModelGenerator.setData<Addon>(true, "Ok", null);
                return StatusCode(500, result);

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

                var result = _exceptionModelGenerator.setData<Addon>(true, "Ok", null);
                return StatusCode(500, result);

            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<Addon>(true, e.Message, null));

            }
        }



    }
}
