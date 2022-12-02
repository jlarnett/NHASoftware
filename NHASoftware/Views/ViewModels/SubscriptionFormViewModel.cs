using System.ComponentModel.DataAnnotations;
using NHASoftware.Entities;
using NHASoftware.Entities.Identity;

namespace NHASoftware.ViewModels
{
    public class SubscriptionFormViewModel
    {
        public int SubscriptionId { get; set; }
        [Display(Name = "Description")]
        public string SubscriptionName { get; set; }

        [Display(Name = "Execution Date")]
        public DateTime SubscriptionDate { get; set; }

        public int SubscriptionDay { get; set; }


        [Display(Name = "Cost")]
        public decimal SubscriptionCost { get; set; }

        public ApplicationUser? User { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string UserId { get; set; }

        public TaskItem? TaskItem { get; set; }

        [Display(Name = "Linked Task Item")]
        public int? TaskItemId { get; set; }
    }
}
