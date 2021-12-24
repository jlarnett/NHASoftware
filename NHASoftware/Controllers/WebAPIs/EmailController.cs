using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHASoftware.Models;
using NHASoftware.Services;

namespace NHASoftware.Controllers.WebAPIs
{
    [ApiController]
    public class EmailController : ControllerBase
    {
        private IEmailService _emailService = null;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("email/SendEmail")]
        public bool SendEmail([Bind("EmailToId, EmailToName, EmailSubject, EmailBody")] EmailData emailData)
        {
            return _emailService.SendEmail(emailData);
        }
    }
}
