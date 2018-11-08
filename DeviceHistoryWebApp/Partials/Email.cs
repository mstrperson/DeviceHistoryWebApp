using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;


namespace DeviceHistoryWebApp.Partials
{
    public static class Email
    {
        private static SmtpClient smtpClient = new SmtpClient("localhost");
        private const string NO_REPLY = "noreply@winsor.edu";

        public static void SendMessage(string subject, string body, string toAddress, string replyTo = NO_REPLY)
        {
            MailMessage mail = new MailMessage(NO_REPLY, toAddress, subject, body);
            mail.ReplyToList.Add(new MailAddress(replyTo));

            smtpClient.Send(mail);
        }
    }
}