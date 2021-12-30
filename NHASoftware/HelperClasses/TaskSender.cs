
using System.Runtime.Loader;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private IEmailSender _emailService = null;
        private ApplicationDbContext _context;

        public TaskSender(IEmailSender emailService, ApplicationDbContext context)
        {
            /************************************************************************************
             *      Gets the email service interface.
             ************************************************************************************/

            _emailService = emailService;
            _context = context;
        }

        public async Task SendTaskReminder(TaskItem item)
        {
            /*************************************************************************************
             *  Takes task item from the recurring job & Sends Email to user.
             *************************************************************************************/

            var user = _context.Users.Find(item.UserId);
            await _emailService.SendEmailAsync(user.Email, "Task Reminder For", item.TaskDescription);
        }
    }
}
