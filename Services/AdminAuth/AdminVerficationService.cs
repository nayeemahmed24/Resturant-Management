using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;
using Repository;
using Services.AdminAuth.Contracts;

namespace Services.AdminAuth
{
    public class AdminVerficationService :IAdminVerificationService
    {
        private readonly IMongoRepository _repository;
        public AdminVerficationService(IMongoRepository repository)
        {
            _repository = repository;

        }
        public async Task<bool> VerifyUserName(string UserName)
        {
            if (UserName != null)
            {
                var user = await _repository.GetItemAsync<AdminUserModel>(d =>d.UserName == UserName);
                if (user != null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            if (email != null)
            {
                var user = await _repository.GetItemAsync<AdminUserModel>(d => d.Email == email);
                if (user != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
    
}
