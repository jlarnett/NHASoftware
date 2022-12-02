namespace NHASoftware.ViewModels
{
    public class IndexPageViewModel
    {
        public int TaskCount { get; set; }
        public int SubscriptionCount { get; set; }

        public IndexPageViewModel(int subCount, int taskCount)
        {
            TaskCount = subCount;
            SubscriptionCount = subCount;
        }
    }
}
