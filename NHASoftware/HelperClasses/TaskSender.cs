using System.Runtime.Loader;
using NHASoftware.Controllers;
using NHASoftware.Controllers.WebAPIs;
using NHASoftware.Data;
using NHASoftware.Models;
using NHASoftware.Services;
using NuGet.DependencyResolver;

namespace NHASoftware
{
    public class TaskSender
    {
        private IEmailService _emailService = null;
        private ApplicationDbContext _context;

        public TaskSender(IEmailService emailService, ApplicationDbContext context)
        {
            /************************************************************************************
             *      Gets the email service interface.
             ************************************************************************************/

            _emailService = emailService;
            _context = context;
        }

        public bool SendTaskReminder(TaskItem item)
        {
            /*************************************************************************************
             *  Takes task item from the recurring job & Sends Email to user.
             *************************************************************************************/

            var user = _context.Users.Find(item.UserId);

            EmailData emailData = new EmailData()
            {
                EmailToId = user.Email,
                EmailSubject = "Task Reminder for: " + item.TaskDescription,
                EmailBody = String.Format("This is your email reminder to complete this task: {0}", item.TaskDescription),
                EmailToName = user.Email
            };

            return _emailService.SendEmail(emailData);
        }
    }
}
