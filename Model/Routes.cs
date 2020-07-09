using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
   
        public class Routes : IRoutes
        {
            public string MailVerify { get; set; }
            public string RecoverPassword { get; set; }
            public string ResetRoute { get; set; }
            public string InvitationRoute { get; set; }
            public string StaticDir { get; set; }
        }
        public interface IRoutes
        {
            public string MailVerify { get; set; }
            public string RecoverPassword { get; set; }
            public string ResetRoute { get; set; }
            public string InvitationRoute { get; set; }
            public string StaticDir { get; set; }
        }
    
}
