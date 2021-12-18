using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using NHASoftware.Models;

namespace NHASoftware.ViewModels
{
    public class SubscriptionFormViewModel
    {
        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public int SubscriptionDay { get; set; }
        public decimal SubscriptionCost { get; set; }
        public string UserId { get; set; }

        public SubscriptionFormViewModel()
        {
            SubscriptionId = 0;
        }

        public SubscriptionFormViewModel(Subscription sub)
        {
            SubscriptionId = sub.SubscriptionId;
            SubscriptionName = sub.SubscriptionName;
            SubscriptionDate = sub.SubscriptionDate;
            SubscriptionDay = sub.SubscriptionDay;
            SubscriptionCost = sub.SubscriptionCost;
            UserId = sub.UserId;
        }
    }
}
