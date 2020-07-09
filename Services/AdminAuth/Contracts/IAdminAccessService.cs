using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JWT_Token;
using Model.Entities;
using Model.Input_Model;

namespace Services.AdminAuth.Contracts
{
    public interface IAdminAccessService
    {
        public Task<AdminUserModel> Create(AdminInputModel userResponse);
        public Task<AdminUserModel> GetUserByUserName(String Username);
        public TokenModel GetAuthenticationToken(AdminUserModel user);
        public bool IsAuthorizedUser(AdminUserModel userModel, string password);
        public void SendInvitation(InvitationModel invitationModel);
    }
}
