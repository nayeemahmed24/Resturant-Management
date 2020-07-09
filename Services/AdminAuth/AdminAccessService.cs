using System;
using System.Collections.Generic;
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

        public AdminAccessService(IMailSender mailSender,IMongoRepository repository,IPasswordManager passwordManager,IRoutes routes, ITokenGenerator tokenGenerator, IJwtSetting jwtSetting)
        {
            _repository = repository;
            _mailSender = mailSender;
            _passwordManager = passwordManager;
            _tokenGenerator = tokenGenerator;
            _jwtSetting = jwtSetting;
            _routes = routes;
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
                Id = GetRandomString(),
                managerFirstName = "",
                managerLastName = "",
                restaurantName = invitationModel.restaurantName,
                email = invitationModel.email,
                role = Role.User
            };

            string invitationToken = _tokenGenerator.generateToken(restaurantModel, _jwtSetting.maiVerificationKey, 48);
            VerificationMailSender(restaurantModel.email, "Invitation Link", invitationToken, _routes.InvitationRoute);
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
        public string GetRandomString()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 10; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString().ToLower();
        }
    }
}
