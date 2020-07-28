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
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.WebUtilities;
using Model;
using Model.Entities;
using Model.Input_Model;
using Model.View_Model;
using Repository;
using Services.Email;
using Services.FileUploadService;
using Services.Helper_Services;
using Services.UtilityService;
using RazorLight;

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
                Id = userResponse.Id,
                restaurantName = userResponse.restaurantName,
                firstName = userResponse.firstName,
                lastName = userResponse.lastName,
                username = userResponse.userName,
                password = hashedPassword,
                email = userResponse.email,
                //isEmailVerified = true,
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
                Id = GetUniqueId(),
                restaurantName = userResponse.restaurantName,
                firstName = userResponse.lastName,
                lastName = userResponse.lastName,
                username = userResponse.userName,
                password = hashedPassword,
                email = userResponse.email,
                //isEmailVerified = true,
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
        public async Task<RestaurantModel> UpdateResturant(RestaurantUpdateModel user,RestaurantModel userModel)
        {
            //var updateR = new RestaurantModel
            //{
            //    Id = userModel.Id,
            //    restaurantName = user.restaurantName,
            //    firstName = user.firstName,
            //    lastName = user.lastName,
            //    logo = userModel.logo,
            //    backgroundImage = userModel.backgroundImage,
            //    password = userModel.password,
            //    email = user.email,
            //    //isEmailVerified = userModel.isEmailVerified,
            //    isBlockedUser = userModel.isBlockedUser,
            //    role = userModel.role
            //};

            if(user.firstName != null && user.lastName != null)
            {
                userModel.firstName = user.firstName;
                userModel.lastName = user.lastName;
            }

            if (user.restaurantName != null)
            {
                userModel.restaurantName = user.restaurantName;
            }

            if(user.password != null)
            {
                string hashedPassword = _passwordManager.HashPassword(user.password);
                userModel.password = hashedPassword;
            }
    
            await _repository.UpdateAsync<RestaurantModel>(e => e.Id == userModel.Id, userModel);
            return userModel.RemovePassword();
        }
        public  RestaurantModel GetUser(string Id)
        {
            return  _repository.GetItem<RestaurantModel>(d => d.Id == Id);
        }
        public RestaurantModel GetUserByUsername(string username)
        {
            return  _repository.GetItem<RestaurantModel>(d => d.username == username);
        }

        public async Task<RestaurantModel> GetUserByEmail(string email)
        {
            return await _repository.GetItemAsync<RestaurantModel>(d => d.email == email);
        }

        public async void VerificationMailSender(string emailTo, string subject, string token,EmailTemplateModel emailTemplateModel)
        {
            var parameter = new Dictionary<string, string>() { { "token", token } };
            var requestUrl = new Uri(QueryHelpers.AddQueryString(emailTemplateModel.link, parameter));

            emailTemplateModel.link = requestUrl.ToString();

            var templateFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates");
            var engine = new RazorLightEngineBuilder().UseFileSystemProject(templateFolderPath).UseMemoryCachingProvider().Build();
            string result = await engine.CompileRenderAsync("EmailTemplate.cshtml", emailTemplateModel);

            MailModel mailModel = new MailModel
            {
                To = emailTo,
                MessageBody = result,
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
        public ImageDataModel ImagePath(string imageName)
        {
            string fileType = "jpg";
            string file = "";
            if (imageName != null)
            {

                fileType = getExtension(imageName);
                file = Path.Combine(Directory.GetCurrentDirectory(), _routes.StaticDir, imageName);

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
            await _fileUploadService.DeleteImage(user.logo);
            var path = await _fileUploadService.UploadSingleFile(photoUpdate.profilePhoto, FIleDirectories.ImageDir);

            user.logo = path;
            await Update(user);

            var imageData = ImagePath(user.logo);

            return imageData;
        }
        public async Task<ImageDataModel> UpdateBackgroudImage(PhotoUpdate photoUpdate, RestaurantModel user)
        {
            await _fileUploadService.DeleteImage(user.logo);
            var path = await _fileUploadService.UploadSingleFile(photoUpdate.profilePhoto, FIleDirectories.ImageDir);

            user.backgroundImage = path;
            await Update(user);

            var imageData = ImagePath(user.backgroundImage);

            return imageData;
        }
        public async void ResetPassword(string id, RestaurantUpdateModel restaurantUpdateModel)
        {
            var user = GetUser(id);

            await UpdateResturant(restaurantUpdateModel, user);
        }

        public async void PasswordRecovery(RestaurantModel restaurantModel)
        {
            string hashedPassword = _passwordManager.HashPassword(restaurantModel.password);
            restaurantModel.password = hashedPassword;
            await Update(restaurantModel);
        }

        public void SendRecoveryMail(string clientId,RestaurantModel restaurantModel)
        {
           
            string oneTimeToken = GetPasswordRecoverToken(restaurantModel);
            var clientData = GetClientInfo(clientId);
            string redirectionRoute = clientData.host + clientData.recoverRoute;
            var emailModel = new EmailTemplateModel()
            { 
                text = "This mail is from Restaurant Management system. Click the button below to reset your password", 
                buttonText = "Reset Password", 
                link =redirectionRoute
            };
        

            VerificationMailSender(restaurantModel.email, "Reset Your Password", oneTimeToken,emailModel);
        }

        public string GetUniqueId()
        {
            string first = DateTime.Now.ToString("yyMMddHHmmssff");
            string last = GetRandomString();
            string unique_id = first + "-" + last;

            return unique_id;
        }

        public string GetRandomString()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 20; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString().ToLower();
        }

        public bool isRestaurantAvailable(string userId)
        {

            var user = GetUser(userId);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public bool isUserNameAvailable(string username)
        {
            var user = GetUserByUsername(username);
            if (user == null)
            {
                return false;
            }
            return true;
        }
        public ClientModel GetClientInfo(string clientId)
        {
            return _repository.GetItem<ClientModel>(c => c._id == clientId);
        }
    }
}
