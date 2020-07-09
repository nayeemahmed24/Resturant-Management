using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT_Token;
using JWT_Token.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Entities;
using Model.Error_Handler;
using Model.Input_Model;
using MongoDB.Driver;
using Repository;
using Services.AdminAuth.Contracts;

namespace Resturant_Management.Controllers
{
    [Route("v1/admin")]
    [ApiController]
    [Authorize(Roles = Role.Admin)]
    public class AdminAccessController : ControllerBase
    {
        private readonly IMongoRepository _users;
       
        private IExceptionModelGenerator _exceptionModelGenerator;
        private readonly IAdminVerificationService _adminVerificationService;
        private readonly IAdminAccessService _adminAccessService;
        public AdminAccessController(IMongoRepository users,IAdminVerificationService adminVerificationService,IExceptionModelGenerator exceptionModelGenerator,IAdminAccessService adminAccessService)
        {
            _adminVerificationService = adminVerificationService;
            _adminAccessService = adminAccessService;
            _users = users;
            
            _exceptionModelGenerator = exceptionModelGenerator;
        }
        [HttpPost("register")]
        [AllowAnonymous]

        public async Task<IActionResult> SignUp(AdminInputModel adminInputModel)
        {
            var uri = Request.HttpContext.Request.ToString();
            if (await _adminVerificationService.VerifyUserName(adminInputModel.UserName))
            {
                return BadRequest(new { message = "Username already exist" });
            }
            else if (await _adminVerificationService.IsEmailAvailable(adminInputModel.Email))
            {
                return BadRequest(new { message = "User already exist with this email" });
            }
            try
            {
                var user = await _adminAccessService.Create(adminInputModel);
                var result = _exceptionModelGenerator.setData<AdminUserModel>(false, "Ok", user);
                return StatusCode(201, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<AdminUserModel>(true, e.Message, null);
                return StatusCode(500, result);
            }

        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel authenticateModel)
        {
            try
            {
                var user = await _adminAccessService.GetUserByUserName(authenticateModel.Username);
                if (user == null)
                {
                    return StatusCode(404, _exceptionModelGenerator.setData<TokenModel>(true, "NOT_EXISTS", null));
                }
                else
                {
                    if (_adminAccessService.IsAuthorizedUser(user, authenticateModel.Password))
                    {
                        var token = _adminAccessService.GetAuthenticationToken(user);
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

        [HttpPost("restaurant")]
        public IActionResult SignUp([FromBody]InvitationModel invitationModel)
        {

            try
            {
                _adminAccessService.SendInvitation(invitationModel);
                var result = _exceptionModelGenerator.setData<RestaurantModel>(false, "Ok", null);
                return StatusCode(201, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<RestaurantModel>(true, e.Message, null);
                return StatusCode(500, result);

            }
        }
    }
}
