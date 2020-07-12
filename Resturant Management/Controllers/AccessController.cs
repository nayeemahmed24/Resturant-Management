using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JWT_Token;
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
    [Authorize(Roles = Role.User)]
    public class AccessController : ControllerBase
    {
        private IUserAccessService _accessService;
        private IExceptionModelGenerator _exceptionModelGenerator;
        private IRoutes _routes;
        public AccessController(IRoutes routes,IUserAccessService accessService,IExceptionModelGenerator exceptionModelGenerator)
        {
            _routes = routes;
            _accessService = accessService;
            _exceptionModelGenerator = exceptionModelGenerator;
        }

       
        [HttpPost("invitation")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRestaurant(RestaurantInputModel restaurantInputModel)
        {
            try
            {
                var user = _accessService.ValidateMailVerifyToken(restaurantInputModel.invitationToken);
                var identity = user.Identity as ClaimsIdentity;
                var userId = identity.FindFirst(Claims.UserId)?.Value;
                if (user == null)
                {
                    var Errorresult = _exceptionModelGenerator.setData<RestaurantInputModel>(true, "Unauthorized", null);
                    return StatusCode(401, Errorresult);
                }else if (_accessService.isRestaurantAvailable(userId))
                {
                    var Errorresult = _exceptionModelGenerator.setData<RestaurantInputModel>(true, "Forbidden", null);
                    return StatusCode(403, Errorresult);
                }

                var manager = await _accessService.Create(restaurantInputModel);
                var result = _exceptionModelGenerator.setData<RestaurantModel>(false, "Ok", manager);
                return StatusCode(201, result);
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<RestaurantInputModel>(true, e.Message, null));
            }
        }
        [HttpPost("admincreate")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAdmin(RestaurantInputModel restaurantInputModel)
        {

            try
            {
                var manager = await _accessService.CreateAdmin(restaurantInputModel);
                var result = _exceptionModelGenerator.setData<RestaurantModel>(false, "Ok", manager);
                return StatusCode(201, result);
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<RestaurantInputModel>(true, e.Message, null));
            }
        }



        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel authenticateModel)
        {
            try
            {
                var user = await _accessService.GetUserByUsername(authenticateModel.Username);
               
                if (user == null)
                {
                    return StatusCode(404, _exceptionModelGenerator.setData<TokenModel>(true, "NOT_EXISTS", null));
                }
                else
                {

                    if (_accessService.IsAuthorizedUser(user, authenticateModel.Password))
                    {
                        
                            var token = _accessService.GetAuthenticationToken(user);
                            return StatusCode(200, _exceptionModelGenerator.setData<TokenModel>(false, "Ok", token));
                        
                    }
                    else
                    {
                        return StatusCode(401, _exceptionModelGenerator.setData<TokenModel>(true, "INCORECT_PASSWORD", null));
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<TokenModel>(true, e.Message, null));
            }

        }
        [HttpPost("recover")]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword(RestaurantUpdateModel userUpdateModel)
        {
            try
            {

                var user = await _accessService.GetUserByEmail(userUpdateModel.email);
                if (user != null)
                {
                    string oneTimeToken = _accessService.GetPasswordRecoverToken(user);
                    _accessService.VerificationMailSender(user.email, "Reset Your Password", oneTimeToken, _routes.RecoverPassword);
                    return StatusCode(307, _exceptionModelGenerator.setData<RestaurantUpdateModel>(false, "MAIL_SENT", null));
                }
                else
                {
                    return StatusCode(404, _exceptionModelGenerator.setData<RestaurantUpdateModel>(true, "NOT_FOUND", null));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, _exceptionModelGenerator.setData<RestaurantUpdateModel>(true, e.Message, null));
            }
        }

        [HttpGet("recover/{token}")]
        [AllowAnonymous]
        public IActionResult SetNewPassword(string token)
        {
            string route = _routes.ResetRoute + token;
            return Redirect(route);
        }

        //[HttpPost("reset")]
        //[AllowAnonymous]
        //public IActionResult ResetPassword(RestaurantUpdateModel userUpdateModel)
        //{
        //    try
        //    {
        //        if (userUpdateModel.invitationToken != null)
        //        {
        //            string userId = _accessService.ResetPasswordVerification(userUpdateModel.invitationToken);
        //            if (userId != null)
        //            {
        //                var user = _accessService.GetUser(userId);
        //                if (user != null)
        //                {
        //                    user.password = userUpdateModel.password;
        //                    _accessService.Update(user);
        //                    return StatusCode(200, _exceptionModelGenerator.setData<RestaurantUpdateModel>(false, "Ok", null));
        //                }
        //                else
        //                {
        //                    return StatusCode(404, _exceptionModelGenerator.setData<RestaurantUpdateModel>(true, "NOT_FOUND", null));
        //                }
        //            }
        //            else
        //            {
        //                return StatusCode(404, _exceptionModelGenerator.setData<RestaurantUpdateModel>(true, "NOT_FOUND", null));
        //            }
        //        }
        //        else
        //        {
        //            return StatusCode(401, _exceptionModelGenerator.setData<RestaurantUpdateModel>(true, "UNAUTHORIZED", null));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(401, _exceptionModelGenerator.setData<RestaurantUpdateModel>(true, e.Message, null));
        //    }
        //}


    }
}
