﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NHASoftware.Models;

namespace NHASoftware.ViewModels
{
    public class TaskFormViewModel
    {
        public int TaskId { get; set; }
        public string TaskDescription { get; set; }
        public bool TaskIsFinished { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime TaskStartDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public TimeSpan? TaskExecutionTime { get; set; }

        public TaskFrequency? Frequency { get; set; }
        public int FrequencyId { get; set; }

        public IdentityUser? User { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}