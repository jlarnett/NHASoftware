namespace NHA.Website.Software.Services.Time
{
    public class TimeBender : ITimeBender
    {
        //Returns a rounded shorthand string for the age of post. E.G 2H A
        public string GetTimeShortHandString(DateTime time)
        {
            var outputShorthandUnit = string.Empty;

            var timeInSeconds = (int)(DateTime.UtcNow - time).TotalSeconds;
            var timeInMinutes = timeInSeconds / 60;
            var timeInHours = timeInMinutes / 60;
            var timeInDays = timeInHours / 24;
            var timeInYears = timeInDays / 365;


            if (timeInSeconds < 0)
            {
                return "0s ago";
            }
            if (timeInSeconds is < 60 and >= 0)
            {
                return $"{timeInSeconds}s ago";
            }
            if (timeInMinutes is < 60 and >= 1)
            {
                return $"{timeInMinutes}m ago";
            }
            if (timeInHours is < 24 and >= 1)
            {
                return $"{timeInHours}h ago";
            }
            if (timeInDays is < 365 and >= 1)
            {
                return $"{timeInDays}d ago";
            }
            if (timeInYears >= 1)
            {
                return $"{timeInYears}y ago";
            }

            return outputShorthandUnit;
        }

    }
}
