using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NHASoftware.Models
{
    public class TaskItem
    {
        public int TaskId { get; set; }
        public string TaskDescription { get; set; }
        public bool TaskIsFinished { get; set; }

        [DataType(DataType.Date)]
        public DateOnly TaskStartDate { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly TaskExecutionTime { get; set; }

        public TaskFrequency Frequency { get; set; }
        public int FrequencyId { get; set; }

        public IdentityUser? User { get; set; }
        [Required]
        public string UserId { get; set; }

    }
}
