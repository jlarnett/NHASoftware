
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
             *  Takes task item from the recurring job & Sends Email to user. Along with all
             *************************************************************************************/

            string reminderMessage = "Task Reminder For: " + item.TaskDescription + "\n";
            var taskSubscriptions = _context.Subscriptions.Where(c => c.TaskItemId == item.TaskId);

            if (taskSubscriptions.Any())
            {
                foreach (var sub in taskSubscriptions)
                {
                    reminderMessage += sub.SubscriptionName + ": " + sub.SubscriptionCost + "\n";
                }
            }

            var user = _context.Users.Find(item.UserId);
            await _emailService.SendEmailAsync(user.Email, "Task Reminder For " + item.TaskDescription, reminderMessage);
        }
    }
}
