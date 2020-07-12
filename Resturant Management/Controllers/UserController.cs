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
using Services.UserServices;

namespace Resturant_Management.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IExceptionModelGenerator _exceptionModelGenerator;
        private IUserAccessService _userAccessService;
        public UserController(IUserAccessService userAccessService,IExceptionModelGenerator exceptionModelGenerator)
        {
            _userAccessService = userAccessService;
            _exceptionModelGenerator = exceptionModelGenerator;
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                var user = _userAccessService.GetUser(userId);
                user.logo = null;
                user.password = null;
                if (user != null)
                {
                    var result = _exceptionModelGenerator.setData<RestaurantModel>(false, "Ok", user);
                    return StatusCode(200, result);
                }
                else
                {
                    var result = _exceptionModelGenerator.setData<RestaurantModel>(true, "NOT_FOUND", null);
                    return StatusCode(404, result);
                }


            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<RestaurantModel>(true, e.Message, null);
                return StatusCode(500, result);
            }

        }

        [HttpPut("update")]
        public IActionResult Put(RestaurantUpdateModel userUpdateModel)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            var logo = userUpdateModel.logo;
            try
            {

                var user = _userAccessService.GetUser(userId);
                if (user != null)
                {
                    var newdata = _userAccessService.UpdateResturant(userUpdateModel,user);
                    return StatusCode(204, _exceptionModelGenerator.setData<RestaurantModel>(false, "Ok", null));
                }
                else
                {
                    return StatusCode(404, _exceptionModelGenerator.setData<RestaurantModel>(true, "NOT_FOUND", null));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<RestaurantModel>(true, e.Message, null));
            }
        }

        [HttpGet("image")]
        public IActionResult GetImage()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            Response.Headers.Add("Cache-Control", "no-cache");
            try
            {
                var user = _userAccessService.GetUser(userId);

                if (user == null)
                {
                    return StatusCode(404, _exceptionModelGenerator.setData<RestaurantModel>(true, "NOT_FOUND", null));
                }

                var fileData = _userAccessService.ImagePath(user);

                var b = PhysicalFile(fileData.path, "image/" + fileData.Imagetype);

                return b;
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<RestaurantModel>(true, e.Message, null));
            }
        }

        [HttpPut("update/photo")]
        public async Task<IActionResult> PhotoUpdate([FromForm]PhotoUpdate photoUpdate)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            var photo = photoUpdate.profilePhoto;
            try
            {

                var user = _userAccessService.GetUser(userId);
                if (user != null)
                {
                    var newdata = _userAccessService.UpdateImage(photoUpdate, user);
                    return PhysicalFile(newdata.Result.path, "image/" + newdata.Result.Imagetype);
                }
                else
                {
                    return StatusCode(404, _exceptionModelGenerator.setData<RestaurantModel>(true, "NOT_FOUND", null));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<RestaurantModel>(true, e.Message, null));
            }
        }

        [HttpPut("reset")]
        public IActionResult ResetPassword(RestaurantUpdateModel userUpdateModel)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(Claims.UserId)?.Value;
            try
            {
                _userAccessService.ResetPassword(userId, userUpdateModel);
                var result = _exceptionModelGenerator.setData<RestaurantInputModel>(false, "PASSWORD_RESETED", null);
                return Ok(result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<RestaurantInputModel>(true, e.Message, null);
                return StatusCode(500, result);
            }

        }


    }
}
