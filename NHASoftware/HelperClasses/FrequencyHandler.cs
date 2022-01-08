using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using NHASoftware.Data;
using NHASoftware.Models;
using NHASoftware.Services;

namespace NHASoftware
{
    public class FrequencyHandler
    {
        private readonly ApplicationDbContext _context;

        public FrequencyHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public DateTime GenerateNextDate(DateTime startDate, TaskFrequency frequency)
        {
            /**************************************************************************************************************
            *  Takes in the inputted task date & finds the next trigger date. If future returns inputted date.
            *  else it only takes monthly, daily, custom frequency. Custom dates simply use the day value provided.
            *  
            ****************************************************************************************************************/

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
            else if (frequency.FrequencyName.ToLower().Equals("daily"))
            {
                return newDate.AddDays(1);
            }
            else
            {
                return newDate.AddDays(frequency.FrequencyValue);
            }
        }
        
        public string ReturnFrequencyCronDate(TaskItem item)
        {
            /***************************************************************************************
             *  This method helps the handler generate correct cron string based on frequency.
             ***************************************************************************************/

            if (item.Frequency.FrequencyName == "monthly")
            {
                return String.Format("{0} {1} {2} * *", item.TaskExecutionTime.Minutes, item.TaskExecutionTime.Hours, item.NextTaskDate.Day);
            }
            else if(item.Frequency.FrequencyName == "Daily")
            {
                return String.Format("{0} {1} {2} * *", item.TaskExecutionTime.Minutes, item.TaskExecutionTime.Hours, "*");
            }
            else
            {
                return String.Format("{0} {1} {2} * *", item.TaskExecutionTime.Minutes, item.TaskExecutionTime.Hours, "*/" + item.Frequency.FrequencyValue);
            }
        }
    }
}
