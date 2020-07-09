using System;
using System.Collections.Generic;
using System.Text;

namespace JWT_Token.Configurations
{
    public interface IJwtSetting
    {
        public string SecretKey { get; set; }
        public string maiVerificationKey { get; set; }
        public string passwordResetKey { get; set; }
        public string invitationKey { get; set; }
    }
}
