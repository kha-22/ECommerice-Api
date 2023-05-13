using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Infrastructure.Utilities
{
    public interface IMailService
    {
        //Task SendEmailAsync(MailRequest mailRequest);
        Task<bool> SendEmail(string from, string to, string subject, string body);
    }
}
