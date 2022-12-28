using System.ComponentModel.DataAnnotations;
using NHASoftware.BL.Entities.Identity;

namespace NHASoftware.BL.Entities
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public int SubscriptionDay { get; set; }
        public decimal SubscriptionCost { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public string UserId { get; set; }

        public TaskItem? TaskItem { get; set; }
        public int? TaskItemId { get; set; }
    }
}
