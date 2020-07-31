using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using JWT_Token;
using JWT_Token.Configurations;
using Model;
using Model.Entities;
using Model.Input_Model;
using Repository;
using Services.AdminAuth.Contracts;
using Services.Email;
using Services.Helper_Services;
using Microsoft.AspNetCore.WebUtilities;
using Services.Paginator;
using Model.View_Model;
using Services.UtilityService;
using RazorLight;
using System.IO;

namespace Services.AdminAuth
{
    public class AdminAccessService: IAdminAccessService
    {

        private readonly IMongoRepository _repository;
        private IPasswordManager _passwordManager;
        private ITokenGenerator _tokenGenerator;
        private IJwtSetting _jwtSetting;
        private IMailSender _mailSender;
        private IRoutes _routes;
        private IPaginator _paginator;

        public AdminAccessService(IMailSender mailSender,IPaginator paginator,IMongoRepository repository,IPasswordManager passwordManager,IRoutes routes, ITokenGenerator tokenGenerator, IJwtSetting jwtSetting)
        {
            _repository = repository;
            _mailSender = mailSender;
            _passwordManager = passwordManager;
            _tokenGenerator = tokenGenerator;
            _jwtSetting = jwtSetting;
            _routes = routes;
            _paginator = paginator;
        }

        public async Task<AdminUserModel> Create(AdminInputModel userResponse)
        {
            string hashedPassword = _passwordManager.HashPassword(userResponse.password);
            AdminUserModel userModel = new AdminUserModel
            {
                UserName = userResponse.UserName,
                Name = userResponse.Name,
                password = hashedPassword,
                Email = userResponse.Email,
                role = Role.Admin
            };
            await _repository.SaveAsync(userModel);
            userModel.password = null;
            return userModel;
        }

        public PaginatorModel<RestaurantModel> GetAllRestaurants(PageParameters pageParameters)
        {
            var data = _paginator.GetPaginatedData<RestaurantModel>("restaurantName", pageParameters.PageNumber, pageParameters.PageSize, data => data.role == "User").RemoveAllPassword();
            return data;
        }

        public async Task<AdminUserModel> GetUserByUserName(string Username)
        {
            return await _repository.GetItemAsync<AdminUserModel>(d => d.UserName == Username);
        }
        public bool IsAuthorizedUser(AdminUserModel userModel, string password)
        {
            string hashedPassword = userModel.password;
            bool passwordMatched = _passwordManager.VerifyPassword(hashedPassword, password);
            return passwordMatched;
        }
        public TokenModel GetAuthenticationToken(AdminUserModel user)
        {
            string token = _tokenGenerator.generateToken(user, _jwtSetting.SecretKey, 168);
            TokenModel tokenModel = new TokenModel();
            tokenModel.token = token;
            return tokenModel;
        }
        public void SendInvitation(InvitationModel invitationModel)
        {
            RestaurantModel restaurantModel = new RestaurantModel
            {
                Id = GetUniqueId(),
                firstName = "",
                lastName = "",
                restaurantName = invitationModel.restaurantName,
                email = invitationModel.email,
                role = Role.User
            };

            var clientDetails = GetClientInfo(invitationModel.ClientId);
            var receiverRoute = clientDetails.host + clientDetails.invitationRoute;

            string invitationToken = _tokenGenerator.generateToken(restaurantModel, _jwtSetting.invitationKey, 48);
            var emailModel = new EmailTemplateModel() {
                text ="Welcome to Restaurant Management service! You are now authorized to create an account for your restaurant. Click the button below to continue. Thank you.",
                buttonText = "Register Now",
                link = receiverRoute
            };
            VerificationMailSender(restaurantModel.email, "Invitation Link", invitationToken, emailModel);
        }
        public async void VerificationMailSender(string emailTo, string subject, string token, EmailTemplateModel emailTemplateModel)
        {
            var parameter = new Dictionary<string, string>() { { "token", token } };
            var requestUrl = new Uri(QueryHelpers.AddQueryString(emailTemplateModel.link, parameter));

            emailTemplateModel.link = requestUrl.ToString();

            var templateFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates");
            var engine = new RazorLightEngineBuilder().UseFileSystemProject(templateFolderPath).UseMemoryCachingProvider().Build();
            string template = await engine.CompileRenderAsync("EmailTemplate.cshtml", emailTemplateModel);

            MailModel mailModel = new MailModel
            {
                To = emailTo,
                MessageBody = template,
                Subject = subject
            };
            _mailSender.sendMail(mailModel);
        }

        public ClientModel GetClientInfo(string clientId)
        {
            return _repository.GetItem<ClientModel>(c => c._id == clientId);
        }

        public string GetUniqueId()
        {
            string first = DateTime.Now.ToString("yyMMddHHmmssff");
            string last = GetRandomString();
            string unique_id = first + "-" + last;

            return unique_id;
        }

        private string GetRandomString()
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
    }
}
