using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Infrastructure.Utilities
{
    public class MailService : IMailService
    {
        public async Task<bool> SendEmail(string from, string to, string subject, string body)
        {
            try
            {
                MailMessage oMailMessage = new MailMessage();

                oMailMessage.From = new MailAddress(from.Trim());
                oMailMessage.To.Add(new MailAddress(to.Trim()));
                oMailMessage.Subject = subject.Trim();
                oMailMessage.Body = body.Trim();
                oMailMessage.IsBodyHtml = true;

                SmtpClient oSmtpClient = new SmtpClient();
                oSmtpClient.SendAsync(oMailMessage,null);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
