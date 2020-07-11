using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT_Token;
using JWT_Token.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Model;
using Model.Entities;
using Model.Error_Handler;
using Model.Input_Model;
using Model.View_Model;
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

        [HttpGet("getrestaurants")]
        public IActionResult GetRestaurants([FromQuery] PageParameters pageParameters)
        {
            try
            {
                var data = _adminAccessService.GetAllRestaurants(pageParameters);
                var result = _exceptionModelGenerator.setData<PaginatorModel<RestaurantModel>>(false, "Ok", data);
                return StatusCode(200, result);
            }
            catch (Exception e)
            {
                var result = _exceptionModelGenerator.setData<RestaurantModel>(true, e.Message, null);
                return StatusCode(500, result);

            }
        }
    }
}
