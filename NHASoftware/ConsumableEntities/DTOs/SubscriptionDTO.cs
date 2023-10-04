using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace NHASoftware.ConsumableEntities.DTOs
{
    public class SubscriptionDTO
    {
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; } = string.Empty;
        public DateTime SubscriptionDate { get; set; }
        public decimal SubscriptionCost { get; set; }
        public int SubscriptionDay { get; set; }
    }
}
