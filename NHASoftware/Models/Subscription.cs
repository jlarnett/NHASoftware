using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NHASoftware.Models
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
    }
}
