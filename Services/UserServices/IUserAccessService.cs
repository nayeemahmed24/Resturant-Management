using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWT_Token;
using Model.Entities;
using Model.Input_Model;
using Model.View_Model;

namespace Services.UserServices
{
    public interface IUserAccessService
    {
        public ClaimsPrincipal ValidateMailVerifyToken(string token);
        public  Task<RestaurantModel> Create(RestaurantInputModel userResponse);
        public Task<RestaurantModel> GetUserByUsername(string username);
        public bool IsAuthorizedUser(RestaurantModel userModel, string password);
        public TokenModel GetAuthenticationToken(RestaurantModel user);
        public  Task<RestaurantModel> GetUserByEmail(string email);
        public string GetPasswordRecoverToken(RestaurantModel userModel);
        public void VerificationMailSender(string emailTo, string subject, string token, string api);
        public string ResetPasswordVerification(string token);
        public RestaurantModel GetUser(string Id);
        public  Task<bool> Update(RestaurantModel user);
        public ImageDataModel ImagePath(RestaurantModel restaurantModel);
        public  Task<bool> UpdateResturant(RestaurantUpdateModel user, RestaurantModel userModel);
        public  Task<ImageDataModel> UpdateImage(PhotoUpdate photoUpdate, RestaurantModel user);
        public void ResetPassword(string id, RestaurantUpdateModel userUpdateModel);
        public  Task<RestaurantModel> CreateAdmin(RestaurantInputModel userResponse);
    }
}
