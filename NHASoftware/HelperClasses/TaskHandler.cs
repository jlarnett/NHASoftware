using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using NHASoftware.DBContext;
using NHASoftware.Entities;
using NHASoftware.Entities.Identity;

namespace NHASoftware.HelperClasses
{
    public class TaskHandler
    {
        //Dependency Injected Services
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        //Custom Classes
        private FrequencyHandler frequencyHandler;
        private TaskSender taskSender;

        public TaskHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailService)
        {
            _context = context;
            _userManager = userManager;
            taskSender = new TaskSender(emailService, context);
            frequencyHandler = new FrequencyHandler(context);
        }

        public void ClearDatedJobs()
        {
            //Gets the users outside task deletion tolerence date. Then adds all task related to users to list. 
            //Removes all recurring Jobs & Task From Database

            int toleranceDays = 40;
            var AllUsers = _userManager.Users.ToList();
            List<ApplicationUser> datedUsers = new List<ApplicationUser>();

            foreach (var user in AllUsers)
            {
                if((DateTime.Today - user.LastLoginDate).TotalDays > toleranceDays)
                    datedUsers.Add(user);
            }

            List<TaskItem> itemToDelete = new List<TaskItem>();

            foreach (var user in datedUsers)
            {
              itemToDelete.AddRange(_context.Tasks.Where(t => t.UserId == user.Id).ToList());
            }

            foreach (var item in itemToDelete)
            {
                RecurringJob.RemoveIfExists("TaskId: " + item.TaskId);
                _context.Tasks.RemoveRange(itemToDelete);
            }
        }

        public void CreateNewTaskJobs()
        {
            /**********************************************************************************************
             *  This is ran daily. Gets all the task that hangfire job need created for.
             *  Creates the recurring job in hangfire.
             **********************************************************************************************/

            List<TaskItem> taskItems = _context.Tasks.Where(t => t.NextTaskDate.Month <= DateTime.Today.Month && t.NextTaskDate.Year <= DateTime.Today.Year && t.JobCrated != true).ToList();

            foreach (var item in taskItems)
            {
                item.Frequency = _context.Frequencies.Find(item.FrequencyId);
                string cronString = frequencyHandler.ReturnFrequencyCronDate(item);

                RecurringJob.AddOrUpdate("TaskId: " + item.TaskId, () => taskSender.SendTaskReminder(item), cronString, TimeZoneInfo.Local);

                item.JobCrated = true;
                _context.SaveChangesAsync();
            }
        }
    }
}

