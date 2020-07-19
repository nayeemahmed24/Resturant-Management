using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using Model;
using Services.Email.Configuration;

namespace Services.Email
{
    public class MailSender : IMailSender
    {
        private SmtpClient server;
        private IMailSetting _mailSetting;
        public MailSender(IMailSetting mailSetting)
        {
            _mailSetting = mailSetting;
            server = new SmtpClient(mailSetting.MailHost, mailSetting.Port);
            server.UseDefaultCredentials = true;
            server.Credentials = new NetworkCredential(mailSetting.Sender, mailSetting.Password);
            server.EnableSsl = true;
        }
        public void sendMail(MailModel mailModel)
        {
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(mailModel.To));
            message.From = new MailAddress(_mailSetting.Sender);
            message.Subject = mailModel.Subject;
            message.IsBodyHtml = true;
            message.Body = mailModel.MessageBody;
            server.Send(message);

        }
    }

    public interface IMailSender
    {
        public void sendMail(MailModel mailModel);
    }
}
