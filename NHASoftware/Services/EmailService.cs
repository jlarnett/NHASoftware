using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Options;
using MimeKit;
using NHASoftware.Configuration;
using NHASoftware.Models;

namespace NHASoftware.Services
{
    /****************************************************************************************************
     *  This namespace currently holds IEmailService Interface & EmailService which implements it.
     ****************************************************************************************************/
    public interface IEmailService
    {
        bool SendEmail(EmailData emailData);
    }

    public class EmailService : IEmailService
    {
        private EmailSettings _emailSettings = null;

        public EmailService(IOptions<EmailSettings> options)
        {
            //Email Settings is gotten from Ioptions interface.
            _emailSettings = options.Value;
        }

        public bool SendEmail(EmailData emailData)
        {
            /****************************************************************************
            *   Takes emailData model and sends basic task email information. 
            ***************************************************************************/

            try
            {
                MimeMessage emailMessage = new MimeMessage();
                MailboxAddress emailFrom = new MailboxAddress(_emailSettings.Name, _emailSettings.EmailId);

                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new MailboxAddress(emailData.EmailToName, emailData.EmailToId);

                emailMessage.To.Add(emailTo);
                emailMessage.Subject = emailData.EmailSubject;

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.TextBody = emailData.EmailBody;
                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                //SMTP client connection & sends emailMessage email. 
                SmtpClient emailClient = new SmtpClient();

                emailClient.Connect(_emailSettings.Host, _emailSettings.Port, _emailSettings.UseSSL);
                emailClient.Authenticate(_emailSettings.EmailId, _emailSettings.Password);
                emailClient.Send(emailMessage);
                emailClient.Disconnect(true);
                emailClient.Dispose();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

}
