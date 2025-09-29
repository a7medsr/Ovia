using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Ovia.Services.SendEmails
{
    public class MailSettings
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }


    }
}
