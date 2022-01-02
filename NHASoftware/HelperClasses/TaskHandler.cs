using Hangfire;
using Microsoft.AspNetCore.Identity;
using NHASoftware.Data;
using NHASoftware.Models;

namespace NHASoftware.HelperClasses
{
    public class TaskHandler
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskHandler(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
    }
}

