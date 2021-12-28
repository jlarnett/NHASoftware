using Hangfire;
using NHASoftware.Data;
using NHASoftware.Models;
using NHASoftware.Services;

namespace NHASoftware
{
    public class FrequencyHandler
    {
        public List<TaskItem> taskItems { get; set; }
        private readonly ApplicationDbContext _context;
        private TaskSender taskSender;


        public FrequencyHandler(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            taskSender = new TaskSender(emailService, context);
        }

        public DateTime GenerateNextDate(DateTime startDate, TaskFrequency frequency)
        {
            /**********************************************************************************************
            *  Takes in the inputted task date & finds the next trigger date. If future returns inputted date.
            *  else it only takes monthly frequency currently. Adds one month to inputted date.
            *  also has ability to just use days that isn't implemented in the cron string yet.
            **********************************************************************************************/
            DateTime newDate;

            //Adds minutes to the date so that today value Next Dates are generated correctly.
            newDate = startDate.AddMinutes(1400);
            

            if (newDate > DateTime.Today)
            {
                return startDate;
            }
            else
            {
                newDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, startDate.Day);
            }

            if (frequency.FrequencyName.ToLower() == "monthly")
            {
                return newDate.AddMonths(1);
            }
            else
            {
                return newDate.AddDays(frequency.FrequencyValue);
            }
        }
        
        public void GetRelevantTask()
        {
            /**********************************************************************************************
             *  This is ran daily. Gets all the task that hangfire job need created for.
             *  Creates the recurring job in hangfire.
             **********************************************************************************************/

            taskItems = _context.Tasks.Where(t => t.NextTaskDate.Month <= DateTime.Today.Month && t.NextTaskDate.Year <= DateTime.Today.Year && t.JobCrated != true).ToList();

            foreach (var item in taskItems)
            {
                string cronString = String.Format("{0} {1} {2} * *", item.TaskExecutionTime.Minutes, item.TaskExecutionTime.Hours, item.NextTaskDate.Day);
                RecurringJob.AddOrUpdate("TaskId: " + item.TaskId, () => taskSender.SendTaskReminder(item), cronString, TimeZoneInfo.Local);

                item.JobCrated = true;
                _context.SaveChangesAsync();
            }
        }
    }
}
