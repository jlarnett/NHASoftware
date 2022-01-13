using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace NHASoftware.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskDescription { get; set; }
        public bool TaskIsFinished { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime TaskStartDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public TimeSpan TaskExecutionTime { get; set; }

        public TaskFrequency Frequency { get; set; }
        public int FrequencyId { get; set; }

        public ApplicationUser? User { get; set; }
        [Required]
        public string? UserId { get; set; }

        public DateTime NextTaskDate { get; set; }

        public bool JobCrated { get; set; }

        public IList<Subscription>? Subscriptions { get; set; }
    }
}
