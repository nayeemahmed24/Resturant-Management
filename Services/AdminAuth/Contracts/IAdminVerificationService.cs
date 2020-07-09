using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.AdminAuth.Contracts
{
    public interface IAdminVerificationService
    {
        public  Task<bool> VerifyUserName(string UserName);
        public  Task<bool> IsEmailAvailable(string email);
    }
}
