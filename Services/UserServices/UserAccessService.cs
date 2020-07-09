using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWT_Token;
using JWT_Token.Configurations;
using Model;
using Model.Entities;
using Model.Input_Model;
using Model.View_Model;
using Repository;
using Services.Email;
using Services.FileUploadService;
using Services.Helper_Services;

namespace Services.UserServices
{
    public class UserAccessService:IUserAccessService
    {
        private CustomTokenValidator _customTokenValidator;
        private IJwtSetting _jwtSetting;
        private IPasswordManager _passwordManager;
        private readonly IMongoRepository _repository;
        private ITokenGenerator _tokenGenerator;
        private IMailSender _mailSender;
        private IRoutes _routes;
        private IFileUploadService _fileUploadService;
        public UserAccessService(IFileUploadService fileUploadService,IRoutes routes,IMailSender mailSender,ITokenGenerator tokenGenerator,IMongoRepository repository,IPasswordManager passwordManager,CustomTokenValidator customTokenValidator,IJwtSetting jwtSetting)
        {
            _fileUploadService = fileUploadService;
            _routes = routes;
            _mailSender = mailSender;
            _tokenGenerator = tokenGenerator;
            _customTokenValidator = customTokenValidator;
            _repository = repository;
            _jwtSetting = jwtSetting;
            _passwordManager = passwordManager;

        }
        public ClaimsPrincipal ValidateMailVerifyToken(string token)
        {
            var claims = _customTokenValidator.validateToken(token, _jwtSetting.invitationKey);
            return claims;
        }
        public async Task<RestaurantModel> Create(RestaurantInputModel userResponse)
        {
            string hashedPassword = _passwordManager.HashPassword(userResponse.password);
            RestaurantModel userModel = new RestaurantModel
            {
                restaurantName = userResponse.restaurantName,
                managerFirstName = userResponse.managerFirstName,
                managerLastName = userResponse.managerLastName,
                password = hashedPassword,
                email = userResponse.email,
                isEmailVerified = true,
                role = Role.User
            };
            
            await _repository.SaveAsync(userModel);
            // string mailVarificationToken = _tokenGenerator.generateToken(userModel,_jwtSetting.maiVerificationKey,24);
            // VerificationMailSender(userResponse.email, "Verify Email", mailVarificationToken,_routes.MailVerify);
            userModel.password = null;
            return userModel;
        }
        public async Task<RestaurantModel> CreateAdmin(RestaurantInputModel userResponse)
        {
            string hashedPassword = _passwordManager.HashPassword(userResponse.password);
            RestaurantModel userModel = new RestaurantModel
            {
                restaurantName = userResponse.restaurantName,
                managerFirstName = userResponse.managerFirstName,
                managerLastName = userResponse.managerLastName,
                password = hashedPassword,
                email = userResponse.email,
                isEmailVerified = true,
                role = Role.Admin
            };

            await _repository.SaveAsync(userModel);
            // string mailVarificationToken = _tokenGenerator.generateToken(userModel,_jwtSetting.maiVerificationKey,24);
            // VerificationMailSender(userResponse.email, "Verify Email", mailVarificationToken,_routes.MailVerify);
            userModel.password = null;
            return userModel;
        }

        public async Task<bool> Update(RestaurantModel user)
        {
            await _repository.UpdateAsync<RestaurantModel>(e => e.Id == user.Id, user);
            return true;
        }
        public async Task<bool> UpdateResturant(RestaurantUpdateModel user,RestaurantModel userModel)
        {
            var updateR = new RestaurantModel
            {
                Id = userModel.Id,
                restaurantName = user.restaurantName,
                managerFirstName = user.managerFirstName,
                managerLastName = user.managerLastName,
                logo = userModel.logo,
                backgroundImage = userModel.backgroundImage,
                password = userModel.password,
                email = user.email,
                isEmailVerified = userModel.isEmailVerified,
                isBlockedUser = userModel.isBlockedUser,
                role = userModel.role
            };

    
            await _repository.UpdateAsync<RestaurantModel>(e => e.Id == userModel.Id, updateR);
            return true;
        }
        public  RestaurantModel GetUser(string Id)
        {
            return  _repository.GetItem<RestaurantModel>(d => d.Id == Id);
        }
        public async Task<RestaurantModel> GetUserByUsername(string username)
        {
            return await _repository.GetItemAsync<RestaurantModel>(d => d.restaurantName == username);
        }

