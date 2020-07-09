using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Email.Configuration
{
    public class MailSetting : IMailSetting
    {
        public string MailHost { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
    }

    public interface IMailSetting
    {
        public string MailHost { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
    }
}
