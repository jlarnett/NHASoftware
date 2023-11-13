using Microsoft.AspNetCore.Identity.UI.Services;
using NHASoftware.DBContext;
using NHASoftware.Entities;

namespace NHASoftware
{
    public class TaskSender
    {
        private readonly IEmailSender _emailService;
        private readonly ApplicationDbContext _context;

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
            var taskSubscriptions = _context.Subscriptions!.Where(c => c.TaskItemId == item.TaskId);

            if (taskSubscriptions.Any())
            {
                foreach (var sub in taskSubscriptions)
                {
                    reminderMessage += sub.SubscriptionName + ": " + sub.SubscriptionCost + "\n";
                }
            }

            var user = await _context.Users.FindAsync(item.UserId);

            if (user != null)
                await _emailService.SendEmailAsync(user.Email, "Task Reminder For " + item.TaskDescription, reminderMessage);
        }
    }
}
