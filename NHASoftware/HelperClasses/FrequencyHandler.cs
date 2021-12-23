using Hangfire;
using NHASoftware.Data;
using NHASoftware.Models;

namespace NHASoftware
{
    public class FrequencyHandler
    {
        public List<TaskItem> taskItems { get; set; }
        private readonly ApplicationDbContext _context;

        public FrequencyHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public DateTime GenerateNextDate(DateTime startDate, TaskFrequency frequency)
        {
            DateTime newDate;

            if (startDate > DateTime.Today)
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
            else if (frequency.FrequencyName.ToLower() == "quarterly")
            {
                return newDate.AddMonths(3);
            }
            else if (frequency.FrequencyName.ToLower() == "yearly")
            {
                return newDate.AddYears(1);
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
                RecurringJob.AddOrUpdate("TaskId: " + item.TaskId, () => TaskSender.SendTaskReminder(item), cronString, TimeZoneInfo.Local);

                item.JobCrated = true;
                _context.SaveChangesAsync();
            }
        }
    }
}