        public async Task<RestaurantModel> GetUserByEmail(string email)
        {
            return await _repository.GetItemAsync<RestaurantModel>(d => d.email == email);
        }

        public void VerificationMailSender(string emailTo, string subject, string token, string api)
        {
            String link = api + token;
            MailModel mailModel = new MailModel
            {
                To = emailTo,
                MessageBody = link,
                Subject = subject
            };
            _mailSender.sendMail(mailModel);
        }
        public string GetPasswordRecoverToken(RestaurantModel userModel)
        {
            string token = _tokenGenerator.generateToken(userModel, userModel.password, 1);
            return token;
        }
        public bool IsAuthorizedUser(RestaurantModel userModel, string password)
        {
            string hashedPassword = userModel.password;
            bool passwordMatched = _passwordManager.VerifyPassword(hashedPassword, password);
            return passwordMatched;
        }
        public TokenModel GetAuthenticationToken(RestaurantModel user)
        {
            string token = _tokenGenerator.generateToken(user, _jwtSetting.SecretKey, 168);
            TokenModel tokenModel = new TokenModel();
            tokenModel.token = token;
            return tokenModel;
        }
        public ImageDataModel ImagePath(RestaurantModel restaurantModel)
        {
            string fileType = "jpg";
            string file = "";
            if (restaurantModel.logo != null)
            {

                fileType = getExtension(restaurantModel.logo);
                file = Path.Combine(Directory.GetCurrentDirectory(), _routes.StaticDir, restaurantModel.logo);

            }
            bool s = File.Exists(file);
            if (!s)
            {
                var data = DefaultImage();
                return data;
            }
            var imageModel = new ImageDataModel
            {
                path = file,
                Imagetype = fileType
            };
            return imageModel;
        }
        public ImageDataModel DefaultImage()
        {
            string file = Path.Combine(Directory.GetCurrentDirectory(), _routes.StaticDir, "images", "default.png");
            string fileType = "png";

            var imageModel = new ImageDataModel
            {
                path = file,
                Imagetype = fileType
            };
            return imageModel;
        }

        private string getExtension(string mainImage)
        {
            var fileSplit = mainImage.Split(".");
            var arrayLength = fileSplit.Length;
            var fileExtension = fileSplit[arrayLength - 1];
            return fileExtension;
        }
        public  string ResetPasswordVerification(string token)
        {
            var jwthandler = new JwtSecurityTokenHandler();
            var jwtToken = jwthandler.ReadToken(token) as JwtSecurityToken;
            var userId = jwtToken.Claims.First(claim => claim.Type == Claims.UserId)?.Value;
            var user =  GetUser(userId);
            string verifiedId = null;

            var claims = _customTokenValidator.validateToken(token, user.password);
            if (claims != null)
            {
                verifiedId = claims.FindFirst(claim => claim.Type == Claims.UserId)?.Value;
                return verifiedId;
            }
            return null;
        }

        public async Task<ImageDataModel> UpdateImage(PhotoUpdate photoUpdate, RestaurantModel user)
        {
            var path = await _fileUploadService.UploadSingleFile(photoUpdate.profilePhoto, FIleDirectories.ImageDir);

            user.logo = path;
            Update(user);

            var imageData = ImagePath(user);

            return imageData;
        }
        public void ResetPassword(string id, RestaurantUpdateModel userUpdateModel)
        {
            var user = GetUser(id);
            user.password = userUpdateModel.password;
            Update(user);
        }
    }
}
