using NHASoftware.Models;

namespace NHASoftware
{
    public static class TaskSender
    {
        public static void SendTaskReminder(TaskItem item)
        {
            Console.WriteLine("This is a recurring job being fired. ");
        }
    }
}
